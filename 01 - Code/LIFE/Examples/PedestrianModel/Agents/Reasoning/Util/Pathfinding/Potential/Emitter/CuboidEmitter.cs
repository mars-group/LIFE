using System;

/// 
namespace de.haw.walk.agent.util.pathfinding.potential.emitter
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
		private readonly Vector3D start;

		/// <summary>
		/// The size of the cumboid along the positive axes.
		/// </summary>
		private readonly Vector3D size;

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
		public CuboidEmitter(Vector3D startPoint, Vector3D size, UnivariateRealFunction function)
		{
			this.start = startPoint.subtract(1.0 / 2.0, size);
			this.size = size;
			this.function = function;
		}

		public double getPotential(Vector3D @ref)
		{
			double distance = 0;

			double refX = @ref.X;
			double refZ = @ref.Z;

			double minX = start.X;
			double minZ = start.Z;
			double maxX = start.X + size.X;
			double maxZ = start.Z + size.Z;

			if (refX < minX)
			{
				// left of the emitter
				if (refZ < minZ)
				{
					// left below the emitter
					distance = distance(minX, minZ, refX, refZ);
				}
				else if (refZ > maxZ)
				{
					// left above the emitter
					distance = distance(minX, maxZ, refX, refZ);
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
				if (refZ < minZ)
				{
					// right below the emitter
					distance = distance(maxX, minZ, refX, refZ);
				}
				else if (refZ > maxZ)
				{
					// right above the emitter
					distance = distance(maxX, maxZ, refX, refZ);
				}
				else
				{
					// right of the emitter
					distance = refX - maxX;
				}
			}
			else
			{
				if (refZ < minZ)
				{
					// below the emitter
					distance = minZ - refZ;
				}
				else if (refZ > maxZ)
				{
					// above the emitter
					distance = refZ - maxZ;
				}
				else
				{
					// inside
					distance = -1.0;
				}
			}

			return function.value(distance);
		}

		/// <summary>
		/// Helper function to calculate the distance between two 2d points.
		/// </summary>
		/// <param name="pX"> pX </param>
		/// <param name="pZ"> pZ </param>
		/// <param name="qX"> qX </param>
		/// <param name="qZ"> qZ </param>
		/// <returns> the euklid distance </returns>
		private double distance(double pX, double pZ, double qX, double qZ)
		{
			double x = pX - qX;
			double z = pZ - qZ;

			return Math.Sqrt(x * x + z * z);
		}
	}

}