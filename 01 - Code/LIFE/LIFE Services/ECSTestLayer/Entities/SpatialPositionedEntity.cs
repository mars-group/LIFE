using CommonTypes.TransportTypes;
using GenericAgentArchitectureCommon.Interfaces;

namespace ESCTestLayer.Entities {

  public struct SpatialPositionedEntity {

    public ISpatialEntity Entity;

    public TVector Position;

    public TVector Direction;
  }
}