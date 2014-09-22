using CommonTypes.DataTypes;

namespace GenericAgentArchitecture.Perception {
    /// <summary>
    ///     A halo capable of sensing in a circle around its position.
    /// </summary>
    public class RadialHalo : Halo {
        private readonly float _radius; // The radius describing the range of this halo.


        /// <summary>
        ///     Create a circular halo.
        /// </summary>
        /// <param name="position">The agent's centre.</param>
        /// <param name="radius">The radius describing the range of this halo.</param>
        public RadialHalo(Vector position, float radius) : base(position) {
            _radius = radius;
        }


        public override Vector GetDirectionOfQuad() {
            return Vector.UnitVectorXAxis;
        }

        public override Vector GetDimensionQuad() {
            return new Vector(_radius*2, _radius*2, _radius*2);
        }

        public override bool IsInRange(Vector position) {
            return Position.GetDistance(position) <= _radius;
        }
    }
}