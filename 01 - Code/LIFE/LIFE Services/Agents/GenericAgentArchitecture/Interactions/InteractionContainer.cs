using System;
using System.Collections.Generic;
using System.Linq;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Interfaces;

namespace GenericAgentArchitecture.Interactions {
  
  /// <summary>
  ///   The interaction container holds all interactions an agent can initiate.
  /// </summary>
  public class InteractionContainer {

    private readonly Agent _agent;                    // Self-reference to the owner.
    private readonly List<Interaction> _actions;      // Reflexive (degenerate) interactions.
    private readonly List<Interaction> _interactions; // Initializable interactions.
    private readonly List<Type> _targetInteractions;  // Interactions this agent can undergo.


    /// <summary>
    ///   Create a new interaction container.
    /// </summary>
    /// <param name="agent">The owning agent.</param>
    /// <param name="loader">Loader class to get agent type specific interactions.</param>
    public InteractionContainer(Agent agent, IIacLoader loader) {
      _agent = agent;
      _actions = loader.GetReflexiveActions(agent);
      _interactions = loader.GetInteractions(agent);
      _targetInteractions = loader.GetTargetInteractions(agent);   
    }

    
    /// <summary>
    ///   This method returns a set of all executable interactions.  
    /// </summary>
    /// <param name="neighborhood">A list of all agents that may be interacted with.</param>
    /// <returns>A mapping: Agent => List of available interactions. The reflexive actions
    /// are mapped to the self-reference of the agent possessing this container.</returns>
    public Dictionary<Agent, List<Interaction>> GetPossibleActions(IEnumerable<Agent> neighborhood) {     
      var tuples = new Dictionary<Agent, List<Interaction>>();

      // Check all available (reflexive) actions.
      var interactions = _actions.Where(action => 
        action.CheckPreconditions() && 
        action.CheckTrigger()).ToList();
      tuples.Add(_agent, interactions);

      // Next, check possible interactions with neighbor agents.
      foreach (var neighbor in neighborhood) {        
        interactions = new List<Interaction>();
        foreach (var interaction in _interactions) { // ↓ Is neighbor agent a valid target?
          if (neighbor.Interactions._targetInteractions.Contains(interaction.GetType())) {
            interaction.Target = neighbor;
            if (interaction.CheckPreconditions() && interaction.CheckTrigger()) {
              interactions.Add(interaction); // ↑ Is the interaction executable and useful? 
            }
          }
        }
        tuples.Add(neighbor, interactions);
      }

      return tuples;
    }
  }
}