namespace GenericAgentArchitecture.Perception
{
    using LayerAPI.Interfaces;
    using Movement;

    public class SphereSpatialEntity : ISpatialEntity
    {
        private readonly RadialHalo _halo;

        public SphereSpatialEntity(Vector position, float radius) {
            _halo = new RadialHalo(position, radius);
        }

        public IGeometry GetBounds() {
            return _halo;
        }
    }
}
