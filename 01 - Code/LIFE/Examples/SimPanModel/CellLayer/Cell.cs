using System;
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
        ///     SC => none
        /// </summary>
        /// <param name="state"></param>
        public bool IsChangeStateToAllowed(CellLayerImpl.CellType state) {
            CellLayerImpl.CellType cellType = CellType;
            switch (cellType) {
                case CellLayerImpl.CellType.Neutral:
                    return true;
                case CellLayerImpl.CellType.Obstacle:
                    if (state == CellLayerImpl.CellType.Neutral || state == CellLayerImpl.CellType.Panic
                        || state == CellLayerImpl.CellType.Chaos) {
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        /// <summary>
        ///     Change the state of cell respecting the allowed transitions.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool ChangeStateTo(CellLayerImpl.CellType state) {
            if (IsChangeStateToAllowed(state)) {
                CellType = state;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Get a new cell object with the changed state of cell type if transition is allowed
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        public Cell GetCopyWithOtherStatus(CellLayerImpl.CellType newState) {
            if (IsChangeStateToAllowed(newState)) {
                Cell copiedCell = new Cell(CellId, Coordinates, newState);
                copiedCell.AgentOnCell = AgentOnCell;
                return copiedCell;
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
                Cell copiedCell = new Cell(CellId, Coordinates, CellType);
                copiedCell.AgentOnCell = agentId;
                return copiedCell;
            }
            return null;
        }

        /// <summary>
        ///     Get a new cell object with member agentID entry empty guid.
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        public Cell GetCopyWithoutAgentOnCell(Guid agentID) {
            if (AgentOnCell == agentID) {
                Cell copiedCell = new Cell(CellId, Coordinates, CellType);
                copiedCell.AgentOnCell = Guid.Empty;
                return copiedCell;
            }
            return null;
        }

        /// <summary>
        ///     Search for all cell ids reachable in a radius of a this cell.
        ///     Respects cell field borders
        /// </summary>
        /// <param name="cellRange"></param>
        /// <returns></returns>
        public List<int> GetNeighbourIdsInRange(int cellRange) {
            List<int> neighbourXCoordinates = new List<int>();
            List<int> neighbourYCoordinates = new List<int>();
            List<int> neighbourIds = new List<int>();

            const int minX = CellLayerImpl.SmallestXCoordinate;
            const int minY = CellLayerImpl.SmallestYCoordinate;


            if (CellLayerImpl.CellCountXAxis == CellLayerImpl.CellCountYAxis && cellRange > 0) {
                int dim = CellLayerImpl.CellCountXAxis;

                // get the range of x and y coordinates which correspond to neighbour cells
                neighbourXCoordinates.AddRange(Enumerable.Range(Coordinates.X - cellRange, cellRange*2 + 1));
                neighbourYCoordinates.AddRange(Enumerable.Range(Coordinates.Y - cellRange, cellRange*2 + 1));

                // Correct the ranges to the coordinates allowed in current cellfield
                neighbourXCoordinates.RemoveAll(coordinate => coordinate < minX || coordinate > dim);
                neighbourYCoordinates.RemoveAll(coordinate => coordinate < minY || coordinate > dim);

                foreach (int xCoord in neighbourXCoordinates) {
                    foreach (int yCoord in neighbourYCoordinates) {
                        neighbourIds.Add((yCoord - 1)*dim + xCoord);
                    }
                }
                neighbourIds.Remove(CellId);
            }
            return neighbourIds;
        }

        /// <summary>
        ///     Get the transport type of the cell.
        /// </summary>
        /// <returns></returns>
        public TCell GetTransportType() {
            return new TCell(this);
        }
    }

}