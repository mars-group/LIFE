using System;
using System.Collections.Generic;
using CommonTypes.TransportTypes;

namespace GenericAgentArchitectureCommon.Datatypes {

    /// <summary>
    ///     Axis Aligned Bounding Box
    ///     @author: Andrew Lubitz (https://code.google.com/p/modelthulhu/)
    /// </summary>
    public class AABB {
        private static float[][] EmptyBoundsArray { get { return new[] {new float[2], new float[2], new float[2]}; } }

        public static readonly AABB Omni = new AABB
            (new[] {
                new[] {float.MinValue, float.MaxValue},
                new[] {float.MinValue, float.MaxValue},
                new[] {float.MinValue, float.MaxValue}
            });

        // first index is x/y/z, second index is min/max
        public readonly float[][] Bounds;

        public AABB() {
            Bounds = EmptyBoundsArray;
        }

        public AABB(float[][] bounds) {
            Bounds = bounds;
        }

        // Gets a default (all zeros) array with the proper dimensions

        /// <summary>
        ///     Creates an AABB with both the min and max bounds set to the specified point
        /// </summary>
        /// <param name="point"></param>
        /// <returns></returns>
        public static AABB Point(TVector point) {
            return new AABB(new[] {new[] {point.X, point.X}, new[] {point.Y, point.Y}, new[] {point.Z, point.Z}});
        }

        /// <summary>
        ///     Creates an AABB with the specified minimum and maximum bounds
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static AABB FromMinMax(TVector min, TVector max) {
            return new AABB(new[] {new[] {min.X, max.X}, new[] {min.Y, max.Y}, new[] {min.Z, max.Z}});
        }

        /// <summary>
        ///     Returns an AABB expanded from the input AABB as necessary in order to include the specified point
        /// </summary>
        /// <param name="input"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static AABB ExpandedToFit(AABB input, TVector point) {
            AABB result = new AABB();
            float[] pointArray = {point.X, point.Y, point.Z};
            for (int dim = 0; dim < 3; dim++) {
                result.Bounds[dim][0] = Math.Min(input.Bounds[dim][0], pointArray[dim]);
                result.Bounds[dim][1] = Math.Max(input.Bounds[dim][1], pointArray[dim]);
            }
            return result;
        }

        /// <summary>
        ///     Computes and returns an AABB encompassing the volumes of both input boxes
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static AABB Union(AABB first, AABB second) {
            AABB result = new AABB();
            for (int dim = 0; dim < 3; dim++) {
                result.Bounds[dim][0] = Math.Min(first.Bounds[dim][0], second.Bounds[dim][0]);
                result.Bounds[dim][1] = Math.Max(first.Bounds[dim][1], second.Bounds[dim][1]);
            }
            return result;
        }

        /// <summary>
        ///     Computes and returns an AABB encompassing the intersection of the two input boxes' volumes
        ///     Don't use this value if CheckIntersection failed!
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static AABB Intersection(AABB first, AABB second) {
            AABB result = new AABB();
            for (int dim = 0; dim < 3; dim++) {
                result.Bounds[dim][0] = Math.Max(first.Bounds[dim][0], second.Bounds[dim][0]);
                result.Bounds[dim][1] = Math.Min(first.Bounds[dim][1], second.Bounds[dim][1]);
            }
            return result;
        }

        /// <summary>
        ///     Returns true if the two AABBs intersect, false otherwise
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public static bool CheckIntersection(AABB first, AABB second) {
            for (int dim = 0; dim < 3; dim++) {
                if (first.Bounds[dim][1] < second.Bounds[dim][0] || first.Bounds[dim][0] > second.Bounds[dim][1])
                    return false;
            }
            return true;
        }

        /// <summary>
        ///     Makes an AABB to fit the specified list of points
        /// </summary>
        /// <param name="points"></param>
        /// <returns></returns>
        public static AABB FitPointList(List<TVector> points) {
            float[][] bounds = EmptyBoundsArray;
            TVector first = points[0];
            bounds[0][0] = bounds[0][1] = first.X;
            bounds[1][0] = bounds[1][1] = first.Y;
            bounds[2][0] = bounds[2][1] = first.Z;
            for (int i = 1; i < points.Count; i++) {
                TVector point = points[i];
                bounds[0][0] = Math.Min(bounds[0][0], point.X);
                bounds[0][1] = Math.Max(bounds[0][1], point.X);
                bounds[1][0] = Math.Min(bounds[1][0], point.Y);
                bounds[1][1] = Math.Max(bounds[1][1], point.Y);
                bounds[2][0] = Math.Min(bounds[2][0], point.Z);
                bounds[2][1] = Math.Max(bounds[2][1], point.Z);
            }
            return new AABB(bounds);
        }
    }
}