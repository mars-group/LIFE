using System;
using System.Collections.Generic;
using Primitive_Architecture.Agents.Heating;
using Primitive_Architecture.Dummies.Heating;
using Primitive_Architecture.Interfaces;
using Primitive_Architecture.Perception;

namespace Primitive_Architecture.Dummies {
  internal class Executor {
    private readonly List<ITickClient> _clients;

    private Executor() {
      var environment = new TempEnvironment();
      var heater = new HeaterAgent(environment);
      var sensors1 = new List<Sensor> {
        //TODO new HeaterSensor(environment), new TempSensor(environment), new WindowSensor(environment)
      };
      var contrl = new TempAgent(environment, heater, sensors1);
      var smith = new AgentSmith(environment);
      _clients = new List<ITickClient> {environment, contrl, heater, smith};
    }


    private void Run() {
      var input = "";
      while (input != "q") {
        foreach (var client in _clients) {
          client.Tick();
        }
        input = Console.ReadLine();
      }
    }


    public static void Main() {
      //new Executor().Run();
  
      Console.ReadLine();
    }
  }
}