using System;
using System.Collections.Generic;
using DalskiAgent.Agents;
using GenericAgentArchitectureCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing {

    public sealed class RayUtil {
        /// <summary>
        ///     Private constructor.
        /// </summary>
        private RayUtil() {}

        /// <summary>
        ///     Returns the simulation object a ray with the given origin and direction intersects first. Returns
        ///     <code>null</code>, if no intersection found.
        /// </summary>
        /// <param name="origin"> the origin of the ray </param>
        /// <param name="direction"> the direction of the ray </param>
        /// <param name="objects"> the objects to search intersects with </param>
        /// <returns> the intersecting object or null </returns>
        public static SpatialAgent PickRay(Vector origin, Vector direction, ICollection<SpatialAgent> objects) {
            SpatialAgent result = null;
            double minDistance = double.MaxValue;

            foreach (SpatialAgent so in objects) {
                Vector intersect = GetIntersect(origin, direction, so);
                if (intersect != null) {
                    double distance = origin.GetDistance(intersect);
                    if (distance < minDistance) {
                        result = so;
                        minDistance = distance;
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Calculates the intersection point of a ray (origin, direction) and a simulation object or return
        ///     Vector3D.NaN if no intersection exists.
        /// </summary>
        /// <param name="origin"> the ray origin </param>
        /// <param name="direction"> the ray direction </param>
        /// <param name="so"> the object to intersect </param>
        /// <returns> the intersection point or NaN </returns>
        public static Vector GetIntersect(Vector origin, Vector direction, SpatialAgent so) {
            Vector position = so.GetPosition();
            Vector bounds = so.GetDimension();

            return GetIntersectWithBox(origin, direction, position + (-0.5d*bounds), position + (0.5d*bounds));
        }

        /// <summary>
        ///     /** Calculates the intersection point of a ray (origin, direction) and a AABB or return Vector3D.NaN if
        ///     no intersection exists.
        /// </summary>
        /// <param name="rayOrigin"> the ray origin </param>
        /// <param name="rayDirection"> the ray direction </param>
        /// <param name="boxMin"> the lower corner of the AABB </param>
        /// <param name="boxMax"> the upper corner of the AABB </param>
        /// <returns> the intersection point or NaN </returns>
        public static Vector GetIntersectWithBox(Vector rayOrigin, Vector rayDirection, Vector boxMin, Vector boxMax) {
            // Ray is parallel to one of the axes
            // Disabling ReSharper warnings, because we know we're comparing to exact values here.
            // ReSharper disable CompareOfFloatsByEqualityOperator
            if (rayDirection.X == 0) {
                if (!(boxMin.X < rayOrigin.X && rayOrigin.X < boxMax.X)) {
                    return null;
                }
            }
            if (rayDirection.Y == 0) {
                if (!(boxMin.Y < rayOrigin.Y && rayOrigin.Y < boxMax.Y)) {
                    return null;
                }
            }
            if (rayDirection.Z == 0) {
                if (!(boxMin.Z < rayOrigin.Z && rayOrigin.Z < boxMax.Z)) {
                    return null;
                }
            }
            // ReSharper restore CompareOfFloatsByEqualityOperator

            // test X planes
            double tXNear = (boxMin.X - rayOrigin.X)/rayDirection.X;
            double tXFar = (boxMax.X - rayOrigin.X)/rayDirection.X;
            if (tXFar < tXNear) {
                double tmp = tXFar;
                tXFar = tXNear;
                tXNear = tmp;
            }
            // the ray starts after the object
            if (tXFar < 0) {
                return null;
            }
            double tNear = tXNear;
            double tFar = tXFar;

            // test Y planes
            double tYNear = (boxMin.Y - rayOrigin.Y)/rayDirection.Y;
            double tYFar = (boxMax.Y - rayOrigin.Y)/rayDirection.Y;
            if (tYFar < tYNear) {
                double tmp = tYFar;
                tYFar = tYNear;
                tYNear = tmp;
            }
            // the ray starts after the object
            if (tYFar < 0) {
                return null;
            }
            // intersections of single plains don't overlap, so no intersection at all
            if (tNear > tYFar || tYNear > tFar) {
                return null;
            }
            tNear = Math.Max(tNear, tYNear);
            tFar = Math.Min(tFar, tYFar);

            // test Z planes
            double tZNear = (boxMin.Z - rayOrigin.Z)/rayDirection.Z;
            double tZFar = (boxMax.Z - rayOrigin.Z)/rayDirection.Z;
            if (tZFar < tZNear) {
                double tmp = tZFar;
                tZFar = tZNear;
                tZNear = tmp;
            }
            // the ray starts after the object
            if (tZFar < 0) {
                return null;
            }
            // intersections of single plains don't overlap, so no intersection at all
            if (tNear > tZFar || tZNear > tFar) {
                return null;
            }
            tNear = Math.Max(tNear, tZNear);

            return rayOrigin + ((float) tNear*rayDirection);
        }

        /// <summary>
        ///     Checks if the two points are visible to each other considering the given collection of simulation
        ///     objects as obstacles.
        /// </summary>
        /// <param name="origin"> the origin point </param>
        /// <param name="target"> the target point </param>
        /// <param name="obstacles"> the obstacles </param>
        /// <returns> true if the points are visible to each other, othervise false </returns>
        public static bool IsVisible(Vector origin, Vector target, IList<Obstacle> obstacles) {
            double minDistance = double.MaxValue;

            Vector direction = target - origin;

            foreach (SpatialAgent so in obstacles) {
                Vector intersect = GetIntersect(origin, direction, so);
                if (intersect != null) {
                    double distance = origin.GetDistance(intersect);
                    if (distance < minDistance) minDistance = distance;
                }
            }

            return origin.GetDistance(target) < minDistance;
        }
    }

}