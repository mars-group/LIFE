using System;
using LifeAPI.Spatial;
using PedestrianModel.Util.Math;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential.Emitter {

    public class CuboidEmitter : IPotentialEmitter {
        /// <summary>
        ///     The lowest point of this cuboid.
        /// </summary>
        private readonly Vector _start;

        /// <summary>
        ///     The size of the cumboid along the positive axes.
        /// </summary>
        private readonly Vector _size;

        /// <summary>
        ///     The function to calculate the potential with.
        /// </summary>
        private readonly IUnivariateRealFunction _function;

        /// <summary>
        ///     Creates a rectangular, parallellepipedic cuboid. All edges of this polyhedron are parallel to one of
        ///     the three axes.
        ///     The distance will only be calculated on two dimensions (x and z)!!
        /// </summary>
        /// <param name="startPoint"> the lowest point of this cuboid </param>
        /// <param name="size"> the size of the cumboid along the positive axes </param>
        /// <param name="function"> the function to calculate the potential with </param>
        public CuboidEmitter(Vector startPoint, Vector size, IUnivariateRealFunction function) {
            _start = startPoint - (0.5d*size);
            _size = size;
            _function = function;
        }

        #region PotentialEmitter Members

        public double GetPotential(Vector reference) {
            double distance;

            double refX = reference.X;
            double refY = reference.Y;

            double minX = _start.X;
            double minY = _start.Y;
            double maxX = _start.X + _size.X;
            double maxY = _start.Y + _size.Y;

            if (refX < minX) {
                // left of the emitter
                if (refY < minY) {
                    // left below the emitter
                    distance = Distance(minX, minY, refX, refY);
                }
                    //else if (refZ > maxZ)
                else if (refY > maxY) {
                    // left above the emitter
                    distance = Distance(minX, maxY, refX, refY);
                }
                else {
                    // left of the emitter
                    distance = minX - refX;
                }
            }
            else if (refX > maxX) {
                // right of the emitter
                if (refY < minY) {
                    // right below the emitter
                    distance = Distance(maxX, minY, refX, refY);
                }
                else if (refY > maxY) {
                    // right above the emitter
                    distance = Distance(maxX, maxY, refX, refY);
                }
                else {
                    // right of the emitter
                    distance = refX - maxX;
                }
            }
            else {
                if (refY < minY) {
                    // below the emitter
                    distance = minY - refY;
                }
                else if (refY > maxY) {
                    // above the emitter
                    distance = refY - maxY;
                }
                else {
                    // inside
                    distance = -1.0;
                }
            }

            return _function.Value(distance);
        }

        #endregion

        /// <summary>
        ///     Helper function to calculate the distance between two 2d points.
        /// </summary>
        /// <param name="pX"> pX </param>
        /// <param name="pY"> pY </param>
        /// <param name="qX"> qX </param>
        /// <param name="qY"> qY </param>
        /// <returns> the euklid distance </returns>
        private double Distance(double pX, double pY, double qX, double qY) {
            double x = pX - qX;
            double y = pY - qY;

            return Math.Sqrt(x*x + y*y);
        }
    }

}