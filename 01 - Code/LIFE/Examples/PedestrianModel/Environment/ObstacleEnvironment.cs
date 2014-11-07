using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using DalskiAgent.Perception;
using PedestrianModel.Agents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Environment
{
    /// <summary>
    ///   An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    public class ObstacleEnvironment : Environment2D
    {
        private readonly IExecution _exec;  // Agent execution container reference.

        /// <summary>
        ///   Create a new environment.
        /// </summary>
        public ObstacleEnvironment(SeqExec exec) : base(new Vector(1000, 1000)) {
            _exec = exec;
            exec.SetEnvironment(this);        
        }

        public override object GetData(int informationType, LayerAPI.Interfaces.IGeometry geometry)
        {
            switch ((InformationTypes)informationType)
            {
                case InformationTypes.AllAgents:
                    return base.GetData(0, geometry);

                case InformationTypes.Obstacles:
                    {

                        Console.WriteLine("blaaaaaaa!");

                        var list = (List<SpatialAgent>)base.GetData(0, geometry);
                        return list.OfType<Obstacle>().ToList();
                    }
                case InformationTypes.Pedestrians:
                    {
                        var list = (List<SpatialAgent>)base.GetData(0, geometry);
                        return list.OfType<Pedestrian>().ToList();
                    }
                default: return null;
            }
        }

        public override void AdvanceEnvironment()
        {
            // Nothing to do here in this case.
        }
    }
}
