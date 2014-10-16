using System.Collections.Generic;
using ESCTestLayer.Entities;
using LayerAPI.Interfaces;

namespace ESCTestLayer.Interface
{
    using CommonTypes.TransportTypes;

    public interface IOctreeESC : IGenericDataSource
    {
        MovementResult Add(ISpatialEntity entity, TVector position, TVector direction);

        MovementResult AddWithRandomPosition(ISpatialEntity entity, TVector min, TVector max, bool grid);

        void Remove(ISpatialEntity entity);

        MovementResult Update(ISpatialEntity entity);

        MovementResult Move(ISpatialEntity entity, TVector position, TVector direction);
        
        IEnumerable<SpatialPositionedEntity> Explore(TVector dimension, TVector position, TVector direction);

        IEnumerable<SpatialPositionedEntity> ExploreAll();
    }
}
