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
	  private readonly RabbitNotifier _notifier;                     // Listener queue notifier.
	  private readonly string _simId;                                // Sim-ID, used for announcement.


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
	    _notifier = new RabbitNotifier(cfgClient);  
	    _simId = simId;
    }


    /// <summary>
    ///   Write the agent data to the MongoDB.
    /// </summary>
    /// <param name="results">A number of result elements to be written.</param>
	/// <param name = "currentTick">The current tick of the simulation.</param>
    public void SendVisualizationData(ConcurrentBag<AgentSimResult> results, int currentTick) {
			_collection.InsertMany (results);
			_notifier.AnnounceNewPackage(_simId, currentTick);
			/*_collection.InsertManyAsync(results)
				.ContinueWith(t => {
					if(t.IsFaulted){
						throw t.Exception;
					}

					if(t.IsCanceled){
						throw new Exception("MongoDB Sender Task got cancelled!");
					}

					if(t.IsCompleted)
					{
						_notifier.AnnounceNewPackage(_simId, currentTick);
					}
				});
			*/
    }
  }
}