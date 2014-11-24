using System.Collections.Generic;
using GenericAgentArchitectureCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Movement {

    /// <summary>
    ///     A pipeline to postprocess the movement vector with several behaviors.
    /// </summary>
    public class ReactiveBehaviorPipeline {
        /// <summary>
        ///     Returns the content of this pipeline as unmodifiable list.
        /// </summary>
        /// <returns> the content of the pipeline </returns>
        public IList<IReactiveMovingBehavior> PipelineContent {
            get {
                return _pipeline.AsReadOnly();
            }
        }

        /// <summary>
        ///     The pipeline.
        /// </summary>
        private readonly List<IReactiveMovingBehavior> _pipeline = new List<IReactiveMovingBehavior>();

        /// <summary>
        ///     Processes the pipeline with a target position and an initial movement vector.
        /// </summary>
        /// <param name="targetPosition"> the target position of the movement </param>
        /// <param name="originalMovementVector"> the initial movement vector </param>
        /// <returns> the final result of the pipeline </returns>
        public Vector ProgressPipeline(Vector targetPosition, Vector originalMovementVector) {
            Vector v = originalMovementVector;

            foreach (IReactiveMovingBehavior behavior in _pipeline) {
                v = behavior.ModifyMovementVector(targetPosition, v);
            }

            return v;
        }

        /// <summary>
        ///     Adds a <seealso cref="IReactiveMovingBehavior" /> to the end of this pipeline.
        /// </summary>
        /// <param name="behavior"> the behavior to add </param>
        public void AddBehavior(IReactiveMovingBehavior behavior) {
            _pipeline.Add(behavior);
        }

        /// <summary>
        ///     Removes all occasions of the given <seealso cref="IReactiveMovingBehavior" /> from this pipeline.
        /// </summary>
        /// <param name="behavior"> the behavior to remove </param>
        /// <returns> true if the pipeline changed as a result of this method call. </returns>
        public bool RemoveBehavior(IReactiveMovingBehavior behavior) {
            int removed = _pipeline.RemoveAll(x => x.Equals(behavior));
            return removed > 0;
        }

        /// <summary>
        ///     Clears the pipeline.
        /// </summary>
        public void ClearPipeline() {
            _pipeline.Clear();
        }
    }

}