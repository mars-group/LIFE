using System;
using PedestrianModel.Util.Math;
using SpatialCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Movement.Potential.Emitter {

    public class PointEmitter : IPotentialEmitter {
        /// <returns> the position </returns>
        public Vector Position { get { return _position; } }

        /// <returns> the function </returns>
        public IUnivariateRealFunction Function { get { return _function; } }

        /// <summary>
        ///     The position of this point emitter.
        /// </summary>
        private readonly Vector _position;

        /// <summary>
        ///     The function to apply the distance to.
        /// </summary>
        private readonly IUnivariateRealFunction _function;

        /// <summary>
        ///     Creates a new Point emitter at the position <code>position</code>. The potential will be calculated by
        ///     applying the euklid distance between the emitters position and the given position to the given
        ///     <seealso cref="IUnivariateRealFunction" />.<br />
        ///     <br />
        ///     The euklid distance between the two points will only be calculated 2D (x and z axis).
        /// </summary>
        /// <param name="position"> the position of the emitter </param>
        /// <param name="function"> the function to calculate the potential with </param>
        public PointEmitter(Vector position, IUnivariateRealFunction function) {
            _position = position;
            _function = function;
        }

        #region PotentialEmitter Members

        public double GetPotential(Vector reference) {
            double x = _position.X - reference.X;
            double y = _position.Y - reference.Y;

            return _function.Value(Math.Sqrt(x*x + y*y));
        }

        #endregion
    }

}