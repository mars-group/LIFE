using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using GenericAgentArchitectureCommon.Interfaces;
using PedestrianModel.Agents;
using PedestrianModel.Logging;
using PedestrianModel.Visualization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PedestrianModel.Environment
{
    /// <summary>
    ///   An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    public class ObstacleEnvironment : Environment2D
    {
        private readonly IExecution _exec;  // Agent execution container reference.
        private readonly AgentLogger agentLogger = new AgentLogger();
        public static SimpleVisualization Visualization;

        /// <summary>
        ///   Create a new environment.
        /// </summary>
        public ObstacleEnvironment(SeqExec exec) : base(new Vector(1000, 1000), false) {
            _exec = exec;
            exec.SetEnvironment(this);        
        }

        public override object GetData(ISpecificator spec)
        {
            switch ((InformationTypes)spec.GetInformationType())
            {
                case InformationTypes.AllAgents:
                    return GetAllObjects();
                case InformationTypes.Obstacles:
                    return GetAllObjects().OfType<Obstacle>().ToList();
                case InformationTypes.Pedestrians:
                    return GetAllObjects().OfType<Pedestrian>().ToList();
                default: return null;
            }
        }

        public override void AdvanceEnvironment()
        {            
            if (Visualization != null)
            {
                Visualization.Invalidate();
            }
            agentLogger.Log(this.GetAllObjects().OfType<Pedestrian>().ToList());
        }

    }
}
