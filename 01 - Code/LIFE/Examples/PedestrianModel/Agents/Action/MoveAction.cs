using DalskiAgent.Movement;
using DalskiAgent.Movement.Actions;
using DalskiAgent.Movement.Movers;

namespace PedestrianModel.Agents.Action {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public class MoveAction {
        /// <returns> the targetPosition </returns>
        public Vector TargetPosition { get { return _targetPosition; } }

        /// <summary>
        ///     The default value for distanceTargetReached. This is the epsilon distance variation in which the agent
        ///     has reached its target.
        /// </summary>
        private const double DefaultDistanceTargetReached = 0.1;

        /// <summary>
        ///     The target position of this MoveAction.
        /// </summary>
        private readonly Vector _targetPosition;

        /// <summary>
        ///     If the distance of the agent's position to the target position is lesser than this value, it counts as
        ///     "target reached".
        /// </summary>
        private readonly double _distanceTargetReached;

        /// <summary>
        ///     Creates a new MoveAction.
        /// </summary>
        /// <param name="targetPosition"> the target position </param>
        /// <param name="distanceTargetReached"> the radius of the target area </param>
        public MoveAction(Vector targetPosition, double distanceTargetReached = DefaultDistanceTargetReached) {
            _targetPosition = targetPosition;
            _distanceTargetReached = distanceTargetReached;
        }

        //public override void PerformAction(EGOAPAgent agent)
        public DirectMovementAction PerformAction(Pedestrian agent, AgentMover mover) {
            //double? maxMovingSpeed = agent.getStartParameter("movingSpeed");
            float maxMovingSpeed = agent.MaxVelocity;
            //Vector3D direction = targetPosition.subtract(agent.Environment.CurrentPosition).normalize().scalarMultiply(maxMovingSpeed);
            Vector direction = (_targetPosition - agent.GetPosition()).GetNormalVector()*maxMovingSpeed;

            direction = agent.MovementPipeline.ProgressPipeline(_targetPosition, direction);

            //if (direction.Norm > 0.0)
            if (direction.GetLength() > 0.0) {
                //double currentMovingSpeed = direction.Norm;
                //double currentMovingSpeed = direction.Length;

                // calculating max move time
                //double distanceToTarget = agent.Environment.CurrentPosition.distance(targetPosition);

                //double distanceToTarget = Vector3DHelper.Distance(agentPos, targetPosition);

                //long moveDuration = (long)(1000d * distanceToTarget / currentMovingSpeed);
                //moveDuration = Math.Min(moveDuration, 200);

                //de.haw.walk.agentplatform.action.actions.MoveAction action = new de.haw.walk.agentplatform.action.actions.MoveAction(direction, moveDuration);

                //agent.Environment.executeAction(action);
#warning This function has to return a DalskiAgent.Movement.Actions.DirectMovementAction or similar

                Vector nextPosition = agent.GetPosition() + ((Config.LengthOfTimestepsInMilliseconds/1000f)*direction);
                return new DirectMovementAction(mover, nextPosition);
            }
            //de.haw.walk.agentplatform.action.actions.MoveAction action = new de.haw.walk.agentplatform.action.actions.MoveAction(Vector3D.ZERO, 0);

            //agent.Environment.executeAction(action);

#warning This function has to return a DalskiAgent.Movement.Actions.DirectMovementAction or similar
            return new DirectMovementAction(mover, agent.GetPosition());
        }

        //public override bool IsValid(EGOAPAgent agent)
        public bool IsValid(Pedestrian agent) {
            return true;
        }

        /// <summary>
        ///     Returns true if the distance to the target is less than DistanceTargetReached />.
        /// </summary>
        /// <param name="agent"> the agent </param>
        /// <returns> true if the agent is at the target position. </returns>
        //public override bool IsFinished(EGOAPAgent agent)
        public bool IsFinished(Pedestrian agent) {
            //Vector3D currentPosition = agent.Environment.CurrentPosition;
            Vector currentPosition = agent.GetPosition();

            // calculate the distance using only x and z values
            // double distance = Math.sqrt(currentPosition.getX() * currentPosition.getX() +
            // targetPosition.getZ()*targetPosition.getZ());

            //double distance = currentPosition.distance(targetPosition);
            double distance = currentPosition.GetDistance(_targetPosition);
            return distance < _distanceTargetReached;
        }

        public override sealed string ToString() {
            return "MoveAction to " + _targetPosition;
        }

        //public override bool IsSuccess(EGOAPAgent agent)
        public bool IsSuccess(Pedestrian agent) {
            return IsFinished(agent);
        }
    }

}