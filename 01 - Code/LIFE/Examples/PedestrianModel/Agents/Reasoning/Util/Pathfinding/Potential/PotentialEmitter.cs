using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace de.haw.walk.agent.util.pathfinding.potential
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public interface PotentialEmitter
	{

		/// <summary>
		/// Calculates the potential if this emitter at the given position <code>position</code> ant returns it.
		/// </summary>
		/// <param name="referringPosition"> the position to calculate the potential for. </param>
		/// <returns> the emitted potential of this emitter at the given position. </returns>
		double GetPotential(Vector3D referringPosition);

	}

}