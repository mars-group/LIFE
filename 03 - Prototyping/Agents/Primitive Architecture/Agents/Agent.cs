using System;
using System.Collections.Generic;
using Primitive_Architecture.Dummies;
using Primitive_Architecture.Interaction;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception;

namespace Primitive_Architecture.Agents {
  internal abstract class Agent : ITickClient {
    
    protected readonly bool DebugEnabled;
    private int _cycle;
    protected readonly string Id;
    private readonly PerceptionUnit _perceptionUnit;
    protected Vector3D Position; 


    /// <summary>
    /// Constructor for an abstract agent. It serves as a base class that is extended with
    /// domain specific sensors, actions and reasoning, optionally extended by a knowledge base.  
    /// </summary>
    /// <param name="id">A unique identifier, shall be used for log and communication.</param>
    /// <param name="sensors">List of sensors that are attached to the perception unit.</param>
    protected Agent(string id, List<Sensor> sensors) {
      Id = id;
      DebugEnabled = true;
      _perceptionUnit = new PerceptionUnit(sensors);
    }


    /// <summary>
    /// This is the main function of the agent program. It executes all three steps, 
    /// calling the concrete functions of the domain-specific agent, respectively.
    /// The execution is governed by some external runtime manager. 
    /// </summary>
    public void Tick() {
      _perceptionUnit.SenseAll();     // Phase 1: Perception
      var plan = CreatePlan();        // Phase 2: Reasoning
      plan.GetNextAction().Execute(); // Phase 3: Execution

      // Print the runtime information for debug purposes. 
      if (DebugEnabled) {
        _cycle++;  //TODO Checken, ob das toString() dieser Klasse aufgerufen wird, wenn kein anderes existiert.
        Console.Write(ToString());
      }
    }


    /// <summary>
    /// The reasoning function. It performs all planning/learning/thinking and returns a plan. 
    /// </summary>
    /// <returns>A sequential list of actions to execute.</returns>
    protected abstract Plan CreatePlan();
 


    protected new virtual string ToString() {
      return "Agent: " + Id + "\t Cycle: " + _cycle;
    }
  }
}