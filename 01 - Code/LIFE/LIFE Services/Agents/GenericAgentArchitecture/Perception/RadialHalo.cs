namespace GenericAgentArchitecture.Perception {
    #region Namespace imports

    using System;
    using CommonTypes.TransportTypes;
    using LayerAPI.Interfaces;
    using Movement;

    #endregion

    /// <summary>
    ///     A halo capable of sensing in a circle around its position.
    /// </summary>
    public class RadialHalo : Halo {
        private readonly float _radius; // The radius describing the range of this halo.
        private readonly AABB _aabb;


        /// <summary>
        ///     Create a circular halo.
        /// </summary>
        /// <param name="position">The agent's centre.</param>
        /// <param name="radius">The radius describing the range of this halo.</param>
        public RadialHalo(Vector position, float radius) : base(position) {
            _radius = radius;

            var min = new TVector(position.X - radius, position.Y - radius, position.Z - radius);
            var max = new TVector(position.X + radius, position.Y + radius, position.Z + radius);
            _aabb = AABB.FromMinMax(min, max);
        }

        public override AABB GetAABB() {
            return _aabb;
        }

        public override bool IsInRange(TVector position) {
            return Position.GetTVector().GetDistance(position) <= _radius;
        }
    }
}