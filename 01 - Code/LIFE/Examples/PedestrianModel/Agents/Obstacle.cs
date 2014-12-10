using DalskiAgent.Agents;
using DalskiAgent.Environments;
using DalskiAgent.Execution;
using GenericAgentArchitectureCommon.Interfaces;
using SpatialCommon.Collision;
using SpatialCommon.Datatypes;

namespace PedestrianModel.Agents {

    /// <summary>
    ///     A passive obstacle agent which is used to create obstacles like walls.
    /// </summary>
    public class Obstacle : SpatialAgent, IAgentLogic {
        /// <summary>
        ///     Create a new obstacle agent.
        /// </summary>
        /// <param name="exec"></param>
        /// <param name="env">Environment reference.</param>
        /// <param name="position">Initial position.</param>
        /// <param name="dimension">Initial dimension.</param>
        /// <param name="direction">Initial direction.</param>
        public Obstacle(IExecution exec, IEnvironment env, Vector position, Vector dimension, Direction direction)
            : base(exec, env, CollisionType.StaticEnvironment, position, dimension, direction) {
            Init();
        }

        #region IAgentLogic Members

        public IInteraction Reason() {
            // Obstacles are passive.
            return null;
        }

        #endregion
    }

}