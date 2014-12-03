using PedestrianModel.Agents.Reasoning.Pathfinding;
using PedestrianModel.Util;
using SpatialCommon.Datatypes;

namespace PedestrianModel {

    internal class Config {
        // Scenario options
        public static ScenarioType Scenario = ScenarioType.Bottleneck50Agents;
        public static readonly bool IsGaussianNormalDistribution = false;

        // ESC
        public static bool UsesESC = true;

        // Debug
        public static readonly bool DebugEnabled = false;

        // Simulation
        public static readonly double LengthOfTimestepsInMilliseconds = 1000/15d; // 66,66666... ms, 15 frames per second
        public static readonly bool WalkLoops = true; // if reached target, teleport to start position and start again

        public static readonly TargetListType TargetListType = TargetListType.Sequential; // how to process the target positions

        public static readonly double TargetReachedDistance = 0.25d; // at what distance I have reached the target position

        public static readonly double MaxVelocity = 1.34d; // maximum movement velocity of agent
        public static readonly double StandardDeviation = 0.26d; // standard deviation of the normal gaussian distribution

        public static readonly Vector PedestrianDimensions = new Vector(0.4d, 0.4d, 0.4d); // agents are 0.4m x 0.4m x 0.4m

        // Visualization
        public static int VisualizationWidth = 1280;
        public static int VisualizationHeight = 720;
        public static double VisualizationZoom = 30;
        public static double VisualizationOffsetX;
        public static double VisualizationOffsetY;
    }

}