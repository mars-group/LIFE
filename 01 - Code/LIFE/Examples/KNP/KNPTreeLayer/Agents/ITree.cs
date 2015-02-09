using ASC.Communication.ScsServices.Service;
using LifeAPI.Agent;

namespace TreeLayer.Agents
{
    [AscService(Version = "0.1")]
    public interface ITree : IAgent, ICacheable
    {
        double Height { get; set; }
        double Diameter { get; set; }
        double CrownDiameter { get; set; }
        double Age { get; set; }
        double Biomass { get; set; }
        double Lat { get; set; }
        double Lon { get; set; }

        string GetIdentifiaction();
    }
}
