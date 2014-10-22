using System.Collections.Generic;

namespace de.haw.walk.agent.util.pathfinding.potential
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class PotentialFieldCollection : HashSet<PotentialField>, PotentialField
	{

		/// 
		private const long serialVersionUID = 1L;

		/// <summary>
		/// Constructs a new empty potential field collection.
		/// </summary>
		public PotentialFieldCollection() : base()
		{
		}

		/// <summary>
		/// Constructs a new potential field collection using the given collection as initial elements.
		/// </summary>
		/// <param name="collection"> the collection whose elements are to be placed into this set </param>
		public PotentialFieldCollection(ICollection<PotentialField> collection) : base(collection)
		{
		}

		/// <summary>
		/// Calculates the potential for all fields for the given position and returns the sum of it.
		/// </summary>
		/// <param name="position"> the position to calculate the potential for </param>
		/// <returns> the sum of all potentials </returns>
		public double calculatePotential(Vector3D position)
		{
			double potentialSum = 0.0;

			foreach (PotentialField field in this)
			{
				potentialSum += field.calculatePotential(position);
			}

			return potentialSum;
		}

		public void addEmitter(PotentialEmitter emitter)
		{
		}

		public ICollection<PotentialEmitter> Emitters
		{
			get
			{
				// TODO Auto-generated method stub
				return null;
			}
		}

		public void clearAll()
		{
			this.Clear();
		}

	}

}