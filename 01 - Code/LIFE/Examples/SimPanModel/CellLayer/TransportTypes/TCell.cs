using System;

namespace CellLayer.TransportTypes {

    /// <summary>
    ///     Represents sensor data of a single cell. Prevent from direkt reference.
    /// </summary>
    public class TCell {
        public readonly int ID;
        public readonly int X;
        public readonly int Y;
        public readonly Guid AgentOnCellGuid;
        public readonly CellLayerImpl.CellType CellType;

        public TCell(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            X = correspondingCell.XCoordinate;
            Y = correspondingCell.YCoordinate;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
        }

        public override string ToString() {
            return string.Format
                ("CellType: {0}, ID: {1}, X: {2}, Y: {3}, AgentOnCellGuid: {4}", CellType, ID, X, Y, AgentOnCellGuid);
        }
    }

}