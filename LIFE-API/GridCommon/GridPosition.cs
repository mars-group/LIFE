using System;

namespace LIFE.API.GridCommon
{
    public class GridPosition : GridCoordinate
    {
        public GridPosition(int x, int y) : base(x, y)
        {

        }

        /// <summary>
        ///   Returns the current yaw as grid orientation constant.
        /// </summary>
        public GridDirection GridDirection { get; set; }
    }
}