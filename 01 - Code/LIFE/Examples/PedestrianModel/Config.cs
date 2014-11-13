using DalskiAgent.Movement;
using PedestrianModel.Agents.Reasoning.Pathfinding;
using PedestrianModel.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    class Config
    {
        // Start
        public static readonly ScenarioBuilder.ScenarioTypes Scenario = ScenarioBuilder.ScenarioTypes.Bottleneck150Agents;

        // Debug
        public static readonly bool DebugEnabled = false;

        // Simulation
        public static readonly float LengthOfTimestepsInMilliseconds = 1000 / 15f; // 66,66666... ms, 15 frames per second
        public static readonly bool WalkLoops = false; // if reached target, teleport to start position and start again
        public static readonly TargetListType TargetListType = TargetListType.Sequential; // how to process the target positions
        public static readonly float TargetReachedDistance = 0.25f; // at what distance I have reached the target position
        public static readonly Vector PedestrianDimensions = new Vector(0.4f, 0.4f, 0.4f); // agents are 0.4m x 0.4m x 0.4m

        // Visualization
        public static int VisualizationWidth = 1280;
        public static int VisualizationHeight = 720;
        public static float VisualizationZoom = 30;
        public static float VisualizationOffsetX = 0;
        public static float VisualizationOffsetY = 0;
    }
}
