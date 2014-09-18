using CommonTypes.DataTypes;

namespace GenericAgentArchitecture.Perception {
    /// <summary>
    ///     A dummy halo. May be used as a stub for sensors with no perception limitation.
    /// </summary>
    internal class OmniHalo : Halo {
        /// <summary>
        ///     Create a halo that is capable of sensing everything.
        /// </summary>
        public OmniHalo() : base(null) {}


        public override Vector3f GetDirectionOfQuad() {
            return Vector3f.UnitVectorXAxis;
        }


        public override Vector3f GetDimensionQuad() {
            return Vector3f.MaxVector;
        }


        /// <summary>
        ///     Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>Always true.</returns>
        public override bool IsInRange(Vector3f position)
        {
            return true;
        }
    }
}