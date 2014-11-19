namespace ESCTestLayer.Implementation {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonTypes.TransportTypes;
    using Entities;
    using GenericAgentArchitectureCommon.Datatypes;
    using GenericAgentArchitectureCommon.Interfaces;
    using GeoAPI.Geometries;
    using Interface;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Utilities;
    using NetTopologySuite.Utilities;

    public class UnboundESC : IUnboundESC { //TODO löschen funktioniert nicht mehr wegen MovementResult
        private const int MaxAttemps = 100;
        private readonly Random _random; // Number generator for random positions.
        private readonly List<ISpatialEntity> _entities;

        public UnboundESC() {
            _random = new Random();
            _entities = new List<ISpatialEntity>();
        }

        #region IUnboundESC Members

        public bool Add(ISpatialEntity entity, TVector position, float directionAngle = 0) {
            MovementResult result = Move(entity, position, directionAngle);
            if (result.Success) _entities.Add(entity);
            return result.Success;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            for (int attempt = 0; attempt < MaxAttemps; attempt++) {
                TVector position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) return true;
            }
            return false;
        }

        public void Remove(ISpatialEntity entity) {
            _entities.Remove(entity);
        }

        public bool Resize(ISpatialEntity entity, IGeometry newBounds) {
            if (Explore(newBounds).Any()) {
                entity.Geometry = newBounds;
                return true;
            }
            return false;
        }

        public MovementResult Move(ISpatialEntity entity, TVector movementVector, float directionAngle = 0) {
            IGeometry old = entity.Geometry;
            AffineTransformation trans = new AffineTransformation();
            trans.SetToTranslation(movementVector.X, movementVector.Y);

            GeometricShapeFactory gsf2 = new GeometricShapeFactory();
            var center = old.Centroid.Coordinate;
            trans.Rotate(directionAngle, center.X, center.Y);
            IGeometry result = trans.Transform(old);

            List<ISpatialEntity> collisions = Explore(result).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) return new MovementResult(collisions);

            entity.Geometry = result;
            return new MovementResult();
        }

        public IEnumerable<ISpatialEntity> Explore(IGeometry geometry) {
            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            foreach (ISpatialEntity entity in _entities) {
                if (geometry.Envelope.Intersects(entity.Geometry) && !geometry.Touches(entity.Geometry))
                    entities.Add(entity);
            }
            return entities;
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities;
        }

        public object GetData(ISpecificator spec) {
            SpatialHalo halo = spec as SpatialHalo;
            if (halo != null) return Explore(halo.Geometry);
            return null;
        }

        #endregion

        #region private methods

        private TVector GenerateRandomPosition(TVector min, TVector max, bool grid) {
            if (grid) {
                int x = _random.Next((int) min.X, (int) max.X + 1);
                int y = _random.Next((int) min.Y, (int) max.Y + 1);
                int z = _random.Next((int) min.Z, (int) max.Z + 1);
                return new TVector(x, y, z);
            }
            else {
                float x = (float) GetRandomNumber(min.X, max.X);
                float y = (float) GetRandomNumber(min.Y, max.Y);
                float z = (float) GetRandomNumber(min.Z, max.Z);
                return new TVector(x, y, z);
            }
        }

        private double GetRandomNumber(double min, double max) {
            return _random.NextDouble()*(max - min) + min;
        }

        #endregion
    }
}