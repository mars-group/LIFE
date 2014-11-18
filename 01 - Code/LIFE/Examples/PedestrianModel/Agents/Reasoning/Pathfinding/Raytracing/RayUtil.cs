using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using DalskiAgent.Movement;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing {

    /// <summary>
    ///     @author Christian Thiel
    /// </summary>
    public sealed class RayUtil {
        /// <summary>
        ///     Private constructor.
        /// </summary>
        private RayUtil() {}

        /// <summary>
        ///     Returns the simulation object a ray with the given orign and direction intersects first. Returns
        ///     <code>null</code>, if no intersection found.
        /// </summary>
        /// <param name="orign"> the orign of the ray </param>
        /// <param name="direction"> the direction of the ray </param>
        /// <param name="objects"> the objects to search intersects with </param>
        /// <returns> the intersecting object or null </returns>
        //public static SimulationObject PickRay(Vector3D orign, Vector3D direction, ICollection<SimulationObject> objects)
        public static SpatialAgent PickRay(Vector orign, Vector direction, ICollection<SpatialAgent> objects) {
            SpatialAgent result = null;
            double minDistance = double.MaxValue;

            foreach (SpatialAgent so in objects) {
                //Vector3D intersect = GetIntersect(orign, direction, so);
                Vector intersect = GetIntersect(orign, direction, so);
                //if (!intersect.NaN)
                if (intersect != null) {
                    //double distance = orign.distance(intersect);
                    double distance = orign.GetDistance(intersect);
                    if (distance < minDistance) {
                        result = so;
                        minDistance = distance;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Calculates the intersection point of a ray (orign, direction) and a simulation object or return
        ///     Vector3D.NaN if no intersection exists.
        /// </summary>
        /// <param name="orign"> the ray orign </param>
        /// <param name="direction"> the ray direction </param>
        /// <param name="so"> the object to intersect </param>
        /// <returns> the intersection point or NaN </returns>
        //public static Vector3D GetIntersect(Vector3D orign, Vector3D direction, SimulationObject so)
        public static Vector GetIntersect(Vector orign, Vector direction, SpatialAgent so) {
            //Vector3D position = so.Position;
            Vector position = so.GetPosition();
            //Vector3D bounds = so.Bounds;
            Vector bounds = so.GetDimension();

            //return GetIntersectWithBox(orign, direction, position.add(-0.5, bounds), position.add(0.5, bounds));
            return GetIntersectWithBox(orign, direction, position + (-0.5f*bounds), position + (0.5f*bounds));
        }

        /// <summary>
        ///     /** Calculates the intersection point of a ray (orign, direction) and a AABB or return Vector3D.NaN if
        ///     no intersection exists.
        /// </summary>
        /// <param name="rayOrign"> the ray orign </param>
        /// <param name="rayDirection"> the ray direction </param>
        /// <param name="boxMin"> the lower corner of the AABB </param>
        /// <param name="boxMax"> the upper corner of the AABB </param>
        /// <returns> the intersection point or NaN </returns>
        //public static Vector3D GetIntersectWithBox(Vector3D rayOrign, Vector3D rayDirection, Vector3D boxMin, Vector3D boxMax)
        public static Vector GetIntersectWithBox(Vector rayOrign, Vector rayDirection, Vector boxMin, Vector boxMax) {
            // Ray is parallel to one of the axes
            // Disabling ReSharper warnings, because we know we're comparing to exact values here.
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (rayDirection.X == 0) {
                if (!(boxMin.X < rayOrign.X && rayOrign.X < boxMax.X)) {
                    //return Vector3D.NaN;
                    return null;
                }
            }
            if (rayDirection.Y == 0) {
                if (!(boxMin.Y < rayOrign.Y && rayOrign.Y < boxMax.Y)) {
                    //return Vector3D.NaN;
                    return null;
                }
            }
            if (rayDirection.Z == 0) {
                if (!(boxMin.Z < rayOrign.Z && rayOrign.Z < boxMax.Z)) {
                    //return Vector3D.NaN;
                    return null;
                }
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            // test X planes
            double tXNear = (boxMin.X - rayOrign.X)/rayDirection.X;
            double tXFar = (boxMax.X - rayOrign.X)/rayDirection.X;
            if (tXFar < tXNear) {
                double tmp = tXFar;
                tXFar = tXNear;
                tXNear = tmp;
            }
            // the ray starts after the object
            if (tXFar < 0) {
                //return Vector3D.NaN;
                return null;
            }
            double tNear = tXNear;
            double tFar = tXFar;

            // test Y planes
            double tYNear = (boxMin.Y - rayOrign.Y)/rayDirection.Y;
            double tYFar = (boxMax.Y - rayOrign.Y)/rayDirection.Y;
            if (tYFar < tYNear) {
                double tmp = tYFar;
                tYFar = tYNear;
                tYNear = tmp;
            }
            // the ray starts after the object
            if (tYFar < 0) {
                //return Vector3D.NaN;
                return null;
            }
            // intersections of single plains don't overlap, so no intersection at all
            if (tNear > tYFar || tYNear > tFar) {
                //return Vector3D.NaN;
                return null;
            }
            tNear = Math.Max(tNear, tYNear);
            tFar = Math.Min(tFar, tYFar);

            // test Z planes
            double tZNear = (boxMin.Z - rayOrign.Z)/rayDirection.Z;
            double tZFar = (boxMax.Z - rayOrign.Z)/rayDirection.Z;
            if (tZFar < tZNear) {
                double tmp = tZFar;
                tZFar = tZNear;
                tZNear = tmp;
            }
            // the ray starts after the object
            if (tZFar < 0) {
                //return Vector3D.NaN;
                return null;
            }
            // intersections of single plains don't overlap, so no intersection at all
            if (tNear > tZFar || tZNear > tFar) {
                //return Vector3D.NaN;
                return null;
            }
            tNear = Math.Max(tNear, tZNear);

            //return rayOrign.add(tNear, rayDirection);
            return rayOrign + ((float) tNear*rayDirection);
        }

        /// <summary>
        ///     Checks if the two points are visible to each other considering the given collection of simulation
        ///     objects as obstacles.
        /// </summary>
        /// <param name="orign"> the orign point </param>
        /// <param name="target"> the target point </param>
        /// <param name="obstacles"> the obstacles </param>
        /// <returns> true if the points are visible to each other, othervise false </returns>
        public static bool IsVisible(Vector orign, Vector target, IList<Obstacle> obstacles) {
            double minDistance = double.MaxValue;

            //Vector3D direction = target.subtract(orign);
            Vector direction = target - orign;

            foreach (SpatialAgent so in obstacles) {
                //Vector3D intersect = GetIntersect(orign, direction, so);
                Vector intersect = GetIntersect(orign, direction, so);
                //if (!intersect.NaN)
                if (intersect != null) {
                    //double distance = orign.distance(intersect);
                    double distance = orign.GetDistance(intersect);
                    if (distance < minDistance) minDistance = distance;
                }
            }

            //return orign.distance(target) < minDistance;
            return orign.GetDistance(target) < minDistance;
        }
    }

}