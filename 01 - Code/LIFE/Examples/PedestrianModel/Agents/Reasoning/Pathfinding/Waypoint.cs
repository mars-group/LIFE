using DalskiAgent.Movement.Actions;
using DalskiAgent.Movement.Movers;
using SpatialCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Pathfinding {

    public class Waypoint {
        /// <returns> the targetPosition </returns>
        public Vector TargetPosition { get { return _targetPosition; } }

        /// <summary>
        ///     The default value for distanceTargetReached. This is the epsilon distance variation in which the agent
        ///     has reached its target.
        /// </summary>
        private const double DefaultDistanceTargetReached = 0.1;

        /// <summary>
        ///     The target position of this Waypoint.
        /// </summary>
        private readonly Vector _targetPosition;

        /// <summary>
        ///     If the distance of the agent's position to the target position is lesser than this value, it counts as
        ///     "target reached".
        /// </summary>
        private readonly double _distanceTargetReached;

        /// <summary>
        ///     Creates a new Waypoint.
        /// </summary>
        /// <param name="targetPosition"> the target position </param>
        /// <param name="distanceTargetReached"> the radius of the target area </param>
        public Waypoint(Vector targetPosition, double distanceTargetReached = DefaultDistanceTargetReached) {
            _targetPosition = targetPosition;
            _distanceTargetReached = distanceTargetReached;
        }

        public DirectMovementAction Approach(Pedestrian agent, AgentMover mover) {
            double maxMovingSpeed = agent.MaxVelocity;
            Vector direction = (_targetPosition - agent.GetPosition()).GetNormalVector()*maxMovingSpeed;

            direction = agent.MovementPipeline.ProgressPipeline(_targetPosition, direction);

            if (direction.GetLength() > 0.0) {
                Vector nextPosition = agent.GetPosition() + ((Config.LengthOfTimestepsInMilliseconds/1000d)*direction);
                return new DirectMovementAction(mover, nextPosition);
            }
            return new DirectMovementAction(mover, agent.GetPosition());
        }

        public bool IsValid(Pedestrian agent) {
            return true;
        }

        /// <summary>
        ///     Returns true if the distance to the target is less than DistanceTargetReached />.
        /// </summary>
        /// <param name="agent"> the agent </param>
        /// <returns> true if the agent is at the target position. </returns>
        public bool IsReached(Pedestrian agent) {
            Vector currentPosition = agent.GetPosition();
            double distance = currentPosition.GetDistance(_targetPosition);
            return distance < _distanceTargetReached;
        }

        public override sealed string ToString() {
            return "Waypoint to " + _targetPosition;
        }

        public bool IsSuccess(Pedestrian agent) {
            return IsReached(agent);
        }
    }

}