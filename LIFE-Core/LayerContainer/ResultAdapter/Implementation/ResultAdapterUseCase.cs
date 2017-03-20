using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConfigService;
using LIFE.API.Agent;
using LIFE.API.Results;
using ResultAdapter.Implementation.DataOutput;
using ResultAdapter.Interface;
[assembly: InternalsVisibleTo("ResultAdapterTests")]

namespace ResultAdapter.Implementation {

  /// <summary>
  ///   Implementation of the result adapter.
  /// </summary>
  internal class ResultAdapterUseCase : IResultAdapter {

    private MongoSender _sender;                            // Database connector.
    private readonly RabbitNotifier _notifier;              // Listener queue notifier.
    private readonly LoggerGenerator _generator;            // Result logger generator.
    private readonly Dictionary<int, LoggerGroup> _loggers; // Logger listing (key=frequency).
    private readonly string _mongoDbHost;                   // MongoDB host address.


    /// <summary>
    ///   The Simulation ID. It will be set before the first call to WriteResults().
    /// </summary>
    /// <value>The simulation identifier.</value>
    public Guid SimulationId { get; set; }


    /// <summary>
    ///   Instantiate the concrete result adapter.
    /// </summary>
    public ResultAdapterUseCase(string resultConfigId, bool enableTestMode = false) {
      _loggers = new Dictionary<int, LoggerGroup>();
      _mongoDbHost = enableTestMode ? "127.0.0.1" : "result-mongodb";
      var rcsHost = enableTestMode ? "127.0.0.1:8080" : "resultcfg-svc";
      var configHost = enableTestMode ? "127.0.0.1:8080" : "config-svc";
      var rabbitHost = enableTestMode ? "127.0.0.1" : "rabbitmq";
      _generator = new LoggerGenerator(rcsHost, resultConfigId);
      var cfgClient = new ConfigServiceClient("http://"+configHost);
      _notifier = new RabbitNotifier(rabbitHost, cfgClient);
    }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    public void WriteResults(int currentTick) {

      // Initialization in the first tick. It is deferred, because the
      // simulation identifier is not available in the constructor.
      if (_sender == null) {
        _sender = new MongoSender(_mongoDbHost, SimulationId.ToString());
        _sender.CreateMongoDbIndexes();
      }

      // Lists for the storage of the result data.
      var oldResults = new ConcurrentBag<AgentSimResult>();
      //TODO . . .

      foreach (var outputGroup in _loggers.Keys) { //| Loop over all logger groups and
        if (currentTick % outputGroup == 0) {      //| check if output is necessary.

          var oldLoggers = _loggers[outputGroup].OldLoggers.Keys;
          Console.WriteLine("[ResultAdapter] Parallel run over "+oldLoggers.Count+" legacy loggers.");
          Parallel.ForEach(oldLoggers, logger => {
            oldResults.Add(logger.GetResultData());
          });

          //TODO Ausgabelogik für die neuen Logger hier einfügen!
        }
      }

      // MongoDB bulk insert of the output packets and RMQ notification.
      if (!oldResults.IsEmpty) _sender.WriteLegacyResults(oldResults);
      _notifier.AnnounceNewPackage(SimulationId.ToString(), currentTick);
    }


    /// <summary>
    ///   Register a simulation object at the result adapter.
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    /// <param name="execGrp">Agent execution (and output) group.</param>
    public void Register(ITickClient simObject, int execGrp = 1) {
      if (!_loggers.ContainsKey(execGrp)) {
        _loggers.Add(execGrp, new LoggerGroup());
      }
      var logger = _generator.GetResultLogger(simObject);       //| If there is a logger
      if (logger != null) {                                     //| specification for the
        _loggers[execGrp].GenLoggers.TryAdd(simObject, logger); //| current type, use it!
      }
      else { // If no logger definition was found, attempt legacy output.
        var simResult = simObject as ISimResult;
        if (simResult != null) {
          _loggers[execGrp].OldLoggers.TryAdd(simResult, new byte());
        }
      }
    }


    /// <summary>
    ///   De-registers a simulation object from the result adapter.
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    /// <param name="execGrp">Agent execution (and output) group.</param>
    public void DeRegister(ITickClient simObject, int execGrp = 1) {
      if (_loggers.ContainsKey(execGrp)) {
        if (_generator.HasLoggerDefinition(simObject)) {
          var genLoggers = _loggers[execGrp].GenLoggers;
          if (genLoggers.ContainsKey(simObject)) {
            IGeneratedLogger logger;
            genLoggers.TryRemove(simObject, out logger);
          }
        }
        else {
          var simResult = simObject as ISimResult;
          if (simResult != null) {
            var oldLoggers = _loggers[execGrp].OldLoggers;
            if (oldLoggers.ContainsKey(simResult)) {
              byte b;
              oldLoggers.TryRemove(simResult, out b);
            }
          }
        }
      }
    }


    /// <summary>
    ///   ResultAdapter debug output. Prints all properties.
    /// </summary>
    /// <param name="rec">Recursive flag. If set, all subcomponents are also printed.</param>
    /// <returns>A formatted string with detailed property listings.</returns>
    public string ToString(bool rec) {
      var str = "[ResultAdapter] \n" +
                " - Execution groups: \n";
      foreach (var execGrp in _loggers.Keys) {
        var gen = _loggers[execGrp].GenLoggers.Count;
        var leg = _loggers[execGrp].OldLoggers.Count;
        str += "    ["+execGrp+"] generated: "+gen+", legacy: "+leg+"\n";
      }

      if (rec) {
        str += _generator.ToString();
      }
      return str;
    }
  }



  /// <summary>
  ///   The logger group groups old and new loggers for an execution group.
  /// </summary>
  internal class LoggerGroup {

    public readonly ConcurrentDictionary<ISimResult, byte> OldLoggers;              // Direct references (legacy).
    public readonly ConcurrentDictionary<ITickClient, IGeneratedLogger> GenLoggers; // Generated loggers.


    /// <summary>
    ///   Create a new looger group. This initializes the logger lists.
    /// </summary>
    public LoggerGroup() {
      OldLoggers = new ConcurrentDictionary<ISimResult, byte>();
      GenLoggers = new ConcurrentDictionary<ITickClient, IGeneratedLogger>();
    }
  }
}
