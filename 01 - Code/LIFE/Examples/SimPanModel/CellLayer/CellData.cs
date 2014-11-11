using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CellLayer
{
    public class CellData
    {
        public int ID { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public CellLayerImpl.CellType cellType;

        public CellData(Cell correspondingCell) {
            this.ID = correspondingCell.CellId;
            this.X = correspondingCell.XCoordinate;
            this.Y = correspondingCell.YCoordinate;
            this.cellType = correspondingCell.CellType;
        }

    }
}
