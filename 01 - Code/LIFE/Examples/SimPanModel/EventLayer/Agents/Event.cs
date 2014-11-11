using System;
using System.Threading;
using CellLayer;
using LayerAPI.Interfaces;

namespace EventLayer.Agents {
    public class Event : IAgent {
        private readonly CellLayerImpl _cellLayer;
        public readonly Guid ID = Guid.NewGuid();
        private int _tickAccu = 0;

        public Event(CellLayerImpl cellLayer) {
            _cellLayer = cellLayer;
        }

        #region IAgent Members

        public void Tick() {
            Thread.Sleep(1000);
            
            if (EventLayerImpl.PanicTime.ContainsKey(_tickAccu)) {
                int regardingCell = EventLayerImpl.PanicTime[_tickAccu][0];
                int range = EventLayerImpl.PanicTime[_tickAccu][1];

                _cellLayer.SetCellToPanik(regardingCell, range);
                EventLayerImpl.Log.Info("Panic Time! at tick: " + _tickAccu + " !");
                //EventLayerImpl.log.InfoFormat();
            }
            
            IncrTick();
        }

        #endregion

        private void IncrTick() {
            _tickAccu += 1;
        }
    }
}