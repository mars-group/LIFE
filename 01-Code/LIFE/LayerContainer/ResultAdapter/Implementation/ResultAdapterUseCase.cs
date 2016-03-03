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

    private readonly ConcurrentDictionary<ISimResult, byte> _simObjects; // List of all objects to output.
    private MongoSender _sender;                                         // Database connector.
    private RabbitNotifier _notifier;                                    // Listener queue notifier.


    /// <summary>
    ///   Instantiate the concrete result adapter.
    /// </summary>
    public ResultAdapterUseCase() {
      _simObjects = new ConcurrentDictionary<ISimResult, byte>();
    }


    /// <summary>
    ///   Fetch all tick results and write them to the database.
    /// </summary>
    /// <param name="currentTick">The current tick. Needed for sanity check.</param>
    public void WriteResults(int currentTick) {

      // Deferred init of the connectors. Reason: MongoDB uses the SimID as collection.
      if (_sender == null) {
        var cfgClient = new ConfigServiceClient("http://marsconfig:8080/");
        _sender = new MongoSender(cfgClient, SimulationId.ToString());
        _notifier = new RabbitNotifier(cfgClient);        
      }

      // Loop in parallel over all simulation elements to output.
      var results = new ConcurrentBag<AgentSimResult>();
      Parallel.ForEach(_simObjects.Keys, entity => {
        results.Add(entity.GetResultData());
      });

      // MongoDB bulk insert of the output strings and RMQ notification.
      _sender.SendVisualizationData(results);
      _notifier.AnnounceNewPackage(SimulationId.ToString(), currentTick);
    }


    /// <summary>
    ///   Register a simulation object at the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to add to output queue.</param>
    public void Register(ISimResult simObject) {
      _simObjects.TryAdd(simObject, new byte());
    }


    /// <summary>
    ///   Deregisters a simulation object from the result adapter. 
    /// </summary>
    /// <param name="simObject">The simulation entity to remove.</param>
    public void DeRegister(ISimResult simObject) {
      byte b;
      _simObjects.TryRemove(simObject, out b);
    }
  }
}