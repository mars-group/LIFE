using System;
using System.Drawing;

namespace CellLayer.TransportTypes {

    /// <summary>
    ///     Represents sensor data of a single cell. Prevents original cell object from direkt reference.
    /// </summary>
    public class TCell {
        public readonly int ID;
        public readonly Point Coordinates;
        public readonly Guid AgentOnCellGuid;
        public readonly CellLayerImpl.CellType CellType;
        public readonly int PressureOnCell;

        public readonly bool HasCalmingSphereTechnical;
        public readonly bool HasCalmingSphereByHuman;
        public readonly bool IsExit;

        public readonly bool IsExitArea;
        public Point ExitInformationTechnical;
        public bool IsTechnicalInformationSource;
        public Point ExitAreaInformation;
        public Point DominantMassFlightLeaderCoordinates;

        /// <summary>
        ///     Create the TCell by the values of the original cell object. Because the data types are all
        ///     value type there is no depp copy needed.
        /// </summary>
        /// <param name="correspondingCell"></param>
        public TCell(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            Coordinates = correspondingCell.Coordinates;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
            IsExit = correspondingCell.IsExit;
            IsTechnicalInformationSource = correspondingCell.IsTechnicalInformationSource;
            PressureOnCell = correspondingCell.PressureOnCell;

            DominantMassFlightLeaderCoordinates = correspondingCell.DominantMassFlightLeaderCoordinates;
            ExitInformationTechnical = correspondingCell.ExitInformationTechnical;
            HasCalmingSphereByHuman = correspondingCell.HasCalmingSphereByHuman;
            HasCalmingSphereTechnical = correspondingCell.HasCalmingSphereTechnical;
            
            IsExitArea = correspondingCell.IsExitArea;
            ExitAreaInformation = correspondingCell.ExitAreaInformation;
        }
    }

}