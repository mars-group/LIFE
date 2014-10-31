using System;

namespace CellLayer
{
    class Cell
    {
        private int _cellId;
        private int _posVertical;
        private int _poshorizontal;
        private CellLayerImpl.CellType _cellType;

       
        public Cell(int cellIid, int posVertical, int posHorizontal, CellLayerImpl.CellType cellType) {
            this._cellId = cellIid;
            this._posVertical = posVertical;
            this._poshorizontal = posHorizontal;
            this._cellType = cellType;
        }

        public void ChangeStateTo(CellLayerImpl.CellType state) {
            this._cellType = state;
           
        }

       
    }
}
