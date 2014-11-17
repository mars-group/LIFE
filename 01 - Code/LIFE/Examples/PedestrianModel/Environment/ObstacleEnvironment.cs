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
    public class ObstacleEnvironment : Environment2D, IGenericDataSource
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

        /// <summary>
        ///   Create a new environment.
        /// </summary>
        public ObstacleEnvironment() : base(new Vector(1000, 1000), false)
        {

        }

        /// <summary>
        ///   Retrieve information from a data source.
        /// </summary>
        /// <param name="spec">Information object describing which data to query.</param>
        /// <returns>An object representing the percepted information.</returns>
        public object GetData(ISpecificator spec)
        {

            if (!(spec is Halo)) throw new Exception(
              "[Environment2D] Error on GetData() specificator: Not of type 'Halo'!");
            var halo = (Halo)spec;

            switch ((InformationTypes)spec.GetInformationType())
            {

                case InformationTypes.AllAgents:
                    var objects = new List<ISpatialObject>();
                    foreach (var obj in GetAllObjects())
                        if (halo.IsInRange(obj.GetPosition().GetTVector())) objects.Add(obj);
                    return objects;

                case InformationTypes.Obstacles:
                    {
                        var obstacle = new List<Obstacle>();
                        foreach (var obj in GetAllObjects().OfType<Obstacle>())
                            if (halo.IsInRange(obj.GetPosition().GetTVector())) obstacle.Add(obj);
                        return obstacle;
                    }

                case InformationTypes.Pedestrians:
                    {
                        var pedestrian = new List<Pedestrian>();
                        foreach (var obj in GetAllObjects().OfType<Pedestrian>())
                            if (halo.IsInRange(obj.GetPosition().GetTVector())) pedestrian.Add(obj);
                        return pedestrian;
                    }

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
