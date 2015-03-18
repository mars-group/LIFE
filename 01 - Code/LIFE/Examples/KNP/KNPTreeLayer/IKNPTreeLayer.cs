using System;
using Hik.Communication.ScsServices.Service;
using LifeAPI.Layer;
using TreeLayer.Agents;

namespace TreeLayer
{
    [ScsService(Version = "0.1")]
    public interface IKnpTreeLayer : ISteppedLayer {
        double ChopTree(Guid id);
        ITree GetTreeById(Guid id);
    }
}
