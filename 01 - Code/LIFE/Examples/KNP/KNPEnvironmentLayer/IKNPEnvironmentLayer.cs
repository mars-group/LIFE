using Hik.Communication.ScsServices.Service;
using LifeAPI.Layer;
using SpatialAPI.Environment;

namespace KNPEnvironmentLayer
{
    [ScsService(Version = "0.1")]
    public interface IKNPEnvironmentLayer : ISteppedLayer, IEnvironment
    {
        void Nothing();
    }
}
