﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer.TransportTypes;

namespace CellLayer {

    /// <summary>
    ///     represents one single cell. cell holds specific simulation data and is communication medium for agents.
    /// </summary>
    public class Cell {
        public readonly int CellId;
        public readonly Point Coordinates;
        public CellLayerImpl.CellType CellType;
        public Guid AgentOnCell = Guid.Empty;
        public bool IsExit;

        public bool IsTechnicalInformationSource;
        public Point ExitInformationTechnical;

        public bool IsExitArea;
        public Point ExitAreaInformation;

        public int PressureOnCell;
        private int _resistanceToPressure = CellFieldStartConfig.PressureResistanceOfObstacleCells;

        /// <summary>
        ///     There may be more than one influencing human on one cell, if one deletes his information the cell must show
        ///     furthermore if there is another entry in the array. For a simple solution only the first entry is shown on cell.
        /// </summary>
        private Point[] _coordinatesOfMassFlightLeaders = new Point[0];
        public Point DominantMassFlightLeaderCoordinates;

        /// <summary>
        ///     There may be more than one influencing human on one cell, if one deletes his effect the cell must show it
        ///     furthermore if there is another entry in the array
        /// </summary>
        private Guid[] _guidsOfCalmingHumans = new Guid[0];

        public bool HasCalmingSphereByHuman;
        public bool HasCalmingSphereTechnical;
        
        public Cell(int cellIid, Point position, CellLayerImpl.CellType cellType) {
            CellId = cellIid;
            Coordinates = position;
            CellType = cellType;
        }

        /// <summary>
        ///     available transitions for cells types
        ///     NC = neutral cell
        ///     OC = obstacle cell
        ///     PC = panic event cell
        ///     CC = chaos cell
        ///     SC = agent diend on cell
        ///     transitions:
        ///     NC => {ALL}
        ///     OC => {NC; PC, CC}
        ///     PC => none
        ///     CC => none
        ///     SC => {PC, CC} this is a change to the SimPan model, but is needed for trampled humans and then panic event on cell
        /// </summary>
        /// <param name="state"></param>
        public bool IsChangeStateToAllowed(CellLayerImpl.CellType state) {
            switch (CellType) {
                case CellLayerImpl.CellType.Neutral:
                    return true;

                case CellLayerImpl.CellType.Obstacle:
                    return state == CellLayerImpl.CellType.Neutral || state == CellLayerImpl.CellType.Panic
                           || state == CellLayerImpl.CellType.Chaos;

                case CellLayerImpl.CellType.Sacrifice:
                    return state == CellLayerImpl.CellType.Panic || state == CellLayerImpl.CellType.Chaos;

                default:
                    return false;
            }
        }

        /// <summary>
        ///     Add the strenght value to the current pressure of the cell. If pressure is higher than the
        ///     pressure resistance value and is an obstacle cell it gets broken down to neutral cell.
        ///     Humans read this value too but their reaction depends on their own resistance to pressure.
        /// </summary>
        /// <param name="strenght"></param>
        public void AddPressure(int strenght) {
            PressureOnCell += strenght;

            if (PressureOnCell > _resistanceToPressure && CellType == CellLayerImpl.CellType.Obstacle) {
                PressureOnCell = 0;
                CellLayerImpl.Log.Info("CELL: " + CellId + " Obstacle cell broke under pressure to neutral");
                ChangeStateTo(CellLayerImpl.CellType.Neutral);
            }
        }

        /// <summary>
        ///     Change the state of cell respecting the allowed transitions.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private void ChangeStateTo(CellLayerImpl.CellType state) {
            if (IsChangeStateToAllowed(state)) {
                CellType = state;
            }
        }

        /// <summary>
        ///     Get a cloned Cell object to work on and to replace the old on the cell field data structure.
        ///     Because array is always reference type a direct copy is needed.
        /// </summary>
        /// <returns></returns>
        public Cell GetClone() {
            Cell cloneCell = new Cell(CellId, Coordinates, CellType);
            cloneCell.AgentOnCell = AgentOnCell;
            cloneCell.IsExit = IsExit;
            cloneCell.IsTechnicalInformationSource = IsTechnicalInformationSource;
            cloneCell.PressureOnCell = PressureOnCell;
            cloneCell._resistanceToPressure = _resistanceToPressure;
            cloneCell._coordinatesOfMassFlightLeaders = (Point[]) _coordinatesOfMassFlightLeaders.Clone();
            cloneCell.DominantMassFlightLeaderCoordinates = DominantMassFlightLeaderCoordinates;
            cloneCell.ExitInformationTechnical = ExitInformationTechnical;
            cloneCell._guidsOfCalmingHumans = (Guid[]) _guidsOfCalmingHumans.Clone();
            cloneCell.HasCalmingSphereByHuman = HasCalmingSphereByHuman;
            cloneCell.HasCalmingSphereTechnical = HasCalmingSphereTechnical;

            cloneCell.IsExitArea = IsExitArea;
            cloneCell.ExitAreaInformation = ExitAreaInformation;

            return cloneCell;
        }

