using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Environments;
using EnvironmentServiceComponent.Implementation;
using LifeAPI.Spatial;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;

namespace PedestrianModel.Environment {

    public class ObstacleEnvironment : ESCRectAdapter {
        public AgentLogger AgentLogger = new AgentLogger();
        public SimpleVisualization Visualization;

        # warning Size?
        public ObstacleEnvironment() : base(new RectESC(), new Vector(1000, 1000), false) {
            InitializeVisualization();
        }

        public new void AdvanceEnvironment() {
            if (Visualization != null) {
                Visualization.Invalidate();
            }
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