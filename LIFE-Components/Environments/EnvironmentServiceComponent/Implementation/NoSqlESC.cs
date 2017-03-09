using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConfigService;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;

namespace LIFE.Components.ESC.Implementation {

  public class NoSqlESC : IESC {

    private static IMongoCollection<BsonDocument> _collection;
    private readonly bool _distributed;

    private readonly bool _instantCommit;
    private readonly IDictionary<string, Type> _knownTypes;
    private readonly IDictionary<Guid, ISpatialEntity> _spatialEntities;
    private ConcurrentBag<UpdateOneModel<BsonDocument>> _entitesToMoveAtCommit;
    private ConcurrentBag<BsonDocument> _entitiesToAddAtCommit;
    private ConcurrentBag<FilterDefinition<BsonDocument>> _moveFilter;


    public NoSqlESC(bool instantCommit = false, bool distributed = false) {
      _instantCommit = instantCommit;
      _distributed = distributed;
      _spatialEntities = new ConcurrentDictionary<Guid, ISpatialEntity>();
      _knownTypes = new ConcurrentDictionary<string, Type>();
      _moveFilter = new ConcurrentBag<FilterDefinition<BsonDocument>>();
      _entitesToMoveAtCommit = new ConcurrentBag<UpdateOneModel<BsonDocument>>();
      _entitiesToAddAtCommit = new ConcurrentBag<BsonDocument>();


      var cfgClient = new ConfigServiceClient("http://marsconfig:8080/");
      var ip = cfgClient.Get("mongodb/ip");
      var port = cfgClient.Get("mongodb/port");
      IMongoClient client = new MongoClient("mongodb://" + ip + ":" + port);

      var database = client.GetDatabase("marslife");
      _collection = database.GetCollection<BsonDocument>("spatial_entities");
      // clean up collection
      _collection.DeleteManyAsync("{}").Wait();

      // create spatial 2dsphere index
      var key = Builders<BsonDocument>.IndexKeys.Geo2DSphere("loc");
      _collection.Indexes.CreateOneAsync(key).Wait();

      //_collection.Indexes.CreateOneAsync(Builders<BsonDocument>.IndexKeys.Hashed("AgentGuid")).Wait();
    }


    public bool Add(ISpatialEntity entity, Vector3 position, Direction rotation = null) {
      var movementVector = position - entity.Shape.Position;
      if (rotation == null) rotation = new Direction();
      var newShape = entity.Shape.Transform(movementVector, rotation);
      entity.Shape = newShape;
      var spatialEntityBson = ConvertEntityToBson(entity);
      if (_instantCommit) {
        _spatialEntities.Add(entity.AgentGuid, entity);
        _collection.InsertOneAsync(spatialEntityBson);
      }
      else _entitiesToAddAtCommit.Add(spatialEntityBson);
      return true;
    }


    public bool AddWithRandomPosition(ISpatialEntity entity, Vector3 min, Vector3 max, bool grid) {
      for (var attempt = 0; attempt < 100000; attempt++) {
        var position = GenerateRandomPosition(new Vector3(-90, 0, -180), new Vector3(90, 0, 180));
        var result = Add(entity, position);
        if (result) {
          _spatialEntities[entity.AgentGuid] = entity;
          return true;
        }
      }

      return false;
    }

    public void Remove(ISpatialEntity entity) {
      var filter = Builders<BsonDocument>.Filter.Eq("AgentGuid", entity.AgentGuid.ToString());

      _collection.DeleteOneAsync(filter);
      _spatialEntities.Remove(entity.AgentGuid);
    }

    public bool Resize(ISpatialEntity entity, IShape shape) {
      throw new NotImplementedException();
    }

