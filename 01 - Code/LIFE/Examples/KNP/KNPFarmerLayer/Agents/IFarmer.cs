using ASC.Communication.ScsServices.Service;
using LifeAPI.Agent;
using SpatialAPI.Entities;

namespace KNPFarmerLayer.Agents
{
    [AscService(Version = "0.1")]
    public interface IFarmer : IAgent {
        ISpatialEntity SpatialEntity { get; set; }
    }
}
