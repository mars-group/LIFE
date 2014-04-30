using System;
using Primitive_Architecture.Dummies.Heating;
using Primitive_Architecture.Interaction;

namespace Primitive_Architecture.Agents.Heating {
  internal class AgentSmith : Agent {
    private readonly TempEnvironment _room; // The room where Smith lives in.


    /// <summary>
    ///   An agent walks into a bar ...
    /// </summary>
    /// <param name="room"></param>
    public AgentSmith(TempEnvironment room) : base("Smith ", null) {
      _room = room;
    }


    //TODO Muß weg!
    public new void Tick() {

      // Smith really likes a breeze of fresh air every now and then ...
      var random = new Random();
      var rnd = random.Next(10);
      if (rnd < 7) return;
      
      rnd = random.Next(10);
      var nowOpen = rnd > 7;
      if (nowOpen != _room.WindowOpen)
        Console.WriteLine("\n *** Agent Smith hat das Fenster " +
                          (nowOpen? "geöffnet" : "geschlossen") + "! ***");

      _room.WindowOpen = nowOpen;
    }

    protected override Plan CreatePlan() {
      throw new NotImplementedException();
    }
  }
}