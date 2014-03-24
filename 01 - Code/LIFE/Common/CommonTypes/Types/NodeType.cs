using ProtoBuf;

namespace CommonTypes.Types
{

    [ProtoContract]
    public enum NodeType
    {
        LayerContainer,
        SimulationManager,
        SimulationController,

    }
}
