using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ESCTestLayer.Entities;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Interface {
    using GeoAPI.Geometries;

    public interface IUnboundESC : IGenericDataSource {

    bool Add(ISpatialEntity entity, TVector position, float directionAngle = 0);

    bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid);

    void Remove(ISpatialEntity entity);

    bool Update(ISpatialEntity entity, IGeometry newBounds);

    MovementResult Move(ISpatialEntity entity, TVector movementVector, float directionAngle = 0);

    IEnumerable<ISpatialEntity> Explore(IGeometry geometry);

    IEnumerable<ISpatialEntity> ExploreAll();
  }
}