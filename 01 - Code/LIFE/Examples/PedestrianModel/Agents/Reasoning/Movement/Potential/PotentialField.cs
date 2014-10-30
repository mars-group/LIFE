using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public interface PotentialField
	{

		/// <summary>
		/// Adds an emitter to the field.
		/// </summary>
		/// <param name="emitter"> the emitter to add </param>
		void AddEmitter(PotentialEmitter emitter);

		/// <summary>
		/// Returns all potential emitters of this potential field.
		/// </summary>
		/// <returns> a collection of all potential emitters of this potential field </returns>
		ICollection<PotentialEmitter> Emitters {get;}

		/// <summary>
		/// Clears the whole potential field.
		/// </summary>
		void ClearAll();

		/// <summary>
		/// Calculates the potential of this potential field at the position <code>position</code> by adding the
		/// potentials of all <seealso cref="PotentialEmitter"/>s at this position.
		/// </summary>
		/// <param name="position"> the position of the potential </param>
		/// <returns> the potential at this position </returns>
		double CalculatePotential(Vector3D position);

	}

}