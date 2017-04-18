using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using ConfigService;
using LIFE.API.Agent;
using LIFE.API.Results;
using ResultAdapter.Implementation.DataOutput;
using ResultAdapter.Interface;

// ReSharper disable SuspiciousTypeConversion.Global

[assembly: InternalsVisibleTo("ResultAdapterTests")]

namespace ResultAdapter.Implementation
{
    /// <summary>
    ///   Implementation of the result adapter.
    /// </summary>
    internal class ResultAdapterUseCase : IResultAdapter
    {
        private IResultWriter _writer; // Database connector.
        private readonly string _mongoDbHost; // MongoDB host address.
        private readonly RabbitNotifier _notifier; // Listener queue notifier.
        private readonly LoggerGenerator _generator; // Result logger generator.
        private readonly ConcurrentDictionary<int, LoggerGroup> _loggers; // Logger listing (key=frequency).
        private ConcurrentBag<IGeneratedLogger> _newAgents; // Meta entries of new agents.
        private ConcurrentBag<string> _deletedAgents; // Agents removed in last tick.
        private readonly bool _verboseOutput; // Is verbose output desired?

        /// <summary>
        ///   The Simulation ID. It will be set before the first call to WriteResults().
        /// </summary>
        /// <value>The simulation identifier.</value>
        public Guid SimulationId { get; set; }


        /// <summary>
        ///   Instantiate the concrete result adapter.
        /// </summary>
        public ResultAdapterUseCase(string resultConfigId, bool enableTestMode = false)
        {
            _loggers = new ConcurrentDictionary<int, LoggerGroup>();
            _newAgents = new ConcurrentBag<IGeneratedLogger>();
            _deletedAgents = new ConcurrentBag<string>();
            _mongoDbHost = enableTestMode ? "127.0.0.1" : "result-mongodb";
            var rcsHost = enableTestMode ? "127.0.0.1:8080" : "resultcfg-svc";
            var configHost = enableTestMode ? "127.0.0.1:8080" : "config-svc";
            var rabbitHost = enableTestMode ? "127.0.0.1" : "rabbitmq";
            _generator = new LoggerGenerator(rcsHost, resultConfigId);
            var cfgClient = new ConfigServiceClient("http://" + configHost);
            _notifier = new RabbitNotifier(rabbitHost, cfgClient);
            _verboseOutput = false;
        }


        /// <summary>
        ///   Fetch all tick results and write them to the database.
        /// </summary>
        /// <param name="currentTick">The current tick. Needed for sanity check.</param>
        public void WriteResults(int currentTick)
        {
            // Initialization in the first tick. It is deferred, because the
            // simulation identifier is not available in the constructor.
            if (currentTick == 0 && _writer == null)
            {
                if (_generator.OutputTarget == string.Empty ||
                    _generator.OutputTarget.ToUpper().Equals("MONGODB"))
                {
                    _writer = new MongoDbWriter(_mongoDbHost, SimulationId.ToString(),
                        _generator.LoggerDefinitions);
                    Console.WriteLine("[ResultAdapter] Initialized MongoDB adapter to '" + _mongoDbHost + "'.");
                }
                else if (_generator.OutputTarget.ToUpper().Equals("CASSANDRA"))
                {
                    _generator.OutputParams["SimulationId"] = SimulationId.ToString();
                    _writer = new CassandraWriter(_generator.OutputParams,
                                                  _generator.LoggerDefinitions);
                    Console.WriteLine("[ResultAdapter] Initialized Cassandra adapter.");
                }
                else
                {
                    Console.Error.WriteLine("[ResultAdapter] Error: Unable to initialize database " +
                                            "connector '" + _generator.OutputTarget + "'.");
                }
                Console.WriteLine(ToString(true));
            }
            if (_writer == null) return;


            // Write the metadata entries of the new agents.
            if (_newAgents.Count > 0)
            {
                if (_verboseOutput)
                    Console.Write("[ResultAdapter] Adding " + _newAgents.Count +
                                  (_newAgents.Count > 1 ? " entries" : " entry") + ": ");
                var metadataList = new ConcurrentBag<AgentMetadataEntry>();
                Parallel.ForEach(_newAgents, newAgent =>
                {
                    var entry = newAgent.GetMetatableEntry();
                    metadataList.Add(entry);
                });
                _newAgents = new ConcurrentBag<IGeneratedLogger>();
                _writer.AddMetadataEntries(metadataList);
                if (_verboseOutput) Console.WriteLine("[done]");
            }


            // Set the deletion flags for unregistered agents.
            if (_deletedAgents.Count > 0)
            {
                if (_verboseOutput) Console.Write("[ResultAdapter] Deleting " + _deletedAgents.Count + " old agents: ");
                _writer.SetAgentDeletionFlags(_deletedAgents, currentTick - 1);
                _deletedAgents = new ConcurrentBag<string>();
                if (_verboseOutput) Console.WriteLine("[done]");
            }


            // Lists for the storage of the result data.
            var oldResults = new ConcurrentBag<AgentSimResult>();
            var keyframes = new ConcurrentBag<AgentFrame>();

            foreach (var outputGroup in _loggers.Keys)
            {
                //| Loop over all logger groups and
                if (currentTick % outputGroup == 0)
                {
                    //| check if output is necessary.
                    if (_verboseOutput)
                        Console.WriteLine("[ResultAdapter] Aggregating for exec. group [" + outputGroup + "]: ");

                    var oldLoggers = _loggers[outputGroup].OldLoggers.Keys;
                    var newLoggers = _loggers[outputGroup].GenLoggers.Values;
                    if (oldLoggers.Count > 0)
                    {
                        if (_verboseOutput) Console.Write(" - " + oldLoggers.Count + " legacy: ");
                        Parallel.ForEach(oldLoggers, logger => oldResults.Add(logger.GetResultData()));
                        if (_verboseOutput) Console.WriteLine("[done]");
                    }
                    if (newLoggers.Count > 0)
                    {
                        if (_verboseOutput) Console.Write(" - " + newLoggers.Count + " generated: ");
                        Parallel.ForEach(newLoggers, logger => keyframes.Add(logger.GetKeyFrame()));
                        if (_verboseOutput) Console.WriteLine("[done]");
                    }
                }
            }

            // Database bulk insert of the output packets and RMQ notification.
            if (_verboseOutput) Console.WriteLine("[ResultAdapter] Writing to database: ");
            if (!oldResults.IsEmpty)
            {
                if (_verboseOutput) Console.Write(" - " + oldResults.Count + " legacy (ISimResult): ");
                _writer.WriteLegacyResults(oldResults);
                if (_verboseOutput) Console.WriteLine("[done]");
            }
            if (!keyframes.IsEmpty)
            {
                if (_verboseOutput) Console.Write(" - " + keyframes.Count + " keyframes (JSON): ");
                _writer.WriteAgentFrames(keyframes, true);
                if (_verboseOutput) Console.WriteLine("[done]");
            }
            _notifier.AnnounceNewTick(SimulationId.ToString(), currentTick);
        }


