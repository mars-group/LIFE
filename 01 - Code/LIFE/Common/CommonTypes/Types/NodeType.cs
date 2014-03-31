using System.ComponentModel;
using ProtoBuf;

namespace CommonTypes.Types
{

    [ProtoContract]
    public enum NodeType
    {
        LayerContainer = 0,
        SimulationManager = 1,
        SimulationController = 2,

    }
}
