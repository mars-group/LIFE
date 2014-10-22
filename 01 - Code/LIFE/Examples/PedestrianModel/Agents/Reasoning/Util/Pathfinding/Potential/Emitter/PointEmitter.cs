using PedestrianModel.Agents.Reasoning.Util.Pathfinding;
using System;
using System.Windows.Media.Media3D;

namespace de.haw.walk.agent.util.pathfinding.potential.emitter
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class PointEmitter : PotentialEmitter
	{

		/// <summary>
		/// The position of this point emitter.
		/// </summary>
		private readonly Vector3D position;

		/// <summary>
		/// The function to apply the distance to.
		/// </summary>
		private readonly UnivariateRealFunction function;

		/// <summary>
		/// Creates a new Point emitter at the position <code>position</code>. THe potential will be calculated by
		/// applying the euklid distance between the emitters position and the given position to the given
		/// <seealso cref="DoubleFunction"/>.<br/>
		/// <br/>
		/// The euklid distance between the two points will only be calculated 2D (x and z axis).
		/// </summary>
		/// <param name="position"> the position of the emitter </param>
		/// <param name="function"> the function to calculate the potential with </param>
		public PointEmitter(Vector3D position, UnivariateRealFunction function) : base()
		{
			this.position = position;
			this.function = function;
		}

		public double getPotential(Vector3D referringPosition)
		{
			double x = position.X - referringPosition.X;
			double z = position.Z - referringPosition.Z;

			return this.function.value(Math.Sqrt(x * x + z * z));
		}

		/// <returns> the position </returns>
		public Vector3D Position
		{
			get
			{
				return position;
			}
		}

		/// <returns> the function </returns>
		public UnivariateRealFunction Function
		{
			get
			{
				return function;
			}
		}

	}

}