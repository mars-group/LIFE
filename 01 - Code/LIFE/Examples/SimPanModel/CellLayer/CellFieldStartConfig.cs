using System.Collections.Generic;
using System.Drawing;

namespace CellLayer {

    public static class CellFieldStartConfig {
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
        public static readonly List<int> ExitCells = new List<int> { 391};
        public static readonly Point ExitPoint = new Point(11, 20);

        public static readonly List<int> TechnicalInformationCells = new List<int> {
            1,
            300,
        };

        public static readonly Dictionary<int, Point> TechnicalExitInformation = new Dictionary<int, Point> {
           /* {1, ExitPoint},
            {2, ExitPoint},
            {3, ExitPoint},
            {21, ExitPoint},
            {22, ExitPoint},
            {23, ExitPoint},
            {41, ExitPoint},
            {42, ExitPoint},
            {43, ExitPoint},
            {258, ExitPoint},
            {259, ExitPoint},
            {260, ExitPoint},
            {278, ExitPoint},
            {279, ExitPoint},
            {280, ExitPoint},
            {298, ExitPoint},
            {299, ExitPoint},
            {318, ExitPoint},
            {319, ExitPoint},
            {320, ExitPoint},
            {338, ExitPoint},
            {339, ExitPoint},
            {340, ExitPoint},
            {300, ExitPoint},*/
        };

        public static readonly Dictionary<int, Point> ExitAreaInformation = new Dictionary<int, Point> {
            {329, ExitPoint},
            {330, ExitPoint},
            {331, ExitPoint},
            {332, ExitPoint},
            {333, ExitPoint},
            {349, ExitPoint},
            {350, ExitPoint},
            {351, ExitPoint},
            {352, ExitPoint},
            {353, ExitPoint},
            {370, ExitPoint},
            {371, ExitPoint},
            {372, ExitPoint},
            {390, ExitPoint},
            {391, ExitPoint},
            {392, ExitPoint},
        };
    }

}