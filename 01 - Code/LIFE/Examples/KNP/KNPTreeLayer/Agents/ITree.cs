using LifeAPI.Agent;

namespace TreeLayer.Agents
{
    interface ITree : IAgent
    {
        double Height { get; set; }
        double Diameter { get; set; }
        double CrownDiameter { get; set; }
        double Age { get; set; }
        double Biomass { get; set; }
        double Lat { get; set; }
        double Lon { get; set; }
    }
}
