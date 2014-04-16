using System;
using ProtoBuf;

namespace CommonTypes.Types {
    [ProtoContract]
    [Serializable]
    public enum NodeType {
        LayerContainer = 0,
        SimulationManager = 1,
        SimulationController = 2,
    }
}