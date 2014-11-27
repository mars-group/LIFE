using System;
using System.Collections.Generic;
using System.Linq;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Perception;
using EnvironmentServiceComponent.Implementation;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;
using SpatialCommon.Datatypes;
using SpatialCommon.Interfaces;
using ISpatialObject = DalskiAgent.Environments.ISpatialObject;

namespace PedestrianModel.Environment {

    /// <summary>
    ///     An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    public class ObstacleEnvironment : IEnvironment, IDataSource {
        public static AgentLogger AgentLogger = new AgentLogger();
        public static SimpleVisualization Visualization;

        private readonly IEnvironment _env;   // Environment implementation.
        public readonly bool UsesESC;       // Boolean to indicate ESC usage. 

        /// <summary>
        ///     Create a new environment.
        /// </summary>
        public ObstacleEnvironment(SeqExec exec, bool usesESC) {
            UsesESC = usesESC;
            # warning Size?
            if (UsesESC) _env = new ESCAdapter(new UnboundESC(), new Vector(1000, 1000), false);
            else _env = new Environment2D(new Vector(1000, 1000), false);
            exec.SetEnvironment(this);
        }

        /// <summary>
        ///     Create a new environment.
        /// </summary>
        public ObstacleEnvironment(bool usesESC) {
            UsesESC = usesESC;
            # warning Size?
            if (UsesESC) _env = new ESCAdapter(new UnboundESC(), new Vector(1000, 1000), false);
            else _env = new Environment2D(new Vector(1000, 1000), false);
        }

        #region IDataSource Members

        /// <summary>
        ///     Retrieve information from a data source.
        /// </summary>
        /// <param name="spec">Information object describing which data to query.</param>
        /// <returns>An object representing the percepted information.</returns>
        public object GetData(ISpecification spec) {
            if (UsesESC) {
                switch ((InformationType)spec.GetInformationType())
                {
                    case InformationType.AllAgents:
                        return GetAllObjects();
                    case InformationType.Obstacles:
                        return GetAllObjects().OfType<Obstacle>().ToList();
                    case InformationType.Pedestrians:
                        return GetAllObjects().OfType<Pedestrian>().ToList();
                    default:
                        return null;
                }
            }

            if (!(spec is Halo)) {
                throw new Exception
                    (
                    "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
            }
            Halo halo = (Halo) spec;

            switch ((InformationType) spec.GetInformationType()) {
                case InformationType.AllAgents:
                    List<ISpatialObject> objects = new List<ISpatialObject>();
                    foreach (ISpatialObject obj in GetAllObjects()) {
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
                    }
                    return objects;

                case InformationType.Obstacles: {
                    List<Obstacle> obstacle = new List<Obstacle>();
                    foreach (Obstacle obj in GetAllObjects().OfType<Obstacle>()) {
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) obstacle.Add(obj);
                    }
                    return obstacle;
                }

                case InformationType.Pedestrians: {
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

        public void AddObject(ISpatialObject obj, Vector pos, out DataAccessor acc, Vector dim, Direction dir) {
            _env.AddObject(obj, pos, out acc, dim, dir);
        }

        public void RemoveObject(ISpatialObject obj) {
            _env.RemoveObject(obj);
        }

        public void MoveObject(ISpatialObject obj, Vector movement, Direction dir = null) {
            _env.MoveObject(obj, movement, dir);
        }

        public List<ISpatialObject> GetAllObjects() {
            return _env.GetAllObjects();
        }

        public void AdvanceEnvironment() {
            if (Visualization != null) Visualization.Invalidate();
            AgentLogger.Log(GetAllObjects().OfType<Pedestrian>().ToList());
        }
    }

}