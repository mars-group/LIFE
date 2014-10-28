using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace de.haw.walk.agent.util.pathfinding.raytracing
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public sealed class RayUtil
	{

		/// <summary>
		/// Private constructor.
		/// </summary>
		private RayUtil()
		{

		}

		/// <summary>
		/// Returns the simulation object a ray with the given orign and direction intersects first. Returns
		/// <code>null</code>, if no intersection found.
		/// </summary>
		/// <param name="orign"> the orign of the ray </param>
		/// <param name="direction"> the direction of the ray </param>
		/// <param name="objects"> the objects to search intersects with </param>
		/// <returns> the intersecting object or null </returns>
		public static SimulationObject PickRay(Vector3D orign, Vector3D direction, ICollection<SimulationObject> objects)
		{
			SimulationObject result = null;
			double minDistance = double.MaxValue;

			foreach (SimulationObject so in objects)
			{
				//Vector3D intersect = GetIntersect(orign, direction, so);
                Vector3D? intersect = GetIntersect(orign, direction, so);
				//if (!intersect.NaN)
                if (intersect.HasValue)
				{
                    //double distance = orign.distance(intersect);
                    double distance = Distance(orign, intersect.Value);
					if (distance < minDistance)
					{
						result = so;
						minDistance = distance;
					}
				}
			}

			return result;
		}

		/// <summary>
		/// Calculates the intersection point of a ray (orign, direction) and a simulation object or return
		/// Vector3D.NaN if no intersection exists.
		/// </summary>
		/// <param name="orign"> the ray orign </param>
		/// <param name="direction"> the ray direction </param>
		/// <param name="so"> the object to intersect </param>
		/// <returns> the intersection point or NaN </returns>
		//public static Vector3D GetIntersect(Vector3D orign, Vector3D direction, SimulationObject so)
        public static Vector3D? GetIntersect(Vector3D orign, Vector3D direction, SimulationObject so)
		{
			Vector3D position = so.Position;
			Vector3D bounds = so.Bounds;

			//return GetIntersectWithBox(orign, direction, position.add(-0.5, bounds), position.add(0.5, bounds));
            return GetIntersectWithBox(orign, direction, Vector3D.Add(position, Vector3D.Multiply(-0.5, bounds)), Vector3D.Add(position, Vector3D.Multiply(0.5, bounds)));
		}

		/// <summary>
		/// /** Calculates the intersection point of a ray (orign, direction) and a AABB or return Vector3D.NaN if
		/// no intersection exists.
		/// </summary>
		/// <param name="rayOrign"> the ray orign </param>
		/// <param name="rayDirection"> the ray direction </param>
		/// <param name="boxMin"> the lower corner of the AABB </param>
		/// <param name="boxMax"> the upper corner of the AABB </param>
		/// <returns> the intersection point or NaN </returns>
		//public static Vector3D GetIntersectWithBox(Vector3D rayOrign, Vector3D rayDirection, Vector3D boxMin, Vector3D boxMax)
        public static Vector3D? GetIntersectWithBox(Vector3D rayOrign, Vector3D rayDirection, Vector3D boxMin, Vector3D boxMax)
		{
			double tNear = double.NegativeInfinity;
			double tFar = double.PositiveInfinity;

			// Ray is parallel to one of the axes
			if (rayDirection.X == 0)
			{
				if (!(boxMin.X < rayOrign.X && rayOrign.X < boxMax.X))
				{
					//return Vector3D.NaN;
                    return null;
				}
			}
			if (rayDirection.Y == 0)
			{
				if (!(boxMin.Y < rayOrign.Y && rayOrign.Y < boxMax.Y))
				{
					//return Vector3D.NaN;
                    return null;
				}
			}
			if (rayDirection.Z == 0)
			{
				if (!(boxMin.Z < rayOrign.Z && rayOrign.Z < boxMax.Z))
				{
					//return Vector3D.NaN;
                    return null;
				}
			}

			// test X planes
			double tXNear, tXFar;
			tXNear = (boxMin.X - rayOrign.X) / rayDirection.X;
			tXFar = (boxMax.X - rayOrign.X) / rayDirection.X;
			if (tXFar < tXNear)
			{
				double tmp = tXFar;
				tXFar = tXNear;
				tXNear = tmp;
			}
			// the ray starts after the object
			if (tXFar < 0)
			{
				//return Vector3D.NaN;
                return null;
			}
			tNear = tXNear;
			tFar = tXFar;

			// test Y planes
			double tYNear, tYFar;
			tYNear = (boxMin.Y - rayOrign.Y) / rayDirection.Y;
			tYFar = (boxMax.Y - rayOrign.Y) / rayDirection.Y;
			if (tYFar < tYNear)
			{
				double tmp = tYFar;
				tYFar = tYNear;
				tYNear = tmp;
			}
			// the ray starts after the object
			if (tYFar < 0)
			{
				//return Vector3D.NaN;
                return null;
			}
			// intersections of single plains don't overlap, so no intersection at all
			if (tNear > tYFar || tYNear > tFar)
			{
				//return Vector3D.NaN;
                return null;
			}
			tNear = Math.Max(tNear, tYNear);
			tFar = Math.Min(tFar, tYFar);

			// test Z planes
			double tZNear, tZFar;
			tZNear = (boxMin.Z - rayOrign.Z) / rayDirection.Z;
			tZFar = (boxMax.Z - rayOrign.Z) / rayDirection.Z;
			if (tZFar < tZNear)
			{
				double tmp = tZFar;
				tZFar = tZNear;
				tZNear = tmp;
			}
			// the ray starts after the object
			if (tZFar < 0)
			{
				//return Vector3D.NaN;
                return null;
			}
			// intersections of single plains don't overlap, so no intersection at all
			if (tNear > tZFar || tZNear > tFar)
			{
				//return Vector3D.NaN;
                return null;
			}
			tNear = Math.Max(tNear, tZNear);
			tFar = Math.Min(tFar, tZFar);

			//return rayOrign.add(tNear, rayDirection);
            return Vector3D.Add(rayOrign, Vector3D.Multiply(tNear, rayDirection));
		}

		/// <summary>
		/// Checks if the two points are visible to each other considering the given collection of simulation
		/// objects as obstacles.
		/// </summary>
		/// <param name="orign"> the orign point </param>
		/// <param name="target"> the target point </param>
		/// <param name="obstacles"> the obstacles </param>
		/// <returns> true if the points are visible to each other, othervise false </returns>
		public static bool IsVisible(Vector3D orign, Vector3D target, IList<SimulationObject> obstacles)
		{
			double minDistance = double.MaxValue;

			//Vector3D direction = target.subtract(orign);
            Vector3D direction = Vector3D.Subtract(target, orign);

			foreach (SimulationObject so in obstacles)
			{
				//Vector3D intersect = GetIntersect(orign, direction, so);
                Vector3D? intersect = GetIntersect(orign, direction, so);
				//if (!intersect.NaN)
                if (intersect.HasValue)
				{
					//double distance = orign.distance(intersect);
                    double distance = Distance(orign, intersect.value);
					if (distance < minDistance)
					{
						minDistance = distance;
					}
				}
			}

			//return orign.distance(target) < minDistance;
            return Distance(orign, target) < minDistance;
		}

        public static double Distance(Vector3D from, Vector3D to)
        {
            double dx = to.X - from.X;
            double dy = to.Y - from.Y;
            double dz = to.Z - from.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }
	}

}