using System;
using GeoAPI.Geometries;
using KNPElevationLayer;
using KNPEnvironmentLayer;
using SpatialAPI.Entities;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;
using TreeLayer;
using TreeLayer.Agents;

namespace KNPFarmerLayer.Agents
{
    [Serializable]
    class Farmer : IFarmer
    {
        private readonly IKnpElevationLayer _elevationLayer;
        private readonly IKNPEnvironmentLayer _environmentLayer;
        private readonly IKnpTreeLayer _treeLayer;
        private Coordinate _imageCoordinates;
        private SpatialFarmerEntity _explorationEntity;
        private double _myBiomass;

        public Farmer(IKnpElevationLayer elevationLayer, IKNPEnvironmentLayer environmentLayer, IKnpTreeLayer treeLayer, Guid id, double lat, double lon)
        {
            _elevationLayer = elevationLayer;
            _environmentLayer = environmentLayer;
            _treeLayer = treeLayer;
            ID = id;

            _myBiomass = 0;

            _imageCoordinates = elevationLayer.TransformToImage(lat, lon);
            SpatialEntity = new SpatialFarmerEntity(_imageCoordinates.X, _imageCoordinates.Y, id);

            _explorationEntity = new SpatialFarmerEntity(_imageCoordinates.X, _imageCoordinates.Y, Guid.NewGuid());
            _explorationEntity.Shape = new Cuboid(new Vector3(500, 500, 500), new Vector3(_imageCoordinates.X, _imageCoordinates.Y));
        }

        public ISpatialEntity SpatialEntity { get; set; }

        public void Tick() {
            var result = _environmentLayer.Explore(_explorationEntity);
            
            foreach (var spatialEntity in result)
            {
                if (spatialEntity.AgentType != typeof (Tree)) continue;
                ITree tree;
                if (_treeLayer.GetTreeById(spatialEntity.AgentGuid, out tree))
                {
                    if (tree.Biomass > 10)
                    {
                        _myBiomass += _treeLayer.ChopTree(tree.ID);
                    }
                }
            }
        }

        public Guid ID { get; set; }

        [Serializable]
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

            public Type AgentType { get; set; }
            
        }
    }
}
