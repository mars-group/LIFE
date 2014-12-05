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
        public Point ExitInformationTechnical = new Point();
        public Point ExitInformationFromHuman = new Point();

        public bool HasCalmingSphereTechnical = false;
        public bool HasCalmingSphereByHuman = false;
        public bool IsExit = false;
        public bool IsTechnicalInformationSource = false;


        public TCell(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            Coordinates = correspondingCell.Coordinates;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
            PressureOnCell = correspondingCell.PressureOnCell;
            ResistanceToPressure = correspondingCell.ResistanceToPressure;
            ExitInformationTechnical = correspondingCell.ExitInformationTechnical;
            ExitInformationFromHuman = correspondingCell.ExitInformationByHuman;

            HasCalmingSphereTechnical = correspondingCell.HasCalmingSphereTechnical;
            HasCalmingSphereByHuman = correspondingCell.HasCalmingSphereByHuman;

            IsExit = correspondingCell.IsExit;
            IsTechnicalInformationSource = correspondingCell.IsTechnicalInformationSource;
        }
    }

}