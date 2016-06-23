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
using System.Threading.Tasks;
using CommonTypes;
using ConfigService;
using LifeAPI.Results;
using ResultAdapter.Implementation.DataOutput;
using ResultAdapter.Interface;

namespace ResultAdapter.Implementation {
  internal class ResultAdapterUseCase : IResultAdapter {


    /// <summary>
    ///   The Simulation ID. It will be set before the first call to WriteResults().
    /// </summary>
    /// <value>The simulation identifier.</value>    
    public Guid SimulationId { get; set; }

    private readonly ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>> _simObjects; // List of all objects to output.
    private MongoSender _sender;                                         // Database connector.


    /// <summary>
    ///   Instantiate the concrete result adapter.
    /// </summary>
    public ResultAdapterUseCase() {
      _simObjects = new ConcurrentDictionary<int, ConcurrentDictionary<ISimResult, byte>>();
    }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    public void WriteResults(int currentTick) {
	    if (_simObjects.IsEmpty) return;
	  

      // Deferred init of the connectors. Reason: MongoDB uses the SimID as collection.
      if (_sender == null) {
		var cfgClient = new ConfigServiceClient(MARSConfigServiceSettings.Address);
        _sender = new MongoSender(cfgClient, SimulationId.ToString());
        _sender.CreateMongoDbIndexes();
      }

      // Loop in parallel over all simulation elements to output.
      var results = new ConcurrentBag<AgentSimResult>();
      Parallel.ForEach(_simObjects.Keys, executionGroup => {
          if (executionGroup % currentTick == 0)
          {
              Parallel.ForEach(_simObjects[executionGroup].Keys, simResult => results.Add(simResult.GetResultData()));
          }
      });

      // MongoDB bulk insert of the output strings and RMQ notification, then clean up.
	    _sender.SendVisualizationData(results, currentTick);
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
    public void DeRegister(ISimResult simObject, int executionGroup = 1) {
        if (_simObjects.ContainsKey(executionGroup))
        {
            byte b;
            _simObjects[executionGroup].TryRemove(simObject, out b);
        }
    }
  }
}