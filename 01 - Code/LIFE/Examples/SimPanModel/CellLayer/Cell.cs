using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace CellLayer
{
    public class Cell
    {
        private int _cellId;
        public int PosVertical;
        public int Poshorizontal;
        public CellLayerImpl.CellType CellType;

       
        public Cell(int cellIid, int posVertical, int posHorizontal, CellLayerImpl.CellType cellType) {
            this._cellId = cellIid;
            this.PosVertical = posVertical;
            this.Poshorizontal = posHorizontal;
            this.CellType = cellType;
        }

        public void ChangeStateTo(CellLayerImpl.CellType state) {
            this.CellType = state;
        }
        /*
        public List<int> GetNeighbourIds(int cellRange) {

            List<int> neighbourIds = new List<int>();
            if (CellLayerImpl.CellCountHorizontal == CellLayerImpl.CellCountVertical && cellRange > 0) {

                int dim = CellLayerImpl.CellCountHorizontal;
                var anchor = this._cellId - cellRange;
                
                neighbourIds.Add(anchor);

                neighbourIds.AddRange(Enumerable.Range(anchor, cellRange * 2 + 1));

                for (var i = 1; i <= cellRange; i++) {

                    var lowerCellId = anchor - dim*i;
                    var upperCellId = anchor + dim*i;

                    neighbourIds.AddRange(Enumerable.Range(lowerCellId, cellRange * 2 + 1));
                    neighbourIds.AddRange(Enumerable.Range(upperCellId, cellRange * 2 + 1));
                }
                neighbourIds.Remove(this._cellId);
            }
            return neighbourIds;
        }*/

        public List<int> GetNeighbourIdsInRange(int cellRange) {
            
            List<int> neighbourXCoordinates = new List<int>();
            List<int> neighbourYCoordinates = new List<int>();
            List<int> neighbourIds = new List<int>();
            
            if (CellLayerImpl.CellCountHorizontal == CellLayerImpl.CellCountVertical && cellRange > 0) {

                int dim = CellLayerImpl.CellCountHorizontal;
                int myX = this.Poshorizontal;
                int myY = this.PosVertical;

                neighbourXCoordinates.AddRange(Enumerable.Range(myX - cellRange, cellRange * 2 + 1));
                neighbourYCoordinates.AddRange(Enumerable.Range(myY - cellRange, cellRange * 2 + 1));
                
                neighbourXCoordinates.RemoveAll(num => num < 1 || num > dim);
                neighbourYCoordinates.RemoveAll(num => num < 1 || num > dim);

                foreach (var xCoord in neighbourXCoordinates) {
                    foreach (var yCoord in neighbourYCoordinates) {
                        neighbourIds.Add((xCoord - 1) * dim + yCoord);
                    }
                }
                neighbourIds.Remove(this._cellId);


            }
            return neighbourIds;
        }



        // int cellIid = (posHorizontal - 1)*CellCountHorizontal + posVertical;
        // In Zielrichtung laufen {N,S,W,O,NW,NO,SW,SO}
       
    }
}