        /// <summary>
        ///     Get the transport type of the cell. The transport type represent
        ///     a simple snapshot of the current member values of the cell. The cell
        ///     is not referenced and so there is no danger of manipulating the cell.
        /// </summary>
        /// <returns></returns>
        public TCell GetTransportType() {
            return new TCell(this);
        }

        /// <summary>
        ///     Get a new cell object with the changed state of cell type if transition is allowed
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public Cell GetCopyWithOtherStatus(CellLayerImpl.CellType newState) {
            if (IsChangeStateToAllowed(newState)) {
                Cell cloneToModify = GetClone();
                cloneToModify.CellType = newState;
                return cloneToModify;
            }
            return null;
        }

        /// <summary>
        ///     Get a new cell object with an entry of the member agentID.
        /// </summary>
        /// <param name="agentId"></param>
        /// <returns></returns>
        public Cell GetCopyWithAgentOnCell(Guid agentId) {
            if (AgentOnCell == Guid.Empty) {
                Cell copiedCell = GetClone();
                copiedCell.AgentOnCell = agentId;
                return copiedCell;
            }
            return null;
        }

        /// <summary>
        ///     Get a new cell object with member agentID entry empty guid.
        ///     Delete Pressure if a human has left cell.
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public Cell GetCopyWithoutAgentOnCell(Guid agentID) {
            if (AgentOnCell == agentID) {
                Cell copiedCell = GetClone();
                copiedCell.AgentOnCell = Guid.Empty;
                copiedCell.PressureOnCell = 0;
                return copiedCell;
            }
            return null;
        }

        /// <summary>
        ///     Search for all cell ids reachable in a radius of a this cell.
        ///     Respect cell field borders.
        /// </summary>
        /// <param name="cellRange"></param>
        /// <returns></returns>
        public List<Point> GetNeighbourPointsInRange(int cellRange) {
            List<int> neighbourXCoordinates = new List<int>();
            List<int> neighbourYCoordinates = new List<int>();
            List<Point> neighbourPoints = new List<Point>();

            const int minX = CellFieldStartConfig.SmallestXCoordinate;
            const int minY = CellFieldStartConfig.SmallestYCoordinate;

            // following calculation needs a square cell field
            if (CellFieldStartConfig.CellCountXAxis == CellFieldStartConfig.CellCountYAxis && cellRange > 0) {
                // get the range of x and y coordinates which correspond to neighbour cells
                neighbourXCoordinates.AddRange(Enumerable.Range(Coordinates.X - cellRange, cellRange*2 + 1));
                neighbourYCoordinates.AddRange(Enumerable.Range(Coordinates.Y - cellRange, cellRange*2 + 1));

                // Correct the ranges to the coordinates allowed in current cellfield
                neighbourXCoordinates.RemoveAll
                    (coordinate => coordinate < minX || coordinate > CellFieldStartConfig.CellCountXAxis);
                neighbourYCoordinates.RemoveAll
                    (coordinate => coordinate < minY || coordinate > CellFieldStartConfig.CellCountXAxis);

                foreach (int xCoord in neighbourXCoordinates) {
                    foreach (int yCoord in neighbourYCoordinates) {
                        neighbourPoints.Add(new Point(xCoord, yCoord));
                    }
                }
                neighbourPoints.Remove(Coordinates);
            }
            return neighbourPoints;
        }

        /// <summary>
        ///     Adapter for GetNeighbourPointsInRange inclusive transfomation from point to cell id.
        /// </summary>
        /// <param name="cellRange"></param>
        /// <returns></returns>
        public List<int> GetNeighbourIdsInRange(int cellRange) {
            List<Point> neighbourPoints = GetNeighbourPointsInRange(cellRange);
            List<int> neighbourIds = new List<int>();

            foreach (Point neighbourPoint in neighbourPoints) {
                neighbourIds.Add(CellLayerImpl.CalculateCellId(neighbourPoint.X, neighbourPoint.Y));
            }
            return neighbourIds;
        }

        /// <summary>
        ///     Get the dominant exit information (as point data) if available.
        ///     The hirarchie is defined here.
        /// </summary>
        /// <returns></returns>
        private Point GetExitInformation() {
            if (!ExitAreaInformation.IsEmpty) {
                return ExitAreaInformation;
            }

            if (!DominantMassFlightLeaderCoordinates.IsEmpty) {
                return DominantMassFlightLeaderCoordinates;
            }
            if (!ExitInformationTechnical.IsEmpty) {
                return ExitInformationTechnical;
            }
            return new Point();
        }

        /// <summary>
        ///     Test if the cell has an entry of exit coordinates.
        /// </summary>
        /// <returns></returns>
        private bool HasExitInformation() {
            return (!DominantMassFlightLeaderCoordinates.IsEmpty || !ExitInformationTechnical.IsEmpty
                    || !ExitAreaInformation.IsEmpty);
        }

