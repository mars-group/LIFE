using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using LIFE.API.Results;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace ResultAdapter.Implementation.DataOutput
{
    /// <summary>
    ///   A MongoDB adapter to save the simulation results to a database.
    /// </summary>
    internal class MongoDbWriter : IResultWriter
    {
        private readonly string _simId; // The simulation identifier (name).
        private readonly IMongoDatabase _dbLegacy; // Legacy database.
        private readonly IMongoDatabase _dbSimRuns; // Simulation meta information database.
        private readonly IMongoDatabase _dbResults; // Database for new result output.
        private IMongoCollection<AgentSimResult> _colLegacy; // ISimResult output collection.
        private IMongoCollection<BsonDocument> _colKeyframes; // Key frame collection.
        private IMongoCollection<BsonDocument> _colDeltaframes; // Delta frame collection.
        private IMongoCollection<BsonDocument> _colMetadata; // Meta data entries.
        private readonly JsonSerializerSettings _jsonConf; // Serialization settings.
        private bool _useGeoSpatialIndex; // Determine if Geo2D index should be used

        /// <summary>
        ///   Create the MongoDB adapter for data output.
        /// </summary>
        /// <param name="mongoDbHost">Address of the MongoDB to connect to..</param>
        /// <param name="simId">Simulation ID. Used as collection name.</param>
        public MongoDbWriter(string mongoDbHost, string simId,
                             IEnumerable<LoggerConfig> loggerConfigs)
        {
            var client = new MongoClient("mongodb://" + mongoDbHost + ":27017");
            _dbLegacy = client.GetDatabase("SimResults");
            _dbSimRuns = client.GetDatabase("SimulationRuns");
            _dbResults = client.GetDatabase("ResultData");
            _simId = simId;
            _jsonConf = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
            // all valid geo agents? then use geospatial index!
            if (loggerConfigs != null || loggerConfigs.Any())
            {
                foreach (var loggerConf in loggerConfigs) {
                    _useGeoSpatialIndex = _useGeoSpatialIndex && (loggerConf.SpatialType == "GPS");
                }
            }
        }

        /// <summary>
        ///   Create MongoDB indexes for AgentSimResult attributes.
        /// </summary>
        private async void CreateMongoDbIndexes()
        {
            var indexKeys = Builders<AgentSimResult>.IndexKeys.Ascending("Tick")
                .Ascending("AgentType")
                .Ascending("Layer");
            var indexOptions = new CreateIndexOptions {Background = true};
            await _colLegacy.Indexes.CreateOneAsync(indexKeys, indexOptions);
            if (_useGeoSpatialIndex)
            {
                var geoIndexKeys = Builders<AgentSimResult>.IndexKeys.Geo2DSphere("Position._v");
                await _colLegacy.Indexes.CreateOneAsync(geoIndexKeys);
            }
        }


        /// <summary>
        ///   Write the result data to the MongoDB [legacy function].
        /// </summary>
        /// <param name="results">Agent result listings.</param>
        public void WriteLegacyResults(IEnumerable<AgentSimResult> results)
        {
            if (_colLegacy == null)
            {
                _colLegacy = _dbLegacy.GetCollection<AgentSimResult>(_simId);
                CreateMongoDbIndexes();
            }
            _colLegacy.InsertMany(results);
        }


        /// <summary>
        ///   Add agent meta data entries.
        /// </summary>
        /// <param name="metadata">Set of meta data information to write.</param>
        public void AddMetadataEntries(IEnumerable<AgentMetadataEntry> metadata)
        {
            if (_colMetadata == null)
            {
                _colMetadata = _dbResults.GetCollection<BsonDocument>(_simId + "-meta");
            }
            var documents = new ConcurrentBag<BsonDocument>();
            Parallel.ForEach(metadata, result =>
            {
                var json = JsonConvert.SerializeObject(result, _jsonConf);
                documents.Add(BsonSerializer.Deserialize<BsonDocument>(json));
            });
            _colMetadata.InsertMany(documents);
        }


        /// <summary>
        ///   Mark agents as removed by setting their meta data deletion flag.
        /// </summary>
        /// <param name="agentIds">List of agents that were deleted.</param>
        /// <param name="delTick">Tick of deletion.</param>
        public void SetAgentDeletionFlags(IEnumerable<string> agentIds, int delTick)
        {
            foreach (var agent in agentIds)
            {
                var filter = Builders<BsonDocument>.Filter.Eq(s => s["AgentId"], agent);
                var update = Builders<BsonDocument>.Update.Set(s => s["DeletionTick"], delTick);
                //_colMetadata.UpdateOne(filter, update);
            }
        }


        /// <summary>
        ///   Write agent result data to the storage.
        /// </summary>
        /// <param name="results">A list of agent frames.</param>
        /// <param name="isKeyframe">Set to 'true' on keyframes, 'false' on delta frames.</param>
        public void WriteAgentFrames(IEnumerable<AgentFrame> results, bool isKeyframe)
        {
            var swParallel = Stopwatch.StartNew();
            IMongoCollection<BsonDocument> col;
            if (isKeyframe)
            {
                if (_colKeyframes == null)
                {
                    _colKeyframes = _dbResults.GetCollection<BsonDocument>(_simId + "-kf");
                }
                col = _colKeyframes;
            }
            else
            {
                if (_colDeltaframes == null)
                {
                    _colDeltaframes = _dbResults.GetCollection<BsonDocument>(_simId + "-df");
                }
                col = _colDeltaframes;
            }

            var documents = new ConcurrentBag<BsonDocument>();
            Parallel.ForEach(results, result =>
            {
                var json = JsonConvert.SerializeObject(result, _jsonConf);
                /*var json = JsonConvert.SerializeObject(new FrameComp {
                  K = result.IsKeyframe,
                  I = result.AgentId,
                  T = (int) result.Tick,
                  P = result.Position,
                  O = result.Orientation,
                  V = result.Properties
                }, _jsonConf);*/
                documents.Add(BsonSerializer.Deserialize<BsonDocument>(json));
            });

            // Insert the documents.
            col.InsertMany(documents);
            Console.WriteLine("MongoDB finished writting results in " + swParallel.ElapsedMilliseconds + " ms");
        }

/*
        private class FrameComp
        {
            public bool K;
            public string I;
            public int T;
            public object[] P, O;
            public IDictionary<string, object> V;
        }
*/
    }
}