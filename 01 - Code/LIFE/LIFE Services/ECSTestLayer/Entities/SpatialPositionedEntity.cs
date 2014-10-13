using CommonTypes.DataTypes;

namespace ESCTestLayer.Entities
{
    using LayerAPI.Interfaces;

    public struct SpatialPositionedEntity {

        public ISpatialEntity Entity;

        public Vector Position;

        public Vector Direction;
    }
}
