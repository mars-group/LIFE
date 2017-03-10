//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommonTypes;
using ConfigService;
using ResultAdapter.Implementation.DataOutput;
using ResultAdapter.Interface;
using System.Linq;
using System.Net;
using System.Net.Http;
using LIFE.API.Results;
using Newtonsoft.Json.Linq;

namespace ResultAdapter.Implementation
{
    internal class ResultAdapterUseCase : IResultAdapter
    {
        /// <summary>
        ///   The Simulation ID. It will be set before the first call to WriteResults().
        /// </summary>
        /// <value>The simulation identifier.</value>
        public Guid SimulationId { get; set; }

        private readonly JObject _resultConfig;

        private readonly ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>> _simObjects
            ; // List of all objects to output.

        private readonly List<MongoSender> _senders; // Database connector.


        /// <summary>
        ///   Instantiate the concrete result adapter.
        /// </summary>
        public ResultAdapterUseCase(string resultConfigId)
        {
            _resultConfig = GetResultConfig(resultConfigId);
            _simObjects = new ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>>();
            _senders = new List<MongoSender>();
        }


        /// <summary>
        ///   Fetch all tick results and write them to the database.
        /// </summary>
        /// <param name="currentTick">The current tick. Needed for sanity check.</param>
        public void WriteResults(int currentTick)
        {
            if (_simObjects.IsEmpty) return;


            // Deferred init of the connectors. Reason: MongoDB uses the SimID as collection.
            if (!_senders.Any())
            {
                //for (var i = 0; i < 4; i++) {
                var cfgClient = new ConfigServiceClient(MARSConfigServiceSettings.Address);
                var sender = new MongoSender(cfgClient, SimulationId.ToString());
                sender.CreateMongoDbIndexes();
                _senders.Add(sender);
                //}

                // _notifier = new RabbitNotifier(cfgClient);
            }

            // Loop in parallel over all simulation elements to output.
            var results = new ConcurrentBag<AgentSimResult>();
            Parallel.ForEach(_simObjects.Keys, executionGroup =>
            {
                if (currentTick == 0 || currentTick == 1 || currentTick % executionGroup == 0)
                {
                    Parallel.ForEach(_simObjects[executionGroup].Keys,
                        simResult => results.Add(simResult.GetResultData()));
                }
            });

            // don't do shit, when results are empty
            if (results.IsEmpty)
            {
                results = null;
                return;
            }

            // MongoDB bulk insert of the output strings and RMQ notification, then clean up.
            //var lists = SplitList(results.ToList(), results.Count / _senders.Count);
            //Parallel.For(0, _senders.Count, i => _senders[i].SendVisualizationData(lists[i], currentTick));
            _senders.First().SendVisualizationData(results, currentTick);
            results = null;
        }



        /// <summary>
        ///   Register a simulation object at the result adapter.
        /// </summary>
        /// <param name="simObject">The simulation entity to add to output queue.</param>
        /// <param name="executionGroup"></param>
        public void Register(ISimResult simObject, int executionGroup = 1)
        {
            _simObjects.GetOrAdd(executionGroup, new ConcurrentDictionary<ISimResult, byte>());
            _simObjects[executionGroup].TryAdd(simObject, new byte());
        }

        /// <summary>
        ///   Deregisters a simulation object from the result adapter.
        /// </summary>
        /// <param name="simObject">The simulation entity to remove.</param>
        public void DeRegister(ISimResult simObject, int executionGroup = 1)
        {
            if (_simObjects.ContainsKey(executionGroup))
            {
                byte b;
                _simObjects[executionGroup].TryRemove(simObject, out b);
            }
        }


        #region PrivateMethods


        private static JObject GetResultConfig(string resultConfigId)
        {
            var resultServiceHost = "result-cfg-svc";


            var http = new HttpClient();
            Console.WriteLine("...downloading ResultConfig...");
            var uri = new Uri($"http://{resultServiceHost}/api/ResultConfigs/{resultConfigId}");
            var getTask = http.GetAsync(uri);
            getTask.Wait();
            if (getTask.Result.StatusCode != HttpStatusCode.OK)
            {
                // handle error
                return null; // for now
            }
            var readAsString = getTask.Result.Content.ReadAsStringAsync();
            readAsString.Wait();


            return JObject.Parse(readAsString.Result);
        }

        private static List<List<AgentSimResult>> SplitList(List<AgentSimResult> locations, int nSize = 30)
        {
            var list = new List<List<AgentSimResult>>();
            if (nSize == 0) nSize = 1;
            for (var i = 0; i < locations.Count; i += nSize)
            {
                list.Add(locations.GetRange(i, Math.Min(nSize, locations.Count - i)));
            }

            return list;
        }

        #endregion
    }
}