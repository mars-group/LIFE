using SpatialCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Movement {

    public interface IReactiveMovingBehavior {
        /// <summary>
        ///     Modifies the chosen main movement vector.
        /// </summary>
        /// <param name="targetPosition"> target position of the current movement </param>
        /// <param name="currentPipelineVector">
        ///     the movement vector calculated by the previous behavior routine in the
        ///     pipeline
        /// </param>
        /// <returns> the resulting movement vector </returns>
        Vector ModifyMovementVector(Vector targetPosition, Vector currentPipelineVector);
    }

}