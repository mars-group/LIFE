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

    private readonly IMongoCollection<AgentSimResult> _colLegacy;  // Legacy output collection.
    private readonly IMongoCollection<BsonDocument> _colKeyframes; // Key frame collection.
    private readonly IMongoCollection<BsonDocument> _colMetadata;  // Meta data entries.


    /// <summary>
    ///   Create the MongoDB adapter for data output.
    /// </summary>
    /// <param name="mongoDbHost">Address of the MongoDB to connect to..</param>
    /// <param name="simId">Simulation ID. Used as collection name.</param>
    public MongoSender(string mongoDbHost, string simId) {
      var client = new MongoClient("mongodb://"+mongoDbHost+":27017");
      var databaseLegacy = client.GetDatabase("SimResults");
      var databaseSimRuns = client.GetDatabase("SimulationRuns");
      var databaseResults = client.GetDatabase("ResultData");
      _colLegacy = databaseLegacy.GetCollection<AgentSimResult>(simId);
      _colKeyframes = databaseResults.GetCollection<BsonDocument>(simId+"-kf");
      _colMetadata = databaseResults.GetCollection<BsonDocument>(simId+"-meta");
    }


    /// <summary>
    ///   Create MongoDB indexes for AgentSimResult attributes.
    /// </summary>
    public async void CreateMongoDbIndexes() {
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
      _colLegacy.InsertMany(results);
    }


    /// <summary>
    ///   Write keyframe result data to the MongoDB.
    /// </summary>
    /// <param name="results">A number of keyframes (JSON strings).</param>
    public void WriteKeyframes(IEnumerable<string> results) {
      var documents = new ConcurrentBag<BsonDocument>();
      Parallel.ForEach(results, result => {
        documents.Add(BsonSerializer.Deserialize<BsonDocument>(result));
      });
      _colKeyframes.InsertMany(documents);
    }
  }
}