        /// <summary>
        ///   Register a simulation object at the result adapter.
        /// </summary>
        /// <param name="simObject">The simulation entity to add to output queue.</param>
        /// <param name="execGrp">Agent execution (and output) group.</param>
        public void Register(ITickClient simObject, int execGrp = 1)
        {
            if (!_loggers.ContainsKey(execGrp))
            {
                _loggers.TryAdd(execGrp, new LoggerGroup());
            }
            if (simObject.GetType().IsAssignableFrom(typeof(ISimResult)))
            {
                // If ISimResults is being used -> attempt legacy output.
                var simResult = simObject as ISimResult;
                if (simResult != null)
                {
                    _loggers[execGrp].OldLoggers.TryAdd(simResult, new byte());
                }
            }
            else
            {
                var logger = _generator.GetResultLogger(simObject);     //| If there is a logger
                if (logger == null) return;                             //| specification for the
                _loggers[execGrp].GenLoggers.TryAdd(simObject, logger); //| current type, use it!
                _newAgents.Add(logger);
            }
        }


        /// <summary>
        ///   De-registers a simulation object from the result adapter.
        /// </summary>
        /// <param name="simObject">The simulation entity to remove.</param>
        /// <param name="execGrp">Agent execution (and output) group.</param>
        public void DeRegister(ITickClient simObject, int execGrp = 1)
        {
            if (_loggers.ContainsKey(execGrp))
            {
                if (_generator.HasLoggerDefinition(simObject))
                {
                    var genLoggers = _loggers[execGrp].GenLoggers;
                    if (genLoggers.ContainsKey(simObject))
                    {
                        IGeneratedLogger logger;
                        var success = genLoggers.TryRemove(simObject, out logger);
                        if (success) _deletedAgents.Add(((IAgent) simObject).ID.ToString());
                    }
                }
                else
                {
                    var simResult = simObject as ISimResult;
                    if (simResult != null)
                    {
                        var oldLoggers = _loggers[execGrp].OldLoggers;
                        if (oldLoggers.ContainsKey(simResult))
                        {
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
        public string ToString(bool rec)
        {
            var str = "[ResultAdapter] \n" +
                      " - Execution groups: \n";
            foreach (var execGrp in _loggers.Keys)
            {
                var gen = _loggers[execGrp].GenLoggers.Count;
                var leg = _loggers[execGrp].OldLoggers.Count;
                str += "    [" + execGrp + "] generated: " + gen + ", legacy: " + leg + "\n";
            }

            if (rec)
            {
                str += _generator.ToString(false);
            }
            return str;
        }
    }


    /// <summary>
    ///   The logger group groups old and new loggers for an execution group.
    /// </summary>
    internal class LoggerGroup
    {
        public readonly ConcurrentDictionary<ISimResult, byte> OldLoggers; // Direct references (legacy).
        public readonly ConcurrentDictionary<ITickClient, IGeneratedLogger> GenLoggers; // Generated loggers.


        /// <summary>
        ///   Create a new logger group. This initializes the logger lists.
        /// </summary>
        public LoggerGroup()
        {
            OldLoggers = new ConcurrentDictionary<ISimResult, byte>();
            GenLoggers = new ConcurrentDictionary<ITickClient, IGeneratedLogger>();
        }
    }
}