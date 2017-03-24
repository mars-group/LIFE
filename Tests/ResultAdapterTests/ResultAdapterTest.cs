﻿using LIFE.API.Layer;
using LIFE.API.Results;
using LIFE.Components.Agents.BasicAgents.Agents;
using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.Environments.GridEnvironment;
using NUnit.Framework;
using ResultAdapter.Implementation;
// ReSharper disable UnusedAutoPropertyAccessor.Local

namespace ResultAdapterTests {


  [TestFixture]
  public class ResultAdapterTest {

    [Test]
    public void TestCodeGeneration2() {
      var generator = new LoggerGenerator("127.0.0.1:8080", "rc-wolves");
      generator.WriteGeneratedCodeFile("ResultAdapterTests/GeneratedCode.cs");
    }
  }



  #region Agent Definitions for Testing. Use Result Config 'rc-wolves'.

  public class Grass : GridAgent<Grass>, ISimResult {

    public int FoodValue { get; private set; }    // Nutrition value (energy).
    public int FoodValueMax { get; }              // Maximum food value.
    public override Grass AgentReference => this; // Concrete agent reference.

    public Grass(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IGridEnvironment<GridAgent<Grass>> env)
      : base(layer, regFkt, unregFkt, env) {
      FoodValueMax = 60;
    }

    protected override IInteraction Reason() {
      return null;
    }
  }


  public enum Sex { Male, Female }

  public class Sheep : GridAgent<Sheep>, ISimResult {

    public Sex Sex { get; private set; }               // The sex of this animal.
    public int Energy { get; private set; }            // Current energy (with initial value).
    public int EnergyMax { get; }                      // Maximum health.
    public int Hunger { get; private set; }            // Hunger value of this sheep.
    public string Rule { get; private set; }           // Agent behaviour rule.
    public string Targets { get; private set; }        // Agent target sightings.
    public double TargetDistance { get; private set; } // Distance to active target. (-1 if not used)
    public override Sheep AgentReference => this;      // Concrete agent reference.

    public Sheep(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IGridEnvironment<GridAgent<Sheep>> env)
      : base(layer, regFkt, unregFkt, env) {
      EnergyMax = 40;
    }

    protected override IInteraction Reason() {
      return null;
    }
  }


  public class Wolf : GridAgent<Wolf>, ISimResult {

    public int Energy { get; private set; }            // Current energy (with initial value).
    public int EnergyMax { get; }                      // Maximum health.
    public int Hunger { get; private set; }            // Hunger value of this sheep.
    public string Rule { get; private set; }           // Agent behaviour rule.
    public string Targets { get; private set; }        // Agent target sightings.
    public double TargetDistance { get; private set; } // Distance to active target. (-1 if not used)
    public override Wolf AgentReference => this;       // Reference to the concrete wolf agent type.

    public Wolf(ILayer layer, RegisterAgent regFkt, UnregisterAgent unregFkt, IGridEnvironment<GridAgent<Wolf>> env)
      : base(layer, regFkt, unregFkt, env) {
      EnergyMax = 50;
    }

    protected override IInteraction Reason() {
      return null;
    }
  }
  #endregion
}