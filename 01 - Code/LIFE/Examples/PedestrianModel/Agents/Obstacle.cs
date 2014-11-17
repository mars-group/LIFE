using CommonTypes.TransportTypes;
using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using GenericAgentArchitectureCommon.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents
{
    /// <summary>
    ///   A passive obstacle agent which is used to create obstacles like walls.
    /// </summary>
    public class Obstacle : SpatialAgent, IAgentLogic
    {

        /// <summary>
        ///   Create a new obstacle agent.
        /// </summary>
        /// <param name="id">Agent identifier.</param>
        /// <param name="env">Environment reference.</param>
        /// <param name="pos">Initial position.</param>
        public Obstacle(IExecution exec, IEnvironment env, Vector position, Vector dimension, Direction direction)
            : base(exec, env, position, dimension, direction)
        {
            Init();
        }

        public IInteraction Reason()
        {
            // Obstacles are passive.
            return null;
        }
    }
}
