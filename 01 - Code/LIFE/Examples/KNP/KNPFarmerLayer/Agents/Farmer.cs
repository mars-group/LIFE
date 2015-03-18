using System;
using GeoAPI.Geometries;
using KNPElevationLayer;
using KNPEnvironmentLayer;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;
using TreeLayer;

namespace KNPFarmerLayer.Agents
{
    class Farmer : IFarmer
    {
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly IKNPEnvironmentLayer _environmentLayer;
        private readonly IKnpTreeLayer _treeLayer;
        private Coordinate _imageCoordinates;

        public Farmer(IKnpElevationLayer elevationLayer, IKNPEnvironmentLayer environmentLayer, IKnpTreeLayer treeLayer, Guid id, double lat, double lon)
        {
            _elevationLayer = elevationLayer;
            _environmentLayer = environmentLayer;
            _treeLayer = treeLayer;
            ID = id;

            _imageCoordinates = elevationLayer.TransformToImage(lat, lon);
            SpatialEntity = new SpatialFarmerEntity(_imageCoordinates.X, _imageCoordinates.Y, id);
        }

        public ISpatialEntity SpatialEntity { get; set; }

        public void Tick() {
            throw new NotImplementedException();
        }

        public Guid ID { get; set; }

        private class SpatialFarmerEntity : ISpatialEntity
        {

            public SpatialFarmerEntity(double x, double y, Guid id)
            {
                Shape = new Cuboid(new Vector3(1, 1, 1), new Vector3(x, y));
                AgentGuid = id;
            }

            public IShape Shape { get; set; }

            public Enum CollisionType
            {
                get { return SpatialAPI.Entities.Movement.CollisionType.MassiveAgent; }
            }

            public Guid AgentGuid { get; set; }
        }
    }
}
