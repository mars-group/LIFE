using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using ConfigService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace ResultAdapter.Implementation.DataOutput {

  /// <summary>
  ///   A MongoDB adapter to save the simulation results to a database.
  /// </summary>
  internal class MongoSender {

    private readonly IMongoDatabase _database; // Database to write output to.


    /// <summary>
    ///   Create the MongoDB adapter for data output.
    /// </summary>
    /// <param name="cfgClient">MARS KV client for connection properties.</param>
    public MongoSender(IConfigServiceClient cfgClient) {
      var ip = cfgClient.Get("mongodb/ip");
      var port = cfgClient.Get("mongodb/port");
      var client = new MongoClient("mongodb://"+ip+":"+port);
      _database = client.GetDatabase("test");
    }


    /// <summary>
    ///   Write the agent data to the MongoDB.
    /// </summary>
    /// <param name="json">Listing of strings with the agent's parameters.</param>
    /// <param name="simId">The simulation the data belong to.</param>
    public void SendVisualizationData(ConcurrentBag<string> json, string simId) {

      //TODO tbc ...
      // Under construction ...

      var docs = new ConcurrentBag<BsonDocument>();
      Parallel.ForEach(json, s => docs.Add(BsonSerializer.Deserialize<BsonDocument>(s)));

      var collection = _database.GetCollection<BsonDocument>(simId);
      Console.WriteLine("[MongoSender]: Writing "+json.Count+" packets for simulation '"+simId+"'");
      collection.InsertManyAsync(docs);
    }
  }
}