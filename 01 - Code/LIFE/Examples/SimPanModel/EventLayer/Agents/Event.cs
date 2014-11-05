using System.Threading;
using CellLayer;
using LayerAPI.Interfaces;

namespace EventLayer.Agents {
    internal class Event : IAgent {
        private readonly CellLayerImpl _cellLayer;
        private int _tickAccu = 0;

        public Event(CellLayerImpl cellLayer) {
            _cellLayer = cellLayer;
        }

        #region IAgent Members

        public void Tick() {
            Thread.Sleep(1000);
            if (EventLayerImpl.PanicTime.ContainsKey(_tickAccu)) {
                _cellLayer.SetCellToPanik(EventLayerImpl.PanicTime[_tickAccu],2);
                EventLayerImpl.log.Info("Panic Time! at tick: " + _tickAccu + " !");
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