﻿using DalskiAgent.Movement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential
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

		public void AddEmitter(PotentialEmitter emitter)
		{
			this.potentialEmitters.Add(emitter);
		}

		public ICollection<PotentialEmitter> Emitters
		{
			get
			{
				//return Collections.unmodifiableSet(potentialEmitters);
                return potentialEmitters.ToList<PotentialEmitter>().AsReadOnly();
			}
		}

		public void ClearAll()
		{
			this.potentialEmitters.Clear();
		}

		public double CalculatePotential(Vector position)
		{
			double result = 0.0;

			foreach (PotentialEmitter emitter in potentialEmitters)
			{
				result += emitter.GetPotential(position);
			}

			return result;
		}

	}

}