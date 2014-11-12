namespace ESCTestLayer.Implementation {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CommonTypes.TransportTypes;
    using Entities;
    using GenericAgentArchitectureCommon.Interfaces;
    using GeoAPI.Geometries;
    using Interface;
    using NetTopologySuite.Geometries.Utilities;

    public class UnboundESC : IUnboundESC {
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
            return result.Success;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            for (int attemp = 0; attemp < MaxAttemps; attemp++) {
                TVector position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) return true;
            }
            return false;
        }

        public void Remove(ISpatialEntity entity) {
            _entities.Remove(entity);
        }

        public bool Update(ISpatialEntity entity, IGeometry newBounds) {
            if (Explore(newBounds).Any()) {
                entity.Bounds = newBounds;
                return true;
            }
            return false;
        }

        public MovementResult Move(ISpatialEntity entity, TVector movementVector, float directionAngle = 0) {
            IGeometry old = entity.Bounds;
            AffineTransformation trans = new AffineTransformation();
            trans.SetToTranslation(10, 10);

//          GeometricShapeFactory gsf2 = new GeometricShapeFactory(new GeometryFactory(new PrecisionModel(PrecisionModels.Floating), 4326));
//          gsf2.Centre = old.Centroid.Coordinate;
//          trans.SetToRotation(direction) //TODO vector to angle
            IGeometry result = trans.Transform(old);

            List<ISpatialEntity> collisions = Explore(result).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) return new MovementResult(false, collisions);

            entity.Bounds = result;
            return new MovementResult(convert(result.Centroid.Coordinate));
        }

        public IEnumerable<ISpatialEntity> Explore(IGeometry geometry) {
            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            foreach (ISpatialEntity entity in _entities) {
                if (geometry.Intersects(entity.Bounds)) entities.Add(entity);
            }
            return entities;
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities;
        }

        public object GetData(int informationType, IDeprecatedGeometry deprecatedGeometry) {
            //TODO informationType als filter kriterium
//            const int elementId = -1;
//            Add(elementId, -1, false, geometry.GetDimensionQuad());
//            return Explore(elementId, geometry.GetPosition(), geometry.GetDirectionOfQuad());
            throw new NotImplementedException();
        }

        #endregion

        #region private methods

        private TVector GenerateRandomPosition(TVector min, TVector max, bool grid) {
            if (grid) {
                int x = _random.Next((int) min.X, (int) max.X);
                int y = _random.Next((int) min.Y, (int) max.Y);
                int z = _random.Next((int) min.Z, (int) max.Z);
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

        private TVector convert(Coordinate coordinate) {
            return new TVector((float) coordinate.X, (float) coordinate.Y, (float) coordinate.Z);
        }

        #endregion
    }
}