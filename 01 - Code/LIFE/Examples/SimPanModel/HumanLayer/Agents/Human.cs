using System;
using CellLayer;
using LayerAPI.Interfaces;

namespace HumanLayer.Agents {
    public class Human : IAgent {
        public readonly Guid GuidID;
        private CellLayerImpl _cellLayer;
        private int _posX;
        private int _posY;


        public Human(CellLayerImpl cellLayer) {
            _cellLayer = cellLayer;
            int posX;
            int posY;
            GuidID = new Guid(); 

            cellLayer.GiveAndSetToRandomPosition(GuidID, HumanLayerImpl.HumanDiameter, out this._posX, out this._posY);
        }

        #region IAgent Members

        public void Tick() {
            HumanLayerImpl.Log.Info("i am on : (" + _posX + "," + _posY + ")");
        }

        #endregion
    }
}