using Hik.Communication.ScsServices.Service;
using LifeAPI.Layer.GIS;

namespace KNPElevationLayer
{
    [ScsService(Version = "0.1")]
    public interface IKnpElevationLayer : IGISActiveLayer
    {
        string Name { get; }
    }
}
