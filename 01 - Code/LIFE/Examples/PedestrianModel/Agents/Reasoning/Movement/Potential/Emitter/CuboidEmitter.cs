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
	public class CuboidEmitter : PotentialEmitter
	{

		/// <summary>
		/// The lowest point of this cuboid.
		/// </summary>
		private readonly Vector start;

		/// <summary>
		/// The size of the cumboid along the positive axes.
		/// </summary>
		private readonly Vector size;

		/// <summary>
		/// The function to calculate the potential with.
		/// </summary>
		private readonly UnivariateRealFunction function;

		/// <summary>
		/// Creates a rectangular, parallellepipedic cuboid. All edges of this polyhedron are parallel to one of
		/// the three axes.
		/// 
		/// The distance will only be calculated on two dimensions (x and z)!!
		///  
		/// </summary>
		/// <param name="startPoint"> the lowest point of this cuboid </param>
		/// <param name="size"> the size of the cumboid along the positive axes </param>
		/// <param name="function"> the function to calculate the potential with </param>
		public CuboidEmitter(Vector startPoint, Vector size, UnivariateRealFunction function)
		{
			//this.start = startPoint.subtract(1.0 / 2.0, size);
            this.start = startPoint - (0.5f * size);
			this.size = size;
			this.function = function;
		}

		public double GetPotential(Vector @ref)
		{
			double distance = 0;

			double refX = @ref.X;
			//double refZ = @ref.Z;
            double refY = @ref.Y;

			double minX = start.X;
			//double minZ = start.Z;
            double minY = start.Y;
			double maxX = start.X + size.X;
			//double maxZ = start.Z + size.Z;
            double maxY = start.Y + size.Y;

			if (refX < minX)
			{
				// left of the emitter
				//if (refZ < minZ)
                if (refY < minY)
				{
					// left below the emitter
					//distance = Distance(minX, minZ, refX, refZ);
                    distance = Distance(minX, minY, refX, refY);
				}
				//else if (refZ > maxZ)
                else if (refY > maxY)
				{
					// left above the emitter
					//distance = Distance(minX, maxZ, refX, refZ);
                    distance = Distance(minX, maxY, refX, refY);
				}
				else
				{
					// left of the emitter
					distance = minX - refX;
				}
			}
			else if (refX > maxX)
			{
				// right of the emitter
				//if (refZ < minZ)
                if (refY < minY)
				{
					// right below the emitter
					//distance = Distance(maxX, minZ, refX, refZ);
                    distance = Distance(maxX, minY, refX, refY);
				}
				//else if (refZ > maxZ)
                else if (refY > maxY)
				{
					// right above the emitter
					//distance = Distance(maxX, maxZ, refX, refZ);
                    distance = Distance(maxX, maxY, refX, refY);
				}
				else
				{
					// right of the emitter
					distance = refX - maxX;
				}
			}
			else
			{
				//if (refZ < minZ)
                if (refY < minY)
				{
					// below the emitter
					//distance = minZ - refZ;
                    distance = minY - refY;
				}
				//else if (refZ > maxZ)
                else if (refY > maxY)
				{
					// above the emitter
					//distance = refZ - maxZ;
                    distance = refY - maxY;
				}
				else
				{
					// inside
					distance = -1.0;
				}
			}

			return function.Value(distance);
		}

		/// <summary>
		/// Helper function to calculate the distance between two 2d points.
		/// </summary>
		/// <param name="pX"> pX </param>
		/// <param name="pZ"> pZ </param>
		/// <param name="qX"> qX </param>
		/// <param name="qZ"> qZ </param>
		/// <returns> the euklid distance </returns>
		//private double Distance(double pX, double pZ, double qX, double qZ)
        private double Distance(double pX, double pY, double qX, double qY)
		{
			double x = pX - qX;
			//double z = pZ - qZ;
            double y = pY - qY;

			//return Math.Sqrt(x * x + z * z);
            return Math.Sqrt(x * x + y * y);
		}
	}

}