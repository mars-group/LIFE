﻿namespace ESCTestLayer.Implementation {
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Entities;
    using GenericAgentArchitectureCommon.Datatypes;
    using GenericAgentArchitectureCommon.Interfaces;
    using GenericAgentArchitectureCommon.TransportTypes;
    using GeoAPI.Geometries;
    using Interface;
    using NetTopologySuite.Geometries;
    using NetTopologySuite.Geometries.Utilities;

    public class UnboundESC : IUnboundESC {
        private const int MaxAttempsToAddRandom = 100;
        private readonly Random _random;
        private readonly List<ISpatialEntity> _entities;

        public UnboundESC() {
            _random = new Random();
            _entities = new List<ISpatialEntity>();
        }

        #region IUnboundESC Members

        public bool Add(ISpatialEntity entity, TVector position, TVector rotation = default(TVector)) {
            IGeometry oldGeometry = entity.Geometry;
            IPoint oldCentroid = oldGeometry.Centroid;

            if (!oldCentroid.Equals(new Point(0, 0, 0))) {
                // move to origin
                AffineTransformation trans = new AffineTransformation();
                trans.SetToTranslation(-oldCentroid.X, -oldCentroid.Y);
                IGeometry newGeometry = trans.Transform(oldGeometry);
                entity.Geometry = newGeometry;
            }

            //move to position
            MovementResult result = Move(entity, position, rotation);
            if (!result.Success) {
                entity.Geometry = oldGeometry;
                return false;
            }
            _entities.Add(entity);
            return true;
        }

        public bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            for (int attempt = 0; attempt < MaxAttempsToAddRandom; attempt++) {
                TVector position = GenerateRandomPosition(min, max, grid);
                bool result = Add(entity, position);
                if (result) return true;
            }
            return false;
        }

        public void Remove(ISpatialEntity entity) {
            _entities.Remove(entity);
        }

        public bool Resize(ISpatialEntity entity, IGeometry newGeometry) {
            List<ISpatialEntity> result = Explore(newGeometry).ToList();
            result.Remove(entity);
            if (!result.Any()) {
                entity.Geometry = newGeometry;
                return true;
            }
            return false;
        }

        public MovementResult Move(ISpatialEntity entity, TVector movementVector, TVector rotation = default(TVector)) {
            IGeometry old = entity.Geometry;
            AffineTransformation trans = new AffineTransformation();

            trans.SetToTranslation(movementVector.X, movementVector.Y);
            if (!EqualityComparer<TVector>.Default.Equals(rotation, default(TVector))) {
                Direction direction = new Direction();
                direction.SetDirectionalVector(rotation.GetVector());
                Console.WriteLine(direction.Yaw);
                Coordinate center = old.Centroid.Coordinate;
                trans.Rotate(Direction.DegToRad(direction.Yaw), center.X, center.Y);
            }


            IGeometry result = trans.Transform(old);

            List<ISpatialEntity> collisions = Explore(result).ToList();
            collisions.Remove(entity);
            if (collisions.Any()) return new MovementResult(collisions);

            entity.Geometry = result;
            return new MovementResult();
        }

        public IEnumerable<ISpatialEntity> Explore(IGeometry geometry) {
            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            IGeometryFactory gfactory = GeometryFactory.FloatingSingle;
            foreach (ISpatialEntity entity in _entities.ToArray()) {
                IGeometry poly1 = gfactory.CreatePolygon(geometry.Envelope.Coordinates);
                IGeometry poly2 = gfactory.CreatePolygon(entity.Geometry.Coordinates);
                if (poly1.Intersects(poly2) && !poly1.Touches(poly2) &&
                    poly1.Intersection(poly2).OgcGeometryType.Equals(OgcGeometryType.Polygon)) entities.Add(entity);
            }
            return entities;
        }

        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.ToList();
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
    }
}