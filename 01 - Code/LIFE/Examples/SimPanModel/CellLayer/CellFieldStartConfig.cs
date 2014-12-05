using System.Collections.Generic;

namespace CellLayer {

    internal static class CellFieldStartConfig {
        public const int CellCountXAxis = 20;
        public const int CellCountYAxis = 20;
        public const int CellSideLength = 30;
        public const float AgentRadius = 12f;
        public const int SmallestXCoordinate = 1;
        public const int SmallestYCoordinate = 1;

        /// <summary>
        ///     the limit of pressure a cell can resist befor collapsing
        /// </summary>
        public const int PressureResistanceOfObstacleCells = 50;

        /// <summary>
        ///     the list of calming cells
        /// </summary>
        public static readonly List<int> CalmingCells = new List<int> {
            1,
            2,
            3,
            21,
            22,
            23,
            41,
            42,
            43,
            258,
            259,
            260,
            278,
            279,
            280,
            298,
            299,
            318,
            319,
            320,
            338,
            339,
            340,
            300,
        };

        /// <summary>
        ///     The list of obstacle cell ids.
        /// </summary>
        public static readonly List<int> ObstacleCells = new List<int> {
            54,
            55,
            56,
            57,
            58,
            190,
            328,
            329,
            330,
            331,
            332,
            333,
            334,
            370,
            372,
            390,
            392
        };

        /// <summary>
        ///     The list of exit cell ids.
        /// </summary>
        public static readonly List<int> ExitCells = new List<int>() {
            391,
        };
    }

}