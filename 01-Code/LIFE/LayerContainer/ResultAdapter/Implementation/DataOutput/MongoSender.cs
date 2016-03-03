using System.Collections.Concurrent;
using ConfigService;
using LifeAPI.Results;
using MongoDB.Driver;

namespace ResultAdapter.Implementation.DataOutput {

  /// <summary>
  ///   A MongoDB adapter to save the simulation results to a database.
  /// </summary>
  internal class MongoSender {

    private readonly IMongoCollection<AgentSimResult> _collection; // Collection for output.


    /// <summary>
    ///   Create the MongoDB adapter for data output.
    /// </summary>
    /// <param name="cfgClient">MARS KV client for connection properties.</param>
    /// <param name="simId">Simulation ID. Used as collection name.</param>
    public MongoSender(IConfigServiceClient cfgClient, string simId) {
      var ip = cfgClient.Get("mongodb/ip");
      var port = cfgClient.Get("mongodb/port");
      var client = new MongoClient("mongodb://"+ip+":"+port);
      var database = client.GetDatabase("SimResults");
      _collection = database.GetCollection<AgentSimResult>(simId);
    }


    /// <summary>
    ///   Write the agent data to the MongoDB.
    /// </summary>
    /// <param name="results">A number of result elements to be written.</param>
    public void SendVisualizationData(ConcurrentBag<AgentSimResult> results) {
      _collection.InsertManyAsync(results);
    }
  }
}