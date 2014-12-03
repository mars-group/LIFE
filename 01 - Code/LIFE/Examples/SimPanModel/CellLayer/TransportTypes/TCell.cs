using System;
using System.Drawing;

namespace CellLayer.TransportTypes {

    /// <summary>
    ///     Represents sensor data of a single cell. Prevent from direkt reference.
    /// </summary>
    public class TCell {

        public readonly int ID;
        public readonly Point Coordinates;
        public readonly Guid AgentOnCellGuid;
        public readonly CellLayerImpl.CellType CellType;
        public readonly int PressureOnCell;
        public readonly int ResistanceToPressure;



        public TCell(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            Coordinates = correspondingCell.Coordinates;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
            PressureOnCell = correspondingCell.PressureOnCell;
            ResistanceToPressure = correspondingCell.ResistanceToPressure;
        }

        public override string ToString() {
            return string.Format
                ("CellType: {0}, ID: {1}, X: {2}, Y: {3}, AgentOnCellGuid: {4}", CellType, ID, Coordinates.X, Coordinates.Y, AgentOnCellGuid);
        }
    }

}