using CommonTypes.DataTypes;

namespace ESCTestLayer.Entities
{
    using CommonTypes.TransportTypes;
    using LayerAPI.Interfaces;

    public struct SpatialPositionedEntity {

        public ISpatialEntity Entity;

        public TVector Position;

        public TVector Direction;
       
    }
}
