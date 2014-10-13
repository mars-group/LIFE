using System.Collections.Generic;
using CommonTypes.DataTypes;
using ESCTestLayer.Entities;
using LayerAPI.Interfaces;

namespace ESCTestLayer.Interface
{
    public interface IEnvironmentServiceComponent : IGenericDataSource
    {
        MovementResult Add(ISpatialEntity entity, Vector position, Vector direction);

        MovementResult AddWithRandomPosition(ISpatialEntity entity, Vector min, Vector max, bool grid);

        void Remove(ISpatialEntity entity);

        MovementResult Update(ISpatialEntity entity);

        MovementResult Move(ISpatialEntity entity, Vector position, Vector direction);
        
        IEnumerable<SpatialPositionedEntity> Explore(Vector dimension, Vector position, Vector direction);

        IEnumerable<SpatialPositionedEntity> ExploreAll();
    }
}
