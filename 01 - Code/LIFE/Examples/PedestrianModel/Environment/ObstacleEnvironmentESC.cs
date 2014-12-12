using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using EnvironmentServiceComponent.Implementation;
using LayerAPI.Spatial;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;

namespace PedestrianModel.Environment {

    public class ObstacleEnvironmentESC : ESCRectAdapter {
        public AgentLogger AgentLogger = new AgentLogger();
        public SimpleVisualization Visualization;

        # warning Size?
        public ObstacleEnvironmentESC(SeqExec exec) : base(new RectESC(), new Vector(1000, 1000), false) {
            exec.SetEnvironment(this);
            InitializeVisualization();
        }

        # warning Size?
        public ObstacleEnvironmentESC() : base(new RectESC(), new Vector(1000, 1000), false) {
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