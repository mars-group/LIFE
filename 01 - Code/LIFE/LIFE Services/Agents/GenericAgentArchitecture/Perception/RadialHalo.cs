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
        public RadialHalo(Vector3f position, float radius) : base(position) {
            _radius = radius;
        }


        public override Vector3f GetDirectionOfQuad() {
            return Vector3f.UnitVectorXAxis;
        }

        public override Vector3f GetDimensionQuad() {
            return new Vector3f(_radius*2, _radius*2, _radius*2);
        }

        public override bool IsInRange(Vector3f position) {
            return Position.GetDistance(position) <= _radius;
        }
    }
}