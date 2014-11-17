using LayerAPI.Interfaces.GIS;

namespace LayerAPI.Interfaces
{
    /// <summary>
    /// A passive GIS layer, which allows to load and access GIS data
    /// as well as hold agents.
    /// </summary>
    public interface IGISPassiveLayer : ISteppedLayer, IGISAccess {
    }
}
