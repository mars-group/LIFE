using Hik.Communication.ScsServices.Service;
using LifeAPI.Layer;
using LifeAPI.Layer.Visualization;

namespace ExampleLayer
{
    [ScsService(Version = "0.1")]
    public interface IExampleLayer : ISteppedActiveLayer, IVisualizable
    {
    }
}
