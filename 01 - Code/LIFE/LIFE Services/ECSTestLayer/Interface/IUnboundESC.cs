using System.Collections.Generic;
using ESCTestLayer.Entities;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Interface {
    using GenericAgentArchitectureCommon.TransportTypes;
    using GeoAPI.Geometries;

    public interface IUnboundESC : IGenericDataSource {

    bool Add(ISpatialEntity entity, TVector position, TVector direction = default(TVector));

    bool AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid);

    void Remove(ISpatialEntity entity);

    bool Resize(ISpatialEntity entity, IGeometry newBounds);

    MovementResult Move(ISpatialEntity entity, TVector movementVector, TVector direction = default(TVector));

    IEnumerable<ISpatialEntity> Explore(IGeometry geometry);

    IEnumerable<ISpatialEntity> ExploreAll();
  }
}