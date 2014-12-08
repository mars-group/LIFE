using System.Collections.Generic;
using System.Drawing;
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

        /// <summary>
        ///     Rework the informations by the environment. Refresh dependent variables.
        ///     This step is needed after collecting sensor data from the cell.
        /// </summary>
        public void ProcessSensorInformations()
        {
            // Inspect the new data of the cell the agent is on.
            int cellID = _blackboard.Get(Human.CellIdOfPosition);
            TCell cellData = CellIdToData[cellID];

            RefreshTargetDepedingInformation(cellData);

            RefreshPressureDependingValues(cellData);

            if (_blackboard.Get(Human.MovementFailed) && _blackboard.Get(Human.HasMassFlightSphere))
            {
                _blackboard.Set(Human.PauseMassFlightSphere, 2);
            }

            if (cellData.IsExit)
            {
                _blackboard.Set(Human.IsOnExit, true);
            }
        }

        /// <summary>
        ///     Refresh the data depending on target informations on the cell. A reactive human cannot access the technical target
        ///     information. He will only follow a mass flight information. an deliberativ or reflective human will only follow his
        ///     own information by memory of getting into the room or technical source.
        /// </summary>
        /// <param name="cellData"></param>
        public void RefreshTargetDepedingInformation(TCell cellData)
        {
            // human is on mass flisght and is not the leader
            if (!cellData.DominantMassFlightLeaderCoordinates.IsEmpty && !_blackboard.Get(Human.HasMassFlightSphere))
            {
                _blackboard.Set(Human.Target, cellData.DominantMassFlightLeaderCoordinates);
                _blackboard.Set(Human.HasTarget, true);
                _blackboard.Set(Human.IsOnMassFlight, true);
            }
                //human has own target and will follow not the other
            else if (_blackboard.Get(Human.SelfChosenTarget) != new Point()) {
                _blackboard.Set(Human.Target, _blackboard.Get(Human.SelfChosenTarget));
                _blackboard.Set(Human.HasTarget, true);
            }
            else
            {
                _blackboard.Set(Human.Target, new Point());
                _blackboard.Set(Human.HasTarget, false);
            }
        }


        /// <summary>
        ///     Calculate cell pressure depending variables.
        /// </summary>
        /// <param name="cellData"></param>
        public void RefreshPressureDependingValues(TCell cellData)
        {
            // Check if the pressure is higher than the human can handle with
            if (cellData.PressureOnCell > _blackboard.Get(Human.ResistanceToPressure))
            {
                int vitalEnergy = _blackboard.Get(Human.VitalEnergy);
                if (vitalEnergy < 5)
                {
                    _blackboard.Set(Human.VitalEnergy, 0);
                    _owner.SetAsKilled();
                }
                else
                {
                    _blackboard.Set(Human.VitalEnergy, vitalEnergy - 5);
                }
            }
        }


    }

}