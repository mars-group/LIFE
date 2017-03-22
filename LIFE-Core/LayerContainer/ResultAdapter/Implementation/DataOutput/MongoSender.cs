using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LIFE.API.Results;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ResultAdapter.Implementation.DataOutput {

  /// <summary>
  ///   A MongoDB adapter to save the simulation results to a database.
  /// </summary>
  internal class MongoSender {

    private readonly string _simId;                       // The simulation identifier (name).
    private readonly IMongoDatabase _dbLegacy;            // Legacy database.
    private readonly IMongoDatabase _dbSimRuns;           // Simulation meta information database.
    private readonly IMongoDatabase _dbResults;           // Database for new result output.
    private IMongoCollection<AgentSimResult> _colLegacy;  // ISimResult output collection.
    private IMongoCollection<BsonDocument> _colKeyframes; // Key frame collection.
    private IMongoCollection<BsonDocument> _colMetadata;  // Meta data entries.


    /// <summary>
    ///   Create the MongoDB adapter for data output.
    /// </summary>
    /// <param name="mongoDbHost">Address of the MongoDB to connect to..</param>
    /// <param name="simId">Simulation ID. Used as collection name.</param>
    public MongoSender(string mongoDbHost, string simId) {
      var client = new MongoClient("mongodb://"+mongoDbHost+":27017");
      _dbLegacy = client.GetDatabase("SimResults");
      _dbSimRuns = client.GetDatabase("SimulationRuns");
      _dbResults = client.GetDatabase("ResultData");
      _simId = simId;
    }


    /// <summary>
    ///   Create MongoDB indexes for AgentSimResult attributes.
    /// </summary>
    private async void CreateMongoDbIndexes() {
      var indexKeys = Builders<AgentSimResult>.IndexKeys.Ascending("Tick").Ascending("AgentType").Ascending("Layer");
      var indexOptions = new CreateIndexOptions { Background = true };
      await _colLegacy.Indexes.CreateOneAsync(indexKeys, indexOptions);
		  //var geoIndexKeys = Builders<AgentSimResult>.IndexKeys.Geo2DSphere("Position._v");
      //await _collection.Indexes.CreateOneAsync(geoIndexKeys);
    }


    /// <summary>
    ///   Write the result data to the MongoDB [legacy function].
    /// </summary>
    /// <param name="results">Agent result listings.</param>
    public void WriteLegacyResults(IEnumerable<AgentSimResult> results) {
      if (_colLegacy == null) {
        _colLegacy = _dbLegacy.GetCollection<AgentSimResult>(_simId);
        CreateMongoDbIndexes();
      }
      _colLegacy.InsertMany(results);
    }


    /// <summary>
    ///   Write keyframe result data to the MongoDB.
    /// </summary>
    /// <param name="results">A number of keyframes (JSON strings).</param>
    public void WriteKeyframes(IEnumerable<string> results) {
      if (_colKeyframes == null) {
        _colKeyframes = _dbResults.GetCollection<BsonDocument>(_simId+"-kf");
      }
      var documents = new ConcurrentBag<BsonDocument>();
      Parallel.ForEach(results, result => {
        documents.Add(BsonSerializer.Deserialize<BsonDocument>(result));
      });
      _colKeyframes.InsertMany(documents);
    }


    /// <summary>
    ///   Add agent meta data entries.
    /// </summary>
    /// <param name="metadata">Set of meta data information to write.</param>
    public void AddMetaData(IEnumerable<string> metadata) {
      if (_colMetadata == null) {
        _colMetadata = _dbResults.GetCollection<BsonDocument>(_simId+"-meta");
      }
      var documents = new ConcurrentBag<BsonDocument>();
      Parallel.ForEach(metadata, result => {
        documents.Add(BsonSerializer.Deserialize<BsonDocument>(result));
      });
      _colMetadata.InsertMany(documents);
    }


    /// <summary>
    ///   Update agent meta data entries.
    /// </summary>
    /// <param name="agentIds">List of agents that were deleted.</param>
    /// <param name="delTick">Tick of deletion.</param>
    public void UpdateMetaData(IEnumerable<string> agentIds, int delTick) {
      foreach (var agent in agentIds) {
        var filter = Builders<BsonDocument>.Filter.Eq(s => s["AgentId"], agent);
        var update = Builders<BsonDocument>.Update.Set(s => s["DeletionTick"], delTick);
        //_colMetadata.UpdateOne(filter, update);
      }
    }
  }
}
