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
        public void ProcessSensorInformations() {
            // Inspect the new data of the cell the agent is on.
            int cellID = _blackboard.Get(Human.CellIdOfPosition);
            TCell cellData = CellIdToData[cellID];

            _blackboard.Set(Human.IsOnExit, cellData.IsExit);
            _blackboard.Set(Human.IsInExitArea, cellData.IsExitArea);

            RefreshCalmingSphereDependingValues(cellData);
            UpdateBehaviourType();
            RefreshPressureDependingValues(cellData);


            switch (_owner.CurrentBehaviourType) {
                case CellLayerImpl.BehaviourType.Reactive:
                    ProcessSensorInformationsReactive(cellData);
                    break;
                case CellLayerImpl.BehaviourType.Deliberative:
                    ProcessSensorInformationsDeliberative(cellData);
                    break;
                case CellLayerImpl.BehaviourType.Reflective:
                    ProcessSensorInformationsReflective(cellData);
                    break;
            }
        }

        private void ProcessSensorInformationsReflective(TCell cellData) {
          
            if (_blackboard.Get(Human.MovementFailed) && _owner.HasMassFlightSphere) {
                _owner.PauseMassFlightSphere = 2;
            }

            if (_blackboard.Get(Human.IsInExitArea)) {
                _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                _blackboard.Set(Human.HasTarget, true);
            }

            if (_owner.HasCalmingSphere) {
                _owner.CreateOrUpdateCalmingSphere();
            }
            else {
                _owner.DeleteCalmingSphere();
            }
        }


        private void ProcessSensorInformationsDeliberative(TCell cellData) {
           
            if (_blackboard.Get(Human.MovementFailed) && _owner.HasMassFlightSphere) {
                _owner.PauseMassFlightSphere = 2;
            }

            if (_blackboard.Get(Human.IsInExitArea)) {
                _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                _blackboard.Set(Human.HasTarget, true);
            }

            if (_blackboard.Get(Human.HasTarget)) {
                _owner.HasMassFlightSphere = true;
            }
            else {
                _owner.HasMassFlightSphere = false;
            }

            if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere == 0) {
                _owner.CreateOrUpdateMassFlightSphere();
            }

            else if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere > 0) {
                _owner.DeleteMassFlightSphere();
            }
        }

        /// <summary>
        ///     Reactive human will follow a mass flight leader until he is in an exit area. If
        ///     both targets are not given he cannot get a target.
        /// </summary>
        /// <param name="cellData"></param>
        private void ProcessSensorInformationsReactive(TCell cellData) {
            _owner.MassFlightTargetCoordinates = cellData.DominantMassFlightLeaderCoordinates;
            
            if (!_owner.MassFlightTargetCoordinates.IsEmpty || _blackboard.Get(Human.IsInExitArea)) {
                if (_blackboard.Get(Human.IsInExitArea)) {
                    _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                    _blackboard.Set(Human.HasTarget, true);
                }
                else if (!cellData.DominantMassFlightLeaderCoordinates.IsEmpty) {
                    _blackboard.Set(Human.Target, cellData.DominantMassFlightLeaderCoordinates);
                    _blackboard.Set(Human.HasTarget, true);
                    _owner.IsOnMassFlight = true;
                }
            }
            else {
                _blackboard.Set(Human.Target, new Point());
                _blackboard.Set(Human.HasTarget, false);
                _owner.MassFlightTargetCoordinates = new Point();
                _owner.IsOnMassFlight = false;
            }
        }

        /*
        /// <summary>
        ///     Refresh the data depending on target informations on the cell. A reactive human cannot access the technical target
        ///     information. He will only follow a mass flight information. an deliberativ or reflective human will only follow his
        ///     own information by memory of getting into the room or technical source.
        /// </summary>
        /// <param name="cellData"></param>
        public void RefreshTargetDepedingInformation(TCell cellData)
        {
            if (cellData.IsExitArea) {
                _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                _blackboard.Set(Human.HasTarget,true);
                return;
            }
            // human is on mass flisght and is not the leader
           
                //human has own target and will follow not the other
            else if (!_owner.SelfChosenTarget.IsEmpty){
                _blackboard.Set(Human.Target, _owner.SelfChosenTarget);
                _blackboard.Set(Human.HasTarget, true);
            }
            else
            {
                _blackboard.Set(Human.Target, new Point());
                _blackboard.Set(Human.HasTarget, false);
            }
        }*/

        /*
        /// <summary>
        ///     Test if the human has exit information by memory or by technical.
        ///     The access to thieese information is limited to reflective and deliberative humans.
        /// </summary>
        /// <returns></returns>
        public Point GetChosenExitLocation() {

            if (_owner.CurrentBehaviourType == CellLayerImpl.BehaviourType.Reflective
                || _owner.CurrentBehaviourType == CellLayerImpl.BehaviourType.Deliberative) {

                if (!_owner.ExitCoordinatesFromTechnicalInformation.IsEmpty) {


                    _blackboard.Get(Human.Position);



                } else {

                    if (_owner.ExitCoordinatesFromMemory.IsEmpty) {
                        throw new Exception ("PerceptionAnMemory:  no ExitCoordinatesFromMemory avalable in GetChosenExitLocation");
                    }
                    return _owner.ExitCoordinatesFromMemory;
                }
            }
            throw new Exception ("PerceptionAnMemory:  try GetChosenExitLocation with agent in reactive state");
        }*/


        /// <summary>
        ///     Calculate cell pressure depending variables.
        /// </summary>
        /// <param name="cellData"></param>
        private void RefreshPressureDependingValues(TCell cellData) {
            // Check if the pressure is higher than the human can handle with
            if (cellData.PressureOnCell > Human.ResistanceToPressure) {
                if (_owner.VitalEnergy < 5) {
                    _owner.VitalEnergy = 0;
                    _owner.SetAsKilled();
                }
                else {
                    _owner.VitalEnergy = _owner.VitalEnergy - 5;
                }
            }
        }

        private void RefreshCalmingSphereDependingValues(TCell cellData) {
            if (cellData.HasCalmingSphereByHuman) {
                _owner.FearValue -= 4;
            }
            else if (cellData.HasCalmingSphereTechnical) {
                _owner.FearValue -= 2;
            }
            else {
                _owner.FearValue -= 1;
            }
        }

        private void UpdateBehaviourType() {
            // TODO impleent changes
        }
    }

}