// is needed because of TCell !
using System.Collections.Generic;
using System.Drawing;
using CellLayer;
using CellLayer.TransportTypes;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Provide the sensor data mangement and methods.
    /// </summary>
    public class PerceptionAndMemory {
        private readonly CellLayerImpl _dataSourceLayer;
        private readonly Human _owner;
        private readonly Dictionary<int, TCell> _cellIdToData = new Dictionary<int, TCell>();

        public PerceptionAndMemory(CellLayerImpl celllayer, Human owner) {
            _dataSourceLayer = celllayer;
            _owner = owner;
        }

        public void SenseAndProcessInformations() {
            SenseCell();
            CalculateInformations();
        }

        /// <summary>
        ///     Get the data of the cell the agent is on and save it to the humans map.
        /// </summary>
        private void SenseCell() {
            TCell cellData = _dataSourceLayer.GetDataOfCell(_owner.CellId);
            _cellIdToData[_owner.CellId] = cellData;
        }

        public void SetAsKilled() {
            _owner.CanMove = false;
            _owner.IsAlive = false;
            _dataSourceLayer.UpdateAgentDrawStatus(_owner.AgentID, CellLayerImpl.BehaviourType.Dead);
        }

        /// <summary>
        ///     Rework the informations by the environment. Refresh dependent variables.
        /// </summary>
        private void CalculateInformations() {
            Point target = _owner.HumanBlackboard.Get(Human.Target);
            if (!target.IsEmpty) {
                _owner.HumanBlackboard.Set(Human.HasTarget, true);
            }
            else {
                _owner.HumanBlackboard.Set(Human.HasTarget, false);
            }


            TCell cellData = _cellIdToData[_owner.CellId];
            if (cellData.PressureOnCell > _owner.ResistanceToPressure) {
                if (_owner.VitalEnergy < 5) {
                    _owner.VitalEnergy = 0;
                }
                else {
                    _owner.VitalEnergy -= 5;
                }
            }

            if (_owner.VitalEnergy == 0) {
                SetAsKilled();
            }
        }
    }

}