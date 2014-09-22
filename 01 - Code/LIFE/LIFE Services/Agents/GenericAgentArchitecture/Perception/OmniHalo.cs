using CommonTypes.DataTypes;

namespace GenericAgentArchitecture.Perception {
    /// <summary>
    ///     A dummy halo. May be used as a stub for sensors with no perception limitation.
    /// </summary>
    internal class OmniHalo : Halo {
        /// <summary>
        ///     Create a halo that is capable of sensing everything.
        /// </summary>
        public OmniHalo() : base(Vector.Null) { }


        public override Vector GetDirectionOfQuad() {
            return Vector.UnitVectorXAxis;
        }


        public override Vector GetDimensionQuad() {
            return Vector.MaxVector;
        }


        /// <summary>
        ///     Check, if a given position is inside this perception range.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns>Always true.</returns>
        public override bool IsInRange(Vector position)
        {
            return true;
        }
    }
}