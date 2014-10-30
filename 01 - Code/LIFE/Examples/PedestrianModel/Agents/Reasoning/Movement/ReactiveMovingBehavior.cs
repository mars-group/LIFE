using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PedestrianModel.Agents.Reasoning.Movement
{
	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public interface ReactiveMovingBehavior
	{

		/// <summary>
		/// Modifies the chosen main movement vector.
		/// </summary>
		/// <param name="targetPosition"> target position of the current movement </param>
		/// <param name="currentPipelineVector"> the movement vector calculated by the previous behavior routine in the
		///            pipeline </param>
		/// <returns> the resulting movement vector </returns>
		Vector3D ModifyMovementVector(Vector3D targetPosition, Vector3D currentPipelineVector);

	}

}