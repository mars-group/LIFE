using System.Collections.Generic;
using System.Linq;

namespace CellLayer {

    /// <summary>
    /// represents one single cell. cell holds specific simulation data and is communication medium for agents.
    /// </summary>
    public class Cell {
        private readonly int _cellId;
        public readonly int XCoordinate;
        public readonly int YCoordinate;
        public CellLayerImpl.CellType CellType;


        public Cell(int cellIid, int yCoordinate, int xCoordinate, CellLayerImpl.CellType cellType) {
            _cellId = cellIid;
            YCoordinate = yCoordinate;
            XCoordinate = xCoordinate;
            CellType = cellType;
        }

        public void ChangeStateTo(CellLayerImpl.CellType state) {
            CellType = state;
        }
        
        /// <summary>
        /// search for all cell ids reachable in a radius of a this cell
        /// respects cell field borders
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
                neighbourXCoordinates.AddRange(Enumerable.Range(XCoordinate - cellRange, cellRange * 2 + 1));
                neighbourYCoordinates.AddRange(Enumerable.Range(YCoordinate - cellRange, cellRange * 2 + 1));

                // correct the ranges to the coordinates allowed in current cellfield
                neighbourXCoordinates.RemoveAll(coordinate => coordinate < minX || coordinate > dim);
                neighbourYCoordinates.RemoveAll(coordinate => coordinate < minY || coordinate > dim);

                //var a = neighbourXCoordinates.Join<int>(neighbourYCoordinates, x => x, y => y, (x, y) => neighbourIds.Add((x - 1) * dim + y));
                   // , new LambdaEqualityComparer<int>((x, y) => true, (x) => 0)).ToArray();

                foreach (int xCoord in neighbourXCoordinates) {
                    foreach (int yCoord in neighbourYCoordinates) {
                        neighbourIds.Add((xCoord - 1)*dim + yCoord);
                    }
                }
                neighbourIds.Remove(_cellId);
            }
            return neighbourIds;
        }


       
        // In Zielrichtung laufen {N,S,W,O,NW,NO,SW,SO}
    }
}