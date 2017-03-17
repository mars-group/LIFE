using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CommonTypes;
using ConfigService;
using LIFE.API.Agent;
using LIFE.API.Results;
using Newtonsoft.Json.Linq;
using ResultAdapter.Implementation.DataOutput;
using ResultAdapter.Interface;

namespace ResultAdapter.Implementation {

  /// <summary>
  ///   Implementation of the result adapter.
  /// </summary>
  internal class ResultAdapterUseCase : IResultAdapter {


    private MongoSender _sender;        // Database connector.
    private readonly RabbitNotifier _notifier;   // Listener queue notifier.
    private readonly LoggerGenerator _generator; // Result logger generator.


    private readonly ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>> _simObjects; // List of all objects to output.
    private readonly ConcurrentDictionary<ITickClient, ResultLogger> _loggers;


    /// <summary>
    ///   The Simulation ID. It will be set before the first call to WriteResults().
    /// </summary>
    /// <value>The simulation identifier.</value>
    public Guid SimulationId { get; set; }


    /// <summary>
    ///   Instantiate the concrete result adapter.
    /// </summary>
    public ResultAdapterUseCase(string resultConfigId) {
      _simObjects = new ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>>();
      _loggers = new ConcurrentDictionary<ITickClient, ResultLogger>();
      _generator = new LoggerGenerator(resultConfigId);
    }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    public void WriteResults(int currentTick) {

      // Initialization in the first tick. It is deferred, because the simulation identifier
      // is not available in the constructor.
      if (_sender == null) {
        var cfgClient = new ConfigServiceClient(MARSConfigServiceSettings.Address);
        _sender = new MongoSender(cfgClient, SimulationId.ToString());
        _sender.CreateMongoDbIndexes();
      }


      if (_simObjects.IsEmpty) return;

      // Loop in parallel over all simulation elements to output.
      var results = new ConcurrentBag<AgentSimResult>();
      Parallel.ForEach(_simObjects.Keys, executionGroup => {
        if ((currentTick == 0) || (currentTick == 1) || (currentTick%executionGroup == 0))
          Parallel.ForEach(_simObjects[executionGroup].Keys,
            simResult => results.Add(simResult.GetResultData()));
      });

      // don't do shit, when results are empty
      if (results.IsEmpty) return;
      
      // Create result lists (to perform a parallel insert). 
      var resultList = results.ToList();
      var lists = new List<List<AgentSimResult>>();
      var nSize = results.Count/(Environment.ProcessorCount - 1);
      if (nSize == 0) nSize = 1;
      for (var i = 0; i < results.Count; i += nSize) {
        lists.Add(resultList.GetRange(i, Math.Min(nSize, resultList.Count - i)));
      }

      // MongoDB bulk insert of the output strings and RMQ notification, then clean up.
      Parallel.For(0, lists.Count, i => _sender.SendVisualizationData(lists[i], currentTick));
      //Console.WriteLine("--dbg: all results written ("+results.Count+").");
    }


    /// <summary>
    ///   Register a simulation object at the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    /// <param name="executionGroup">Agent execution (and output) group.</param>
    public void Register(ITickClient simObject, int executionGroup = 1) {


      if (_generator != null) {
        var logger = _generator.GetResultLogger(simObject);
        if (logger != null) {
          _loggers.TryAdd(simObject, logger);
        }
      }
      else { // Output the entity the legacy way.
        var simResult = simObject as ISimResult;
        if (simResult != null) { 
          _simObjects.GetOrAdd(executionGroup, new ConcurrentDictionary<ISimResult, byte>());
          _simObjects[executionGroup].TryAdd(simResult, new byte());        
        }
      }
    }



    /// <summary>
    ///   De-registers a simulation object from the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    /// <param name="executionGroup">Agent execution (and output) group.</param>
    public void DeRegister(ITickClient simObject, int executionGroup = 1) {
      if (simObject is ISimResult && _simObjects.ContainsKey(executionGroup)) {
        byte b;
        _simObjects[executionGroup].TryRemove((ISimResult) simObject, out b);
      }
      else {
        ResultLogger logger;
        _loggers.TryRemove(simObject, out logger);        
      }
    }



    /// <summary>
    ///   Register a simulation object at the result adapter.
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    /// <param name="executionGroup"></param>
    public void Register(ISimResult simObject, int executionGroup = 1) {
      _simObjects.GetOrAdd(executionGroup, new ConcurrentDictionary<ISimResult, byte>());
      _simObjects[executionGroup].TryAdd(simObject, new byte());
    }

    /// <summary>
    ///   Deregisters a simulation object from the result adapter.
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    public void DeRegister(ISimResult simObject, int executionGroup = 1) {
      if (_simObjects.ContainsKey(executionGroup)) {
        byte b;
        _simObjects[executionGroup].TryRemove(simObject, out b);
      }
    }
  }



  //TODO
  class LoggerGenerator {

    public LoggerGenerator(string configId) {
      var json = GetConfiguration(configId);
      if (json != null) {
        //TODO hier weitermachen! 
      }
    }


    internal ResultLogger GetResultLogger(ITickClient tickClient) {
      return null;
    }


    private JObject GetConfiguration(string configId) {
      var http = new HttpClient();
      try {
        var getTask = http.GetAsync("http://resultcfg-svc/api/ResultConfigs/");
        getTask.Wait(4000);
        if (getTask.Result.StatusCode == HttpStatusCode.OK) {
          var readTask = getTask.Result.Content.ReadAsStringAsync();
          readTask.Wait();
          http.Dispose();
          return JObject.Parse(readTask.Result);
        }
      }
      catch (Exception ex) {
        Console.Error.WriteLine("[LoggerGenerator] Failed to read configuration '"+configId+"'.");
        Console.Error.WriteLine("[LoggerGenerator] Exception: "+ex);
      }
      return null;
    }
  }


  class ResultLogger { }
}