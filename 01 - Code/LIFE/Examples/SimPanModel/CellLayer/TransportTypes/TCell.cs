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
        //public readonly int ResistanceToPressure;
        public Point ExitInformationTechnical;
        public Point ExitInformationFromHuman;

        public bool HasCalmingSphereTechnical;
        public bool HasCalmingSphereByHuman;
        public bool IsExit;
        public bool IsTechnicalInformationSource;

        //public Point[] CoordinatesOfMassFlightLeaders;
        public Point DominantMassFlightLeaderCoordinates;
        //public Guid[] GuidsOfCalmingHumans;


        public TCell(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            Coordinates = correspondingCell.Coordinates;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
            IsExit = correspondingCell.IsExit;
            IsTechnicalInformationSource = correspondingCell.IsTechnicalInformationSource;
            PressureOnCell = correspondingCell.PressureOnCell;
            //ResistanceToPressure = correspondingCell.ResistanceToPressure;
            //CoordinatesOfMassFlightLeaders = (Point[])correspondingCell.CoordinatesOfMassFlightLeaders.Clone();
            DominantMassFlightLeaderCoordinates = correspondingCell.DominantMassFlightLeaderCoordinates;
            ExitInformationTechnical = correspondingCell.ExitInformationTechnical;
            //GuidsOfCalmingHumans = (Guid[])correspondingCell.GuidsOfCalmingHumans.Clone();
            HasCalmingSphereByHuman = correspondingCell.HasCalmingSphereByHuman;
            HasCalmingSphereTechnical = correspondingCell.HasCalmingSphereTechnical;
           
            
            

           
            
         
      
       
            
            
            
          
            

        }
    }

}