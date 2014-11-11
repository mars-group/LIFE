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
        public CellLayerImpl.CellType CellType;
        public Guid AgentOnCellGuid { get; set; }

        public CellData(Cell correspondingCell) {
            ID = correspondingCell.CellId;
            X = correspondingCell.XCoordinate;
            Y = correspondingCell.YCoordinate;
            CellType = correspondingCell.CellType;
            AgentOnCellGuid = correspondingCell.AgentOnCell;
        }

        public override string ToString() {
            return string.Format("CellType: {0}, ID: {1}, X: {2}, Y: {3}, AgentOnCellGuid: {4}", CellType, ID, X, Y, AgentOnCellGuid);
        }
    }
}
