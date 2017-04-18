using System.Collections.Generic;
using LIFE.API.Results;

namespace ResultAdapter.Implementation.DataOutput
{
    /// <summary>
    ///   Adapter for the Kafka/Cassandra storage pipeline developed by Janus.
    /// </summary>
    public class KafkaWriter : IResultWriter
    {
        private readonly string _simId; // Simulation identifier.


        /// <summary>
        ///   Create a new Kafka connector.
        /// </summary>
        /// <param name="initParams">Initialization settings.</param>
        public KafkaWriter(IDictionary<string, string> initParams,
            IEnumerable<LoggerConfig> loggerDef) {

            _simId = initParams["SimulationId"];
            //TODO
            // ...
        }


        public void WriteLegacyResults(IEnumerable<AgentSimResult> results)
        {
            //TODO To be implemented.
        }

        public void AddMetadataEntries(IEnumerable<AgentMetadataEntry> metadata)
        {
            //TODO To be implemented.
        }

        public void SetAgentDeletionFlags(IEnumerable<string> agentIds, int delTick)
        {
            //TODO To be implemented.
        }

        public void WriteAgentFrames(IEnumerable<AgentFrame> results, bool isKeyframe)
        {
            //TODO To be implemented.
        }
    }
}