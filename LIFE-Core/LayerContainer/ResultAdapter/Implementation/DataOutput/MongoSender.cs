using System.Collections.Generic;
using ConfigService;
using LIFE.API.Results;
using MongoDB.Driver;

namespace ResultAdapter.Implementation.DataOutput {

  /// <summary>
  ///   A MongoDB adapter to save the simulation results to a database.
  /// </summary>
  internal class MongoSender {

    private readonly IMongoCollection<AgentSimResult> _collection; // Collection for output.
	private readonly string _simId;                                // Sim-ID, used for announcement.


    /// <summary>
    ///   Create the MongoDB adapter for data output.
    /// </summary>
    /// <param name="cfgClient">MARS KV client for connection properties.</param>
    /// <param name="simId">Simulation ID. Used as collection name.</param>
    public MongoSender(IConfigServiceClient cfgClient, string simId) {
      var ip = "result-mongodb";
      var port = "27017";
      var client = new MongoClient("mongodb://"+ip+":"+port);
      var database = client.GetDatabase("SimResults");
      _collection = database.GetCollection<AgentSimResult>(simId);
	  _simId = simId;
    }

    /// <summary>
    ///   Create MongoDB indexes for AgentSimResult attributes.
    /// </summary>
    /// <todo>
    ///   Create indexes for selected attributes from the SimConfig. 
    /// </todo>
    public async void CreateMongoDbIndexes()
    {
        var indexKeys = Builders<AgentSimResult>.IndexKeys.Ascending("Tick").Ascending("AgentType").Ascending("Layer");
		//var geoIndexKeys = Builders<AgentSimResult>.IndexKeys.Geo2DSphere("Position._v");
        CreateIndexOptions indexOptions = new CreateIndexOptions { Background = true };
		//await _collection.Indexes.CreateOneAsync(geoIndexKeys);
        await _collection.Indexes.CreateOneAsync(indexKeys, indexOptions);
    }

    /// <summary>
    ///   Write the agent data to the MongoDB.
    /// </summary>
    /// <param name="results">A number of result elements to be written.</param>
	/// <param name = "currentTick">The current tick of the simulation.</param>
    public void SendVisualizationData(IEnumerable<AgentSimResult> results, int currentTick) {
            _collection.InsertMany (results);
    }
  }
}