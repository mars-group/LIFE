namespace ESCTestLayer.Implementation {
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using CommonTypes.TransportTypes;
    using Entities;
    using GenericAgentArchitectureCommon.Interfaces;
    using GeoAPI.Geometries;
    using Interface;
    using NetTopologySuite.Geometries.Utilities;

    public class UnboundESC : IUnboundESC {
        private readonly Random _rnd; // Number generator for random positions.
        private readonly IDictionary<ISpatialEntity, IGeometry> _entities;

        public UnboundESC() {
            _rnd = new Random();
            _entities = new ConcurrentDictionary<ISpatialEntity, IGeometry>();
        }

        #region IUnboundESC Members

        public MovementResult Add(ISpatialEntity entity, TVector position, TVector direction) {
            _entities[entity] = entity.Bounds;
            return Move(entity, position, direction);
        }

        public MovementResult AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid) {
            throw new NotImplementedException();
        }

        public void Remove(ISpatialEntity entity) {
            _entities.Remove(entity);
        }

        public MovementResult Update(ISpatialEntity entity) {
//      _entities[entity] = entity.GetBounds();
            throw new NotImplementedException();
        }

        public MovementResult Move(ISpatialEntity entity, TVector position, TVector direction) {
            IGeometry old = entity.Bounds;
            AffineTransformation trans = new AffineTransformation();
            trans.SetToTranslation(10, 10);

//          GeometricShapeFactory gsf2 = new GeometricShapeFactory(new GeometryFactory(new PrecisionModel(PrecisionModels.Floating), 4326));
//          gsf2.Centre = old.Centroid.Coordinate;
//          trans.SetToRotation(direction) //TODO vector to angle
            IGeometry result = trans.Transform(old);
            foreach (KeyValuePair<ISpatialEntity, IGeometry> geometry in _entities) {
                if (geometry.Value.Intersects(result)) {
                    Dictionary<string, object> dictionary =
                        new Dictionary<string, object>();
                    dictionary.Add("collision", geometry.Key);
                    return new MovementResult(false, convert(old.Centroid.Coordinate), dictionary);
                }
            }
            _entities[entity] = result;
            entity.Bounds = result;
            return new MovementResult(position);
        }

        public IEnumerable<ISpatialEntity> Explore
            (IGeometry geometry) {
            List<ISpatialEntity> entities = new List<ISpatialEntity>();
            foreach (KeyValuePair<ISpatialEntity, IGeometry> entity in _entities) {
                if (geometry.Intersects(entity.Value)) entities.Add(entity.Key);
            }
            return entities;
        }


        public IEnumerable<ISpatialEntity> ExploreAll() {
            return _entities.Keys;
        }

        public object GetData(int informationType, IDeprecatedGeometry deprecatedGeometry) {
            //TODO informationType als filter kriterium
//            const int elementId = -1;
//            Add(elementId, -1, false, geometry.GetDimensionQuad());
//            return Explore(elementId, geometry.GetPosition(), geometry.GetDirectionOfQuad());
            throw new NotImplementedException();
        }

        #endregion

        private TVector convert(Coordinate coordinate) {
            return new TVector((float) coordinate.X, (float) coordinate.Y, (float) coordinate.Z);
        }
    }
}