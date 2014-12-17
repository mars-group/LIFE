using System;
using System.Collections.Generic;
using System.Linq;
using EnvironmentServiceComponent.Entities.Shape;
using GeoAPI.Geometries;
using LifeAPI.Perception;
using LifeAPI.Spatial;
using NetTopologySuite.Geometries;
using NetTopologySuite.Geometries.Utilities;

namespace EnvironmentServiceComponent.Implementation {

    public class GeometryESC : ACollisionESC {
        private const int MaxAttempsToAddRandom = 100;
        private readonly Random _random;
        private readonly List<ISpatialEntity> _entities;

        public GeometryESC() {
            _random = new Random();
            _entities = new List<ISpatialEntity>();
        }

        public override bool Add(ISpatialEntity entity, TVector position, TVector rotation = default(TVector)) {
            GeometryShape geometryShape = entity.Shape as GeometryShape;
            if (geometryShape != null) {
                IGeometry oldGeometry = geometryShape.Geometry;
                IPoint oldCentroid = oldGeometry.Centroid;

                if (!oldCentroid.Equals(new Point(0, 0, 0))) {
                    // move to origin
                    AffineTransformation trans = new AffineTransformation();
                    trans.SetToTranslation(-oldCentroid.X, -oldCentroid.Y);
                    IGeometry newGeometry = trans.Transform(oldGeometry);
                    geometryShape.Geometry = newGeometry;
                }

                //move to position
                MovementResult result = Move(entity, position, rotation);
                if (!result.Success) {
                    geometryShape.Geometry = oldGeometry;
                    return false;
                }
            }
            _entities.Add(entity);
            return true;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            for (int attempt = 0; attempt < MaxAttempsToAddRandom; attempt++) {
                TVector position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) {
                    return true;
                }
            }
            return false;
        }

        public override void Remove(ISpatialEntity entity) {
            _entities.Remove(entity);
        }

        public override bool Resize(ISpatialEntity entity, IShape shape) {
            GeometryShape geometryShape = shape as GeometryShape;
            if (geometryShape == null) {
                return false;
            }
            List<ISpatialEntity> result = Explore(new ExploreSpatialObject(geometryShape.Geometry, entity.GetCollisionType())).ToList();
            result.Remove(entity);
            if (!result.Any()) {
                entity.Shape = geometryShape;
                return true;
            }
            return false;
        }

        public override MovementResult Move
            (ISpatialEntity entity, TVector movementVector, TVector rotation = default(TVector)) {
            GeometryShape geometryShape = entity.Shape as GeometryShape;
            if (geometryShape == null) {
                throw new NotImplementedException();
            }

            IGeometry old = geometryShape.Geometry;
            AffineTransformation trans = new AffineTransformation();
            trans.Translate(movementVector.X, movementVector.Y);
            if (!EqualityComparer<TVector>.Default.Equals(rotation, default(TVector))) {
                Direction direction = new Direction();
                direction.SetDirectionalVector(rotation.GetVector());
                Coordinate center = old.Centroid.Coordinate;
                trans.Rotate(Direction.DegToRad(direction.Yaw), center.X, center.Y);
            }

            IGeometry result = trans.Transform(old);

            List<ISpatialEntity> collisions = Explore(new ExploreSpatialObject(result, entity.GetCollisionType())).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) {
                return new MovementResult(collisions);
            }
            geometryShape.Geometry = result;
            return new MovementResult();
        }

        public override IEnumerable<ISpatialEntity> Explore(ISpatialObject spatial) {
            GeometryShape exploreShape = spatial.Shape as GeometryShape;
            if (exploreShape == null) {
                throw new NotImplementedException();
            }

            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            foreach (ISpatialEntity entity in _entities.ToArray()) {
                GeometryShape entityShape = entity.Shape as GeometryShape;
                if (entityShape == null) {//TODO
                    throw new NotImplementedException();
                }
                if (exploreShape.Geometry.Envelope.Intersects(entityShape.Geometry)) {
                    int givenCollisionType = spatial.GetCollisionType().GetHashCode();
                    int foundCollisionType = entity.GetCollisionType().GetHashCode();
                    if (Collides(givenCollisionType, foundCollisionType)) {
                        entities.Add(entity);
                    }
                }
            }
            return entities;
        }

        public override IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.ToList();
        }

        public override object GetData(ISpecification spec) {
            ISpatialObject spatialObject = spec as ISpatialObject;
            if (spatialObject != null) {
                return Explore(spatialObject);
            }
            return null;
        }

        #region private methods

        private TVector GenerateRandomPosition(TVector min, TVector max, bool grid) {
            if (grid) {
                int x = _random.Next((int) min.X, (int) max.X + 1);
                int y = _random.Next((int) min.Y, (int) max.Y + 1);
                int z = _random.Next((int) min.Z, (int) max.Z + 1);
                return new TVector(x, y, z);
            }
            else {
                double x = GetRandomNumber(min.X, max.X);
                double y = GetRandomNumber(min.Y, max.Y);
                double z = GetRandomNumber(min.Z, max.Z);
                return new TVector(x, y, z);
            }
        }

        private double GetRandomNumber(double min, double max) {
            return _random.NextDouble()*(max - min) + min;
        }

        #endregion

        #region Nested type: ExploreSpatialObject

        private class ExploreSpatialObject : ISpatialObject {
            private readonly Enum _collisionType;

            public ExploreSpatialObject(IGeometry geometry, Enum collisionType) {
                _collisionType = collisionType;
                Shape = new ExploreShape(geometry);
            }

            #region ISpatialObject Members

            public IShape Shape { get; set; }

            public Enum GetInformationType() {
                throw new NotImplementedException();
            }

            public Enum GetCollisionType() {
                return _collisionType;
            }

            #endregion
        }

        #endregion
    }

}