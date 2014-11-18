using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;

namespace PedestrianModel.Environment {

    /// <summary>
    ///     Data query information types.
    /// </summary>
    public enum InformationTypes {
        AllAgents,
        Obstacles,
        Pedestrians
    }

    /// <summary>
    ///     An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    public class ObstacleEnvironment : Environment2D, IGenericDataSource {
        public static AgentLogger AgentLogger = new AgentLogger();
        public static SimpleVisualization Visualization;

        /// <summary>
        ///     Create a new environment.
        /// </summary>
        public ObstacleEnvironment(SeqExec exec) : base(new Vector(1000, 1000), false) {
            exec.SetEnvironment(this);
        }

        /// <summary>
        ///     Create a new environment.
        /// </summary>
        public ObstacleEnvironment() : base(new Vector(1000, 1000), false) {}

        #region IGenericDataSource Members

        /// <summary>
        ///     Retrieve information from a data source.
        /// </summary>
        /// <param name="spec">Information object describing which data to query.</param>
        /// <returns>An object representing the percepted information.</returns>
        public object GetData(ISpecificator spec) {
            if (!(spec is Halo)) {
                throw new Exception
                    (
                    "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
            }
            Halo halo = (Halo) spec;

            switch ((InformationTypes) spec.GetInformationType()) {
                case InformationTypes.AllAgents:
                    List<ISpatialObject> objects = new List<ISpatialObject>();
                    foreach (ISpatialObject obj in GetAllObjects()) {
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
                    }
                    return objects;

                case InformationTypes.Obstacles: {
                    List<Obstacle> obstacle = new List<Obstacle>();
                    foreach (Obstacle obj in GetAllObjects().OfType<Obstacle>()) {
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) obstacle.Add(obj);
                    }
                    return obstacle;
                }

                case InformationTypes.Pedestrians: {
                    List<Pedestrian> pedestrian = new List<Pedestrian>();
                    foreach (Pedestrian obj in GetAllObjects().OfType<Pedestrian>()) {
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) pedestrian.Add(obj);
                    }
                    return pedestrian;
                }

                default:
                    return null;
            }
        }

        #endregion

        public override void AdvanceEnvironment() {
            if (Visualization != null) Visualization.Invalidate();
            AgentLogger.Log(GetAllObjects().OfType<Pedestrian>().ToList());
        }
    }

}