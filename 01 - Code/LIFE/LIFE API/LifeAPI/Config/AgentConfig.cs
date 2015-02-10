
namespace LifeAPI.Config
{
    /// <summary>
    /// An agent configuration providing information about the agent's name and amount
    /// </summary>
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

        /// <summary>
        /// Basic constructor. Creates 0 "noname" agents.
        /// </summary>
        public AgentConfig() {
            AgentName = "noname";
            AgentCount = 0;
        }

        /// <summary>
        /// Create a new AgentConfig
        /// </summary>
        /// <param name="agentName">The agent's class name.</param>
        /// <param name="agentCount">The amount of agents to simulate.</param>
        public AgentConfig(string agentName, int agentCount)
        {
            AgentName = agentName;
            AgentCount = agentCount;
        }
    }
}
