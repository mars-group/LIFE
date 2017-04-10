using System.Collections.Generic;
using LIFE.API.Results;

namespace ResultAdapter.Implementation.DataOutput
{
    /// <summary>
    ///   Output functions required from the database connectors.
    /// </summary>
    public interface IResultWriter
    {
        /// <summary>
        ///   Write the legacy ('ISimResult') result data to the storage.
        /// </summary>
        /// <param name="results">Agent result listings.</param>
        void WriteLegacyResults(IEnumerable<AgentSimResult> results);


        /// <summary>
        ///   Add agent meta data entries.
        /// </summary>
        /// <param name="metadata">Set of meta data information to write.</param>
        void AddMetadataEntries(IEnumerable<AgentMetadataEntry> metadata);


        /// <summary>
        ///   Mark agents as removed by setting their meta data deletion flag.
        /// </summary>
        /// <param name="agentIds">List of agents that were deleted.</param>
        /// <param name="delTick">Tick of deletion.</param>
        void SetAgentDeletionFlags(IEnumerable<string> agentIds, int delTick);


        /// <summary>
        ///   Write agent result data to the storage.
        /// </summary>
        /// <param name="results">A list of agent frames.</param>
        /// <param name="isKeyframe">Set to 'true' on keyframes, 'false' on delta frames.</param>
        void WriteAgentFrames(IEnumerable<AgentFrame> results, bool isKeyframe);
    }
}