using LIFE.Components.Agents.BasicAgents.Reasoning;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using Guid = System.Guid;

namespace DalskiAgent.newAsycClasses
{

    public class DirectMoverAsync {
        private readonly IAsyncEnvironment _env; // Environment interaction interface.
        private readonly Guid _agentId; // Spatial data, needed for some calculations.
        private readonly MovementDelegate _movementDelegate; // MovementDelegate for the movement result

        /// <summary>
        ///   Create an agent mover for direct placement.
        /// </summary>
        /// <param name="env">Environment interaction interface.</param>
        /// <param name="agentId">Spatial data, needed for some calculations.</param>
        /// <param name="movementSensor">Simple default sensor for movement feedback.</param>
        public DirectMoverAsync(IAsyncEnvironment env, Guid agentId, MovementDelegate movementDelegate) {
            _agentId = agentId;
            _movementDelegate = movementDelegate;
            _env = env;
        }


        /// <summary>
        ///   Set the agent to a new position.
        /// </summary>
        /// <param name="position">The target position.</param>
        /// <param name="dir">The new direction [default: use old heading].</param> 
        /// <returns>An interaction object that contains the code to execute this movement.</returns>
        public IInteraction SetToPosition(Vector3 position, Direction dir = null) {
            var tmpShape = _env.GetSpatialEntity(_agentId).Shape;
            if (dir == null) dir = tmpShape.Rotation;
            var vector = new Vector3
                (
                position.X - tmpShape.Position.X,
                position.Y - tmpShape.Position.Y,
                position.Z - tmpShape.Position.Z
                );

            return new MovementAction
                (delegate {
                    _env.Move(_agentId, vector, dir, _movementDelegate);
                });
        }
    }

}
