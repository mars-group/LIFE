using System.Collections.Generic;
using CommonTypes.TransportTypes;
using ESCTestLayer.Entities;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Interface {
    using GeoAPI.Geometries;

    public interface IUnboundESC : IGenericDataSource {

    MovementResult Add(ISpatialEntity entity, TVector position, TVector direction);

    MovementResult AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid);

    void Remove(ISpatialEntity entity);

    MovementResult Update(ISpatialEntity entity);

    MovementResult Move(ISpatialEntity entity, TVector position, TVector direction);

    IEnumerable<ISpatialEntity> Explore(IGeometry geometry);

    IEnumerable<ISpatialEntity> ExploreAll();
  }
}