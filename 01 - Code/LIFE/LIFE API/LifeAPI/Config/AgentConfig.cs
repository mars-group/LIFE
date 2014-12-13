
namespace LifeAPI.Config
{
    public class AgentConfig
    {
        /// <summary>
        /// The name of the agent
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// The amount of agents to use in the simulation
        /// </summary>
        public int AgentCount { get; set; }

        public AgentConfig(string agentName, int agentCount)
        {
            AgentName = agentName;
            AgentCount = agentCount;
        }
    }
}
