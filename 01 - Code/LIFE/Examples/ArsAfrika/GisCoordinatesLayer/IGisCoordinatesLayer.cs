using Hik.Communication.ScsServices.Service;
using LifeAPI.Layer;
using LifeAPI.Layer.GIS;

namespace GisCoordinatesLayer {

    [ScsService(Version = "0.1")]
    public interface IGisCoordinatesLayer : IGISActiveLayer
    {
        string Name { get; }
    }

}