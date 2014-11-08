using PedestrianModel.Agents.Reasoning.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PedestrianModel
{
    class Config
    {
        public static readonly float lengthOfTimestepsInMilliseconds = 1000 / 15f;     // 66,66666... ms, 15 frames per second
        public static readonly bool walkLoops = false; // if reached target, teleport to start position and start again
        public static readonly TargetListType targetListType = TargetListType.Sequential; // how to process the target positions
        public static readonly float targetReachedDistance = 0.25f; // at what distance I have reached the target position
    }
}
