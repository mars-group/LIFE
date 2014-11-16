using LayerAPI.Interfaces.GIS;

namespace LayerAPI.Interfaces
{
    /// <summary>
    /// An active GIS layer, which allows to load and access GIS data
    /// as well as hold agents.
    /// This layer gets ticked via PreTick, Tick and PostTick.
    /// </summary>
    public interface IGISActiveLayer : ISteppedActiveLayer, IGISAccess {
    }
}
