using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitecture.Movement;
using GenericAgentArchitecture.Perception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    /// <summary>
    ///   An environment used to simulate pedestrians in areas with obstacles like rooms or buildings with walls.
    /// </summary>
    internal class ObstacleEnvironment : Environment2D
    {

        /// <summary>
        ///   Create a new environment.
        /// </summary>
        public ObstacleEnvironment() : base(new Vector(1000, 1000), false) { }

        /* Data source functions: Information types and retrieval method. */
        public enum InformationTypes { Obstacles, Pedestrians }

        public override object GetData(int informationType, LayerAPI.Interfaces.IGeometry geometry)
        {
            switch ((InformationTypes)informationType)
            {
                case InformationTypes.Pedestrians:
                    {
                        var map = new Dictionary<long, SpatialAgent>();
                        var halo = (Halo)geometry;
                        foreach (var agent in GetAllAgents())
                        {
                            if (agent is Pedestrian)
                            {
                                if (halo.IsInRange(agent.GetPosition().GetTVector()) &&
                                halo.Position.GetDistance(agent.GetPosition()) > float.Epsilon)
                                {
                                    map[agent.Id] = agent;
                                }
                            }                            
                        }
                        return map;
                    }
                case InformationTypes.Obstacles:
                    {
                        var map = new Dictionary<long, SpatialAgent>();
                        var halo = (Halo)geometry;
                        foreach (var agent in GetAllAgents())
                        {
                            if (agent is Obstacle)
                            {
                                if (halo.IsInRange(agent.GetPosition().GetTVector()) &&
                                halo.Position.GetDistance(agent.GetPosition()) > float.Epsilon)
                                {
                                    map[agent.Id] = agent;
                                }
                            }
                        }
                        return map;
                    }
                default: return null;
            }
        }

        protected override void AdvanceEnvironment()
        {
            // Nothing to do here in this case.
        }
    }
}