        /// <summary>
        ///     Add an entry in the GuidsOfCalmingHumans member. there may be more than one
        ///     human influencing this cell with an calming sphere.
        /// </summary>
        /// <param name="guid"></param>
        public void AddGuidOfCalmingHuman(Guid guid) {
            int index = Array.IndexOf(_guidsOfCalmingHumans, guid);

            // check if guid is maybe already contained
            if (index == -1) {
                // Use the list operations for lesser instructions, guids are value types and do not get manipulated 
                List<Guid> guidsList = new List<Guid>(_guidsOfCalmingHumans) {guid};
                Guid[] newCalmingHumans = guidsList.ToArray();
                _guidsOfCalmingHumans = newCalmingHumans;
                RefreshCalmingStatus();
            }
        }

        /// <summary>
        ///     If a human has the calming effect to this cell he may not be the only influencing human.
        ///     So the effect must be kept if there is still influence from another human.
        /// </summary>
        /// <param name="guid"></param>
        public void DeleteGuidOfCalmingHuman(Guid guid) {
            int index = Array.IndexOf(_guidsOfCalmingHumans, guid);

            // Check if guid is really contained
            if (index != -1) {
                // Use the list operations for lesser instructions, guids are value types and do not get manipulated 
                List<Guid> guidsList = new List<Guid>(_guidsOfCalmingHumans);
                guidsList.Remove(guid);
                Guid[] newCalmingHumans = guidsList.ToArray();
                _guidsOfCalmingHumans = newCalmingHumans;
                RefreshCalmingStatus();
            }
        }

        /// <summary>
        ///     Test if any human has influence with his calming sphere on this cell.The guids of all
        ///     influencing humans are listet in the GuidsOfCalmingHumans. If it is empty there is no influencing human.
        /// </summary>
        private void RefreshCalmingStatus() {
            HasCalmingSphereByHuman = _guidsOfCalmingHumans.Length != 0;
        }

        /// <summary>
        ///     If a human has a sphere in which he spread exit informations, this information
        ///     is saved in the cell.
        /// </summary>
        /// <param name="positionOfLeader"></param>
        public void AddMassFlightInformation(Point positionOfLeader) {
            int index = Array.IndexOf(_coordinatesOfMassFlightLeaders, positionOfLeader);

            // check if point is maybe already contained
            if (index == -1) {
                // Use the list operations for lesser instructions, points are value types and do not get manipulated 
                List<Point> pointList = new List<Point>(_coordinatesOfMassFlightLeaders) {positionOfLeader};
                Point[] newMassFlightPoints = pointList.ToArray();
                _coordinatesOfMassFlightLeaders = newMassFlightPoints;
                RefreshMassFlightStatus();
            }
        }

        /// <summary>
        ///     If a human has a sphere in which he spread exit informations, this information
        ///     is saved in the cell and must be deleted if the influence is ending.
        /// </summary>
        /// <param name="positionOfLeader"></param>
        public void DeleteMassFlightInformation(Point positionOfLeader) {
            int index = Array.IndexOf(_coordinatesOfMassFlightLeaders, positionOfLeader);

            // check if point is really contained
            if (index != -1) {
                // Use the list operations for lesser instructions, point are value types and do not get manipulated 
                List<Point> pointList = new List<Point>(_coordinatesOfMassFlightLeaders);
                pointList.Remove(positionOfLeader);
                Point[] newMassFlightPoints = pointList.ToArray();
                _coordinatesOfMassFlightLeaders = newMassFlightPoints;
                RefreshMassFlightStatus();
            }
        }

        /// <summary>
        ///     If there are any mass flight informations from a human the status is positive.
        /// </summary>
        private void RefreshMassFlightStatus() {
            DominantMassFlightLeaderCoordinates = _coordinatesOfMassFlightLeaders.Length != 0
                ? _coordinatesOfMassFlightLeaders[0]
                : new Point();
        }

        /// <summary>
        ///     Prepare the colors and border informations for the view. Data is
        ///     passed via a primitive array of values from various types.
        /// </summary>
        /// <returns></returns>
        public object[] GetViewData() {
            // minimum values
            int posX = Coordinates.X;
            int posY = Coordinates.Y;
            Color fillingColour = CellLayerImpl.CellColors[CellType];
            string cellText = "";
            string borderStyle = "thin black";

            // add a text presenting the submitted coordinates
            if (HasExitInformation()) {
                Point exit = GetExitInformation();
                cellText = "(" + exit.X + "," + exit.Y + ")";
            }

            // correct the cell filling color if is calming
            if (HasCalmingSphereTechnical || HasCalmingSphereByHuman) {
                // Only  present the sphere if it is a neutral cell
                if (CellType == CellLayerImpl.CellType.Neutral) {
                    fillingColour = CellLayerImpl.ColorOfCalmingCell;
                }
            }

            if (IsExit) {
                borderStyle = "fat grey";
            }

            if (IsTechnicalInformationSource) {
                borderStyle = "fat blue";
            }

            object[] data = {posX, posY, fillingColour, cellText, borderStyle};
            return data;
        }
    }

}