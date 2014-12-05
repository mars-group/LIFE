using System.Collections.Generic;
using CellLayer;
using CellLayer.TransportTypes;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Provide the sensor data mangement and methods.
    /// </summary>
    public class PerceptionAndMemory {
        private readonly CellLayerImpl _dataSourceLayer;
        private readonly Human _owner;
        public readonly Dictionary<int, TCell> CellIdToData = new Dictionary<int, TCell>();
        private readonly Blackboard _blackboard;


        public PerceptionAndMemory(CellLayerImpl celllayer, Human owner, Blackboard blackboard) {
            _dataSourceLayer = celllayer;
            _owner = owner;
            _blackboard = blackboard;
        }

        public void SenseCellInformations() {
            SenseCell();
        }

        /// <summary>
        ///     Get the data of the cell the agent is on and save it to the humans map.
        /// </summary>
        private void SenseCell() {
            int cellId = _blackboard.Get(Human.CellIdOfPosition);
            TCell cellData = _dataSourceLayer.GetDataOfCell(cellId);
            CellIdToData[cellId] = cellData;
        }
    }

}