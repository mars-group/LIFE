using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Perception;
using LayerAPI.Perception;
using LayerAPI.Spatial;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;
using ISpatialObject = DalskiAgent.Environments.ISpatialObject;

namespace PedestrianModel.Environment {

    public class ObstacleEnvironment2D : Environment2D, IDataSource {
        public AgentLogger AgentLogger = new AgentLogger();
        public SimpleVisualization Visualization;

        # warning Size?
        public ObstacleEnvironment2D(SeqExec exec) : base(new Vector(1000, 1000), false) {
            exec.SetEnvironment(this);
            InitializeVisualization();
        }

        # warning Size?
        public ObstacleEnvironment2D() : base(new Vector(1000, 1000), false) {
            InitializeVisualization();
        }

        public object GetData(ISpecification spec) {
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

        public override void AdvanceEnvironment() {
            if (Visualization != null) Visualization.Invalidate();
            AgentLogger.Log(GetAllObjects().OfType<Pedestrian>().ToList());
        }

        private void InitializeVisualization() {
            new Thread
                (() => {
                    Visualization = new SimpleVisualization(this);
                    Application.Run(Visualization);
                }).Start();
        }
    }

}
