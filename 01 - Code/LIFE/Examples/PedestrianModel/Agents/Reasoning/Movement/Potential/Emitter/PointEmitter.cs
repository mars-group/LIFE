using DalskiAgent.Movement;
using PedestrianModel.Util.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential.Emitter
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
		private readonly Vector position;

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
		public PointEmitter(Vector position, UnivariateRealFunction function) : base()
		{
			this.position = position;
			this.function = function;
		}

		public double GetPotential(Vector referringPosition)
		{
			double x = position.X - referringPosition.X;
			double z = position.Z - referringPosition.Z;

			return this.function.Value(Math.Sqrt(x * x + z * z));
		}

		/// <returns> the position </returns>
		public Vector Position
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