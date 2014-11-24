using GenericAgentArchitectureCommon.Datatypes;
using PedestrianModel.Agents.Reasoning.Pathfinding;
using PedestrianModel.Util;

namespace PedestrianModel {

    internal class Config {
        // Scenario options
        public static readonly ScenarioType Scenario = ScenarioType.Bottleneck150Agents;
        public static readonly bool IsGaussianNormalDistribution = true;

        // Debug
        public static readonly bool DebugEnabled = false;

        // Simulation
        public static readonly float LengthOfTimestepsInMilliseconds = 1000/15f; // 66,66666... ms, 15 frames per second
        public static readonly bool WalkLoops = false; // if reached target, teleport to start position and start again

        public static readonly TargetListType TargetListType = TargetListType.Sequential; // how to process the target positions

        public static readonly float TargetReachedDistance = 0.25f; // at what distance I have reached the target position

        public static readonly float MaxVelocity = 1.34f; // maximum movement velocity of agent
        public static readonly float StandardDeviation = 0.26f; // standard deviation of the normal gaussian distribution

        public static readonly Vector PedestrianDimensions = new Vector(0.4f, 0.4f, 0.4f); // agents are 0.4m x 0.4m x 0.4m

        // Visualization
        public static int VisualizationWidth = 1280;
        public static int VisualizationHeight = 720;
        public static float VisualizationZoom = 30;
        public static float VisualizationOffsetX;
        public static float VisualizationOffsetY;
    }

}