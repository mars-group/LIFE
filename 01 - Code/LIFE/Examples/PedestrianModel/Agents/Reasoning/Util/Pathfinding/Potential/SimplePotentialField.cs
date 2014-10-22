using System.Collections.Generic;

namespace de.haw.walk.agent.util.pathfinding.potential
{

	/// 
	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class SimplePotentialField : PotentialField
	{

		/// <summary>
		/// The collection of potential emitters.
		/// </summary>
		private readonly HashSet<PotentialEmitter> potentialEmitters = new HashSet<PotentialEmitter>();

		/// <summary>
		/// Creates a new and empty potential field.
		/// </summary>
		public SimplePotentialField()
		{

		}

		public void addEmitter(PotentialEmitter emitter)
		{
			this.potentialEmitters.Add(emitter);
		}

		public ICollection<PotentialEmitter> Emitters
		{
			get
			{
				return Collections.unmodifiableSet(potentialEmitters);
			}
		}

		public void clearAll()
		{
			this.potentialEmitters.Clear();
		}

		public double calculatePotential(Vector3D position)
		{
			double result = 0.0;

			foreach (PotentialEmitter emitter in potentialEmitters)
			{
				result += emitter.getPotential(position);
			}

			return result;
		}

	}

}