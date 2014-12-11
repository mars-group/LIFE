using System;
using System.Collections.Generic;
using System.Globalization;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using GeoAPI.Geometries;
using LayerAPI.Interfaces;
using LayerAPI.Interfaces.Visualization;
using MessageWrappers;
using Mono.Addins;
using PedestrianModel.Agents;
using PedestrianModel.Environment;
using PedestrianModel.Util;
using SpatialCommon.Datatypes;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PedestrianModel {

    /// <summary>
    ///     This layer implementation contains a pedestrian simulation..
    ///     It uses the Generic Agent Architecture and serves as an example for other agent models.
    /// </summary>
    [Extension(typeof (ISteppedLayer))]
    public class PedestrianLayer : ISteppedActiveLayer, IVisualizable
    {
        private long _tick; // Counter of current tick.  
        private IEnvironment _env; // Environment object for spatial agents. 
        private IExecution _exec; // Agent execution container reference.

        #region ISteppedActiveLayer Members

        /// <summary>
        ///     Initializes this layer.
        /// </summary>
        /// <typeparam name="T">Object type of layer init data object.</typeparam>
        /// <param name="layerInitData">Generic layer init data object. Not used here!</param>
        /// <param name="registerAgentHandle">Delegate for agent registration function.</param>
        /// <param name="unregisterAgentHandle">Delegate for agent unregistration function.</param>
        /// <returns></returns>
        public bool InitLayer<T>
            (T layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            _tick = 0;
            
            ChooseCollisions();
            ChooseScenario();

            if (Config.UsesESC) _env = new ObstacleEnvironmentESC();
            else _env = new ObstacleEnvironment2D();
            _exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);

            ScenarioBuilder.CreateScenario(_exec, _env, Config.Scenario);

            // Register the layer itself for execution. The agents are registered by themselves.
            registerAgentHandle.Invoke(this, this);
            return true;
        }

        public void PreTick() {
            if (_env is ObstacleEnvironmentESC) {
                ObstacleEnvironmentESC escEnv = (ObstacleEnvironmentESC) _env;
                escEnv.AdvanceEnvironment();
            }
            _env.AdvanceEnvironment();
        }

        /// <summary>
        ///     This layer is also tickable to execute some functions.
        ///     It increases the tick counter.
        /// </summary>
        public void Tick() {
            Console.WriteLine("Tick: " + _tick);
        }

        public void PostTick() {}

        /// <summary>
        ///     Returns the current tick.
        /// </summary>
        /// <returns>Current tick value.</returns>
        public long GetCurrentTick() {
            return _tick;
        }

        public void SetCurrentTick(long currentTick) {
            _tick = currentTick;
        }

        #endregion

        private void ChooseCollisions()
        {
            Console.WriteLine("Calculate collisions?\n1: Collisions on.\n2: Collisions off.");
            String decisionString = Console.ReadLine();
            int decisionInt;
            int.TryParse(decisionString, out decisionInt);
            if (decisionInt == 1)
            {
                Config.UsesESC = true;
                Console.WriteLine("Collisions on.");
            }
            else if (decisionInt == 2)
            {
                Config.UsesESC = false;
                Console.WriteLine("Collisions off.");
            }
            else
            {
                Console.WriteLine("Invalid input. Using default settings.");
            }
        }

        private void ChooseScenario()
        {
            Console.WriteLine("Which scenario type?");
            Console.WriteLine("1: Effect of bottleneck width on pedestrian flow.");
            Console.WriteLine("2: Effect of pedestrians in front of bottleneck on pedestrian flow.");
            Console.WriteLine("3: Effect of pedestrian density on pedestrian velocity.");
            Console.WriteLine("4: Pedestrian behavior at bottlenecks with opposing flow directions.");
            Console.WriteLine("5: Pedestrian behavior in corridors with opposing flow directions.");
            Console.WriteLine("6: Pedestrian behavior at dead ends.");
            String decisionString = Console.ReadLine();
            int decisionInt;
            int.TryParse(decisionString, out decisionInt);
            switch (decisionInt)
            {
                case 1:
                    Console.WriteLine("1: " + ScenarioType.Bottleneck40Cm);
                    Console.WriteLine("2: " + ScenarioType.Bottleneck50Cm);
                    Console.WriteLine("3: " + ScenarioType.Bottleneck60Cm);
                    Console.WriteLine("4: " + ScenarioType.Bottleneck70Cm);
                    Console.WriteLine("5: " + ScenarioType.Bottleneck80Cm);
                    Console.WriteLine("6: " + ScenarioType.Bottleneck90Cm);
                    Console.WriteLine("7: " + ScenarioType.Bottleneck100Cm);
                    Console.WriteLine("8: " + ScenarioType.Bottleneck120Cm);
                    Console.WriteLine("9: " + ScenarioType.Bottleneck140Cm);
                    Console.WriteLine("10: " + ScenarioType.Bottleneck160Cm);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.Bottleneck40Cm;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.Bottleneck50Cm;
                            break;
                        case 3:
                            Config.Scenario = ScenarioType.Bottleneck60Cm;
                            break;
                        case 4:
                            Config.Scenario = ScenarioType.Bottleneck70Cm;
                            break;
                        case 5:
                            Config.Scenario = ScenarioType.Bottleneck80Cm;
                            break;
                        case 6:
                            Config.Scenario = ScenarioType.Bottleneck90Cm;
                            break;
                        case 7:
                            Config.Scenario = ScenarioType.Bottleneck100Cm;
                            break;
                        case 8:
                            Config.Scenario = ScenarioType.Bottleneck120Cm;
                            break;
                        case 9:
                            Config.Scenario = ScenarioType.Bottleneck140Cm;
                            break;
                        case 10:
                            Config.Scenario = ScenarioType.Bottleneck160Cm;
                            break;
                    }
                    break;
                case 2:
                    Console.WriteLine("1: " + ScenarioType.Bottleneck50Agents);
                    Console.WriteLine("2: " + ScenarioType.Bottleneck75Agents);
                    Console.WriteLine("3: " + ScenarioType.Bottleneck100Agents);
                    Console.WriteLine("4: " + ScenarioType.Bottleneck125Agents);
                    Console.WriteLine("5: " + ScenarioType.Bottleneck150Agents);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.Bottleneck50Agents;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.Bottleneck75Agents;
                            break;
                        case 3:
                            Config.Scenario = ScenarioType.Bottleneck100Agents;
                            break;
                        case 4:
                            Config.Scenario = ScenarioType.Bottleneck125Agents;
                            break;
                        case 5:
                            Config.Scenario = ScenarioType.Bottleneck150Agents;
                            break;
                    }
                    break;
                case 3:
                    Console.WriteLine("1: " + ScenarioType.Density50Agents);
                    Console.WriteLine("2: " + ScenarioType.Density100Agents);
                    Console.WriteLine("3: " + ScenarioType.Density200Agents);
                    Console.WriteLine("4: " + ScenarioType.Density300Agents);
                    Console.WriteLine("5: " + ScenarioType.Density400Agents);
                    Console.WriteLine("6: " + ScenarioType.Density500Agents);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.Density50Agents;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.Density100Agents;
                            break;
                        case 3:
                            Config.Scenario = ScenarioType.Density200Agents;
                            break;
                        case 4:
                            Config.Scenario = ScenarioType.Density300Agents;
                            break;
                        case 5:
                            Config.Scenario = ScenarioType.Density400Agents;
                            break;
                        case 6:
                            Config.Scenario = ScenarioType.Density500Agents;
                            break;
                    }
                    break;
                case 4:
                    Console.WriteLine("1: " + ScenarioType.Oscillation50Cm);
                    Console.WriteLine("2: " + ScenarioType.Oscillation100Cm);
                    Console.WriteLine("3: " + ScenarioType.Oscillation200Cm);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.Oscillation50Cm;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.Oscillation100Cm;
                            break;
                        case 3:
                            Config.Scenario = ScenarioType.Oscillation200Cm;
                            break;
                    }
                    break;
                case 5:
                    Console.WriteLine("1: " + ScenarioType.Lane250Cm);
                    Console.WriteLine("2: " + ScenarioType.Lane500Cm);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.Lane250Cm;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.Lane500Cm;
                            break;
                    }
                    break;
                case 6:
                    Console.WriteLine("1: " + ScenarioType.UShape500Cm);
                    Console.WriteLine("2: " + ScenarioType.UShape10000Cm);
                    decisionString = Console.ReadLine();
                    int.TryParse(decisionString, out decisionInt);
                    switch (decisionInt)
                    {
                        case 1:
                            Config.Scenario = ScenarioType.UShape500Cm;
                            break;
                        case 2:
                            Config.Scenario = ScenarioType.UShape10000Cm;
                            break;
                    }
                    break;
                default:
                    Console.WriteLine("Invalid input. Using default scenario.");
                    break;
            }
        }

        public List<BasicVisualizationMessage> GetVisData()
        {
            List<BasicVisualizationMessage> result = new List<BasicVisualizationMessage>();
            foreach (var o in _env.GetAllObjects())
            {
                if (o is Obstacle)
                {
                    Obstacle obs = (Obstacle)o;
                    Vector obsPos = obs.GetPosition();
                    Vector obsDim = obs.GetDimension();
                    NonMovingPassiveObject npo = new NonMovingPassiveObject
                        (Definitions.PassiveTypes.WallPassiveObject,
                            obsPos.X,
                            obsPos.Y,
                            obsPos.Z,
                            0,
                            obs.Id.ToString(CultureInfo.InvariantCulture),
                            "Wall",
                            "Obstacle",
                            1,
                            obsDim.X,
                            obsDim.Y);
                    result.Add(npo);
                }
                else if (o is Pedestrian)
                {
                    Pedestrian ped = (Pedestrian)o;
                    Vector pedPos = ped.GetPosition();
                    Vector pedDim = ped.GetDimension();
                    MovingBasicAgent mba = new MovingBasicAgent
                        (Definitions.AgentTypes.HumanAgent,
                            pedPos.X,
                            pedPos.Y,
                            pedPos.Z,
                            0,
                            ped.Id.ToString(CultureInfo.InvariantCulture),
                            _tick,
                            (float)pedDim.X,
                            (float)pedDim.Y,
                            (float)pedDim.Z,
                            new Dictionary<string, string>(),
                            "Pedestrian",
                            "Moving",
                            0,
                            new List<GroupDefinition>());
                    result.Add(mba);
                }
            }
            return result;
        }

        public List<BasicVisualizationMessage> GetVisData(IGeometry geometry)
        {
            return GetVisData();
        }
    }

}