    public MovementResult Move(ISpatialEntity entity, Vector3 movementVector, Direction rotation = null) {
      // update entity
      if (rotation == null)
        rotation = entity.Shape.Rotation;
      entity.Shape = entity.Shape.Transform(movementVector, rotation);

      // create filter for agent
      var filter = Builders<BsonDocument>.Filter.Eq("_id", new BsonBinaryData(entity.AgentGuid));

      if (_instantCommit) {
        // check for collisions
        var collisions = Explore(entity);
        var movementResult = new MovementResult(collisions);

        if (movementResult.Success) {
          var update = Builders<BsonDocument>.Update.Set("loc",
            new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
                new GeoJson2DGeographicCoordinates(
                  entity.Shape.Position.Z,
                  entity.Shape.Position.X))
              .ToBsonDocument());

          var result = _collection.UpdateOneAsync(filter, update);


          // store/update entity in local cache
          _spatialEntities[entity.AgentGuid] = entity;

          if (_distributed) {
            var updateEntity = Builders<BsonDocument>.Update.Set("SpatialEntity",
              new BsonString(JsonConvert.SerializeObject(entity, Formatting.Indented,
                new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}))
            );
            _collection.UpdateOneAsync(filter, updateEntity).Wait();
          }

          result.Wait();
        }
      }
      else {
        _moveFilter.Add(filter);
        _entitesToMoveAtCommit.Add(new UpdateOneModel<BsonDocument>(filter, Builders<BsonDocument>.Update.Set("loc",
            new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
              new GeoJson2DGeographicCoordinates(
                entity.Shape.Position.Z,
                entity.Shape.Position.X))))
        );
      }


      return new MovementResult();
    }

    public IEnumerable<ISpatialEntity> Explore(Sphere spatial, int maxResults = -1) {
      throw new NotImplementedException();
    }

    public IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial, Type agentType = null) {
      // Setup filter to find other entites
      FilterDefinition<BsonDocument> collisionFilter;

      if (agentType == null)
        collisionFilter = Builders<BsonDocument>.Filter.GeoWithinCenterSphere("loc",
          spatial.Shape.Position.Z, spatial.Shape.Position.X, spatial.Shape.Bounds.Width/6378.1);
      else
        collisionFilter = Builders<BsonDocument>.Filter.GeoWithinCenterSphere("loc",
                            spatial.Shape.Position.Z, spatial.Shape.Position.X, spatial.Shape.Bounds.Width/6378.1)
                          &
                          Builders<BsonDocument>.Filter.Eq("AgentType", agentType.AssemblyQualifiedName);

      //execute actually query
      return Explore(collisionFilter);
    }

    public IEnumerable<ISpatialEntity> Explore(IShape shape, Enum collisionType = null) {
      // Setup filter to find other entites
      FilterDefinition<BsonDocument> collisionFilter;
      if (collisionType == null)
        collisionFilter = Builders<BsonDocument>.Filter.GeoWithinCenterSphere("loc",
          shape.Position.Z, shape.Position.X, shape.Bounds.Width/6378.1);
      else
        collisionFilter = Builders<BsonDocument>.Filter.GeoWithinCenterSphere("loc",
                            shape.Position.Z, shape.Position.X, shape.Bounds.Width/6378.1)
                          &
                          Builders<BsonDocument>.Filter.Eq("CollisionType", collisionType.ToString());

      // execute actually query
      return Explore(collisionFilter);
    }


    public IEnumerable<ISpatialEntity> ExploreAll() {
      // if not distributed
      return _spatialEntities.Values;
      // todo: if distributed: fetch entities from whole cluster
    }

    public Vector3 MaxDimension { get; set; }

    public bool IsGrid { get; set; }


    /// <summary>
    ///   Signals the finish of a tick and executes a bunch of commit actions like
    ///   bulk addition of SpatialEntities
    /// </summary>
    public void CommitTick() {
      // Add
      if (_entitiesToAddAtCommit.Any()) {
        _collection.InsertManyAsync(_entitiesToAddAtCommit).Wait();
        _entitiesToAddAtCommit = new ConcurrentBag<BsonDocument>();
      }

      // Move
      if (_entitesToMoveAtCommit.Any()) {
        // build aggregated filter from single filters
        var ary = _entitesToMoveAtCommit.ToArray();

        // create bulk write model
        var models = new WriteModel<BsonDocument>[ary.Length];
        ary.CopyTo(models, 0);

        _collection.BulkWriteAsync(models).Wait();

        // reset collections
        _moveFilter = new ConcurrentBag<FilterDefinition<BsonDocument>>();
        _entitesToMoveAtCommit = new ConcurrentBag<UpdateOneModel<BsonDocument>>();
      }
    }

    #region private Methods

    /// <summary>
    ///   Generates a random position within the given bounds.
    /// </summary>
    /// <param name="min">Lower boundary.</param>
    /// <param name="max">Upper boundary.</param>
    /// <returns>The generated position.</returns>
    private Vector3 GenerateRandomPosition(Vector3 min, Vector3 max) {
      var x = GetRandomDouble(min.X, max.X);
      var y = GetRandomDouble(min.Y, max.Y);
      var z = GetRandomDouble(min.Z, max.Z);
      return new Vector3(x, y, z);
    }

    private double GetRandomDouble(double minimum, double maximum) {
      var random = new Random(54897439);
      return random.NextDouble()*(maximum - minimum) + minimum;
    }

    private IEnumerable<ISpatialEntity> Explore(FilterDefinition<BsonDocument> collisionFilter) {
      // prepare Bag for found entities
      var foundEntites = new ConcurrentBag<ISpatialEntity>();

      // filter database and apply operation on each object
      _collection.Find(collisionFilter).ForEachAsync(async doc => {
        // execute async
        await Task.Factory.StartNew(() => {
          var id = Guid.Parse(doc["AgentGuid"].AsString);

          // try to use cache
          if (_spatialEntities.ContainsKey(id)) {
            foundEntites.Add(_spatialEntities[id]);
          }
          // no luck so reflect the type and load SpatialEntity
          else if (_distributed) {
            var typeString = doc["AgentType"].AsString;
            Type type;

            // again try to use cache and avoid slow Type.GetType() call
            if (_knownTypes.ContainsKey(typeString)) {
              type = _knownTypes[typeString];
            }
            else {
              type = Type.GetType(typeString);
              _knownTypes[typeString] = type;
            }

            var spatialEntity =
              (ISpatialEntity) JsonConvert.DeserializeObject(doc["SpatialEntity"].AsString, type,
                new JsonSerializerSettings {
                  ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                }
              );

            foundEntites.Add(spatialEntity);
          }
        });
      }).Wait();

      return foundEntites;
    }

    private BsonDocument ConvertEntityToBson(ISpatialEntity entity) {
      var loc =
        new GeoJsonPoint<GeoJson2DGeographicCoordinates>(
          new GeoJson2DGeographicCoordinates(entity.Shape.Position.Z, entity.Shape.Position.X));

      var doc = new BsonDocument {
        {"_id", new BsonBinaryData(entity.AgentGuid)},
        {"loc", loc.ToBsonDocument()},
        {"AgentType", entity.AgentType.AssemblyQualifiedName},
        {"AgentGuid", entity.AgentGuid.ToString()},
        {"CollisionType", entity.CollisionType.ToString()}
      };

      if (_distributed)
        doc.Add("SpatialEntity",
          new BsonString(JsonConvert.SerializeObject(entity, Formatting.Indented,
            new JsonSerializerSettings {ReferenceLoopHandling = ReferenceLoopHandling.Ignore})));

      return doc;
    }

    #endregion
  }
}