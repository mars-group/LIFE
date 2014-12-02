using System.Collections.Generic;
using CellLayer;
using CellLayer.TransportTypes;
using System.Drawing; // is needed because of TCell !

namespace HumanLayer.Agents {
    /// <summary>
    /// Provide the sensor data mangement and methods.
    /// </summary>
    public class PerceptionAndMemory {
        private readonly CellLayerImpl _dataSourceLayer;
        private readonly Human _owner;
        private readonly Dictionary<int, TCell> _cellIdToData = new Dictionary<int, TCell>();

        public PerceptionAndMemory(CellLayerImpl celllayer, Human owner) {
            _dataSourceLayer = celllayer;
            _owner = owner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cellId"></param>
        public void SenseCell() {
            TCell cellData = _dataSourceLayer.GetDataOfCell(_owner.CellId);
            _cellIdToData[_owner.CellId] = cellData;
        }

        public void SetAsKilled() {
            _owner.CanMove = false;
            _owner.IsAlive = false;
            _dataSourceLayer.UpdateAgentDrawStatus(_owner.AgentID, CellLayerImpl.BehaviourType.Dead);
        }
    }

}