using System;
using System.Collections.Generic;
using System.Threading;
using CellLayer;
using HumanLayer;
using HumanLayer.Agents;
using LayerAPI.Interfaces;

namespace EventLayer.Agents {

    /// <summary>
    ///     One single event agent is designed to manipulate panik on the cell world defined
    ///     by cell id , range of the chaos area aroung the panik cell and the tick number the event has to occur.
    /// </summary>
    public class Event : IAgent {
        private readonly CellLayerImpl _cellLayer;
        private readonly HumanLayerImpl _humanLayer;
        public readonly Guid ID = Guid.NewGuid();
        private int _tickAccu;

        public Event(CellLayerImpl cellLayer, HumanLayerImpl humanLayer) {
            _cellLayer = cellLayer;
            _humanLayer = humanLayer;
        }

        #region IAgent Members

        public void Tick() {
            if (EventLayerImpl.PanicTime.ContainsKey(_tickAccu)) {
                int regardingCell = EventLayerImpl.PanicTime[_tickAccu][0];
                int range = EventLayerImpl.PanicTime[_tickAccu][1];

                List<Guid> affectedAgentIds = _cellLayer.GetAgentIdsOfCellAndRange(regardingCell, range);
                List<Human> humansToKill = _humanLayer.GetHumansById(affectedAgentIds);
                foreach (Human human in humansToKill) {
                    human.GetKilledByPanicArea();
                }
                _cellLayer.SetCellToPanik(regardingCell, range);

                EventLayerImpl.Log.Info("Panic Time! at tick: " + _tickAccu + " !");
            }
            IncrTick();
            Thread.Sleep(10);
        }

        #endregion

        /// <summary>
        ///     Helper for simple call on increment tick.
        /// </summary>
        private void IncrTick() {
            _tickAccu += 1;
        }
    }

}