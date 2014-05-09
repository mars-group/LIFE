using System;
using Primitive_Architecture.Interactions;
using Primitive_Architecture.Interactions.Heating;
using Common.Interfaces;

namespace Primitive_Architecture.Agents.Heating {
  
  /// <summary>
  /// This is Smith. No need for further explanations ...?!
  /// </summary>
  internal class AgentSmith : Agent, IAgentLogic {
    
    private readonly TempEnvironment _room; // The room where Smith lives in.

    /// <summary>
    /// An agent walks into a bar ...
    /// </summary>
    /// <param name="room"></param>
    public AgentSmith(TempEnvironment room) : base("Smith ") {
      _room = room;
      ReasoningComponent = this;
    }


    /// <summary>
    /// Just doing some random disturbance.
    /// </summary>
    /// <returns>A open-the-window interaction.</returns>
    public IInteraction Reason() {

      // Smith really likes a breeze of fresh air every now and then ...
      var random = new Random();
      var rnd = random.Next(10);
      if (rnd < 8) return null;
      
      rnd = random.Next(10);
      var nowOpen = rnd > 7;
      if (nowOpen != _room.WindowOpen) {
        Console.WriteLine("\n *** Agent Smith hat das Fenster " +
                          (nowOpen ? "geöffnet" : "geschlossen") + "! ***");
      }
      return  new OpenWindowInteraction("", _room, nowOpen);
    }

    /*
    /// <summary>
    /// Smith is chatty.
    /// </summary>
    /// <returns>Console output string.</returns>
    protected override string ToString () {
      return "Agent: "+Id+" - ";
    } */
  }
}