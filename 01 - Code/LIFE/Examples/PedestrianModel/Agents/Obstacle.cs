using CommonTypes.TransportTypes;
using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Environments;
using GenericAgentArchitectureCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    /// <summary>
    ///   A passive obstacle agent which is used to create obstacles like walls.
    /// </summary>
    internal class Obstacle : SpatialAgent, IAgentLogic
    {
        /// <summary>
        ///   Create a new obstacle agent.
        /// </summary>
        /// <param name="id">Agent identifier.</param>
        /// <param name="env">Environment reference.</param>
        /// <param name="pos">Initial position.</param>
        public Obstacle(long id, IEnvironment env, TVector pos)
            : base(id, env, pos)
        {

        }

        public IInteraction Reason()
        {
            // Obstacles are passive.
            return null;
        }
    }
}
