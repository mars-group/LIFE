using DalskiAgent.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Movement
{
	/// <summary>
	/// A pipeline to postprocess the movement vector with several behaviors.
	/// 
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class ReactiveBehaviorPipeline
	{

		/// <summary>
		/// The pipeline.
		/// </summary>
		//private readonly IList<ReactiveMovingBehavior> pipeline = new List<ReactiveMovingBehavior>();
        private readonly List<ReactiveMovingBehavior> pipeline = new List<ReactiveMovingBehavior>();

		/// <summary>
		/// Creates a new reactive behavior pipeline.
		/// </summary>
		public ReactiveBehaviorPipeline()
		{

		}

		/// <summary>
		/// Processes the pipeline with a target position and an initial movement vector.
		/// </summary>
		/// <param name="targetPosition"> the target position of the movement </param>
		/// <param name="originalMovementVector"> the initial movement vector </param>
		/// <returns> the final result of the pipeline </returns>
		public Vector ProgressPipeline(Vector targetPosition, Vector originalMovementVector)
		{
			Vector v = originalMovementVector;

			foreach (ReactiveMovingBehavior behavior in pipeline)
			{
				v = behavior.ModifyMovementVector(targetPosition, v);
			}

			return v;
		}

		/// <summary>
		/// Adds a <seealso cref="ReactiveMovingBehavior"/> to the end of this pipeline.
		/// </summary>
		/// <param name="behavior"> the behavior to add </param>
		public void AddBehavior(ReactiveMovingBehavior behavior)
		{
			pipeline.Add(behavior);
		}

		/// <summary>
		/// Removes all occasions of the given <seealso cref="ReactiveMovingBehavior"/> from this pipeline.
		/// </summary>
		/// <param name="behavior"> the behavior to remove </param>
		/// <returns> true if the pipeline changed as a result of this method call. </returns>
		public bool RemoveBehavior(ReactiveMovingBehavior behavior)
		{
			//return pipeline.removeAll(Arrays.asList(behavior));
            int removed = pipeline.RemoveAll(x => x.Equals(behavior));
            return removed > 0;
		}

		/// <summary>
		/// Returns the content of this pipeline as unmodifiable list.
		/// </summary>
		/// <returns> the content of the pipeline </returns>
		public IList<ReactiveMovingBehavior> PipelineContent
		{
			get
			{
				//return Collections.unmodifiableList(pipeline);
                return pipeline.AsReadOnly();
			}
		}

		/// <summary>
		/// Clears the pipeline.
		/// </summary>
		public void ClearPipeline()
		{
			pipeline.Clear();
		}

	}

}