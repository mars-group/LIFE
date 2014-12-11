using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;
using CellLayer.TransportTypes;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Provide the sensor data collection from cells, the interpretation of the data depending on the
    ///     humans bahaviour type and refresh data depending influence on cells.
    /// </summary>
    public class PerceptionAndMemory {
        private readonly CellLayerImpl _dataSourceLayer;
        private readonly Human _owner;
        private TCell _currentCellData;
        private readonly Blackboard _blackboard;

        private List<int> _helperVariableCalmedCells = new List<int>();
        private List<int> _helperVariableMassFlightCells = new List<int>();

        public PerceptionAndMemory(CellLayerImpl celllayer, Human owner, Blackboard blackboard) {
            _dataSourceLayer = celllayer;
            _owner = owner;
            _blackboard = blackboard;
        }

        /// <summary>
        ///     Get the data of the cell the agent is standing on and save it to the humans map. The data is collected
        ///     by a transpport type object so no direct refernce is needed.
        /// </summary>
        public void SenseCellInformations(){
            int cellId = _blackboard.Get(Human.CellIdOfPosition);
            _currentCellData = _dataSourceLayer.GetDataOfCell(cellId);
        }

        /// <summary>
        ///     Rework the informations by the environment. Refresh dependent variables.
        ///     This step is needed after collecting sensor data from the cell.
        /// </summary>
        public void ProcessSensorInformations() {
            _blackboard.Set(Human.IsOnExit, _currentCellData.IsExit);
            _blackboard.Set(Human.IsInExitArea, _currentCellData.IsExitArea);

            RefreshCalmingSphereDependingValues(_currentCellData);
            UpdateBehaviourType();
            RefreshPressureDependingValues(_currentCellData);
            
            switch (_owner.CurrentBehaviourType) {
                case CellLayerImpl.BehaviourType.Reactive:
                    ProcessSensorInformationsReactive(_currentCellData);
                    break;
                case CellLayerImpl.BehaviourType.Deliberative:
                    ProcessSensorInformationsDeliberative(_currentCellData);
                    break;
                case CellLayerImpl.BehaviourType.Reflective:
                    ProcessSensorInformationsReflective(_currentCellData);
                    break;
            }
        }

        /// <summary>
        ///     Sensor information processing for human in status reflective. The target is 
        ///     the exit position in the map, baceause they can remember the way they came in.
        ///     Reflective human can share their exit informations and so they lead the mass flight.
        /// </summary>
        /// <param name="cellData"></param>
        private void ProcessSensorInformationsReflective(TCell cellData) {
            if (_blackboard.Get(Human.IsInExitArea)) {
                _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                _blackboard.Set(Human.HasTarget, true);
            }

            if (_owner.HasCalmingSphere) {
                CreateOrUpdateCalmingSphere();
            }
            else {
                DeleteCalmingSphere();
            }

            if (_blackboard.Get(Human.MovementFailed) && _owner.HasMassFlightSphere) {
                _owner.PauseMassFlightSphere = 2;
            }

            // if there are target informations this human can lead a mass flight
            _owner.HasMassFlightSphere = _blackboard.Get(Human.HasTarget);

            if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere == 0) {
                CreateOrUpdateMassFlightSphere();
            }
            else if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere > 0) {
                DeleteMassFlightSphere();
            }
        }

        /// <summary>
        ///     Create calming information area around the human. If the agent changes his position,
        ///     the spere must be updated and some cell have to loose effect or add the effect.
        ///     Identifies the cells which have to change value for cheaper calcutations.
        /// </summary>
        public void CreateOrUpdateCalmingSphere(int radius = Human.CalmingRadius)
        {
            List<int> previousCalmedCellIds = _helperVariableCalmedCells;
            List<int> actualAffectedCellIds = _dataSourceLayer.GetNeighbourCellIdsInRange
                (_blackboard.Get(Human.CellIdOfPosition), radius);
            actualAffectedCellIds.Add(_blackboard.Get(Human.CellIdOfPosition));

            //get all elements from previous cell id list which are not in actual cell id list
            List<int> cellIdsForEffectDeletion = previousCalmedCellIds.Except(actualAffectedCellIds).ToList();

            //get all elements from actual cell id list which are not in prevoiusc cell id list
            List<int> cellIdsForEffectAddition = actualAffectedCellIds.Except(previousCalmedCellIds).ToList();

            _dataSourceLayer.DeleteHumanCalmingFromCells(cellIdsForEffectDeletion, _owner.AgentID);
            _dataSourceLayer.AddHumanCalmingToCells(cellIdsForEffectAddition, _owner.AgentID);

            _helperVariableCalmedCells = actualAffectedCellIds;
        }

        /// <summary>
        ///     Update or add the massflight coordinates on the cells behind the human. For this calculation
        ///     the direction is needed. Direction is calculable by difference of positions.
        ///     All cells have to get updated because human is changing position and this position
        ///     is the populated and the old entrys get deleted information.
        /// </summary>
        /// <param name="massFlightRadius"></param>
        public void CreateOrUpdateMassFlightSphere(int massFlightRadius = Human.MassFlightRadius)
        {
            List<int> previousCellIds = _helperVariableMassFlightCells;

            if (!_blackboard.Get(Human.LastPosition).IsEmpty)
            {
                Point newPosition = _blackboard.Get(Human.Position);
                Point oldPosition = _blackboard.Get(Human.LastPosition);

                if (oldPosition != newPosition)
                {
                    List<int> actualCellIds = GetSphereCellIDsExceptInFrontOfHuman
                        (oldPosition, newPosition, massFlightRadius);
                    //int lastCell = CellLayerImpl.CalculateCellId(_blackboard.Get(LastPosition));

                    // add the old position to the cells to populate the information.
                    //actualCellIds.Add(lastCell);
                    _dataSourceLayer.AddMassFlightToCells(actualCellIds, _blackboard.Get(Human.Position));
                    _dataSourceLayer.DeleteMassFlightFromCells(previousCellIds, _blackboard.Get(Human.LastPosition));

                    _helperVariableMassFlightCells = actualCellIds;
                }
            }
        }

        /// <summary>
        ///     Sensor information processing for human in status deliberative. The target is 
        ///     the exit position in the map, baceause they can remember the way they came in.
        ///     Deliberatve human can share their exit informations and so they lead the mass flight.
        ///     Additionaly they got a calming sphere and surrounding humans reduce their fear faster.
        /// </summary>
        /// <param name="cellData"></param>
        private void ProcessSensorInformationsDeliberative(TCell cellData) {
            if (_blackboard.Get(Human.IsInExitArea)) {
                _blackboard.Set(Human.Target, cellData.ExitAreaInformation);
                _blackboard.Set(Human.HasTarget, true);
            }

            if (_blackboard.Get(Human.MovementFailed) && _owner.HasMassFlightSphere) {
                _owner.PauseMassFlightSphere = 2;
            }

            // if there are target informations this human can lead a mass flight
            _owner.HasMassFlightSphere = _blackboard.Get(Human.HasTarget);

            if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere == 0) {
                CreateOrUpdateMassFlightSphere();
            }
            else if (_owner.HasMassFlightSphere && _owner.PauseMassFlightSphere > 0) {
                DeleteMassFlightSphere();
            }
        }




        /// <summary>
        ///     Delete the calming sphere of the human.
        /// </summary>
        public void DeleteCalmingSphere() {
            _dataSourceLayer.DeleteHumanCalmingFromCells(_helperVariableCalmedCells, _owner.AgentID);
            _owner.HasCalmingSphere = false;
            _helperVariableCalmedCells = new List<int>();
        }

        /// <summary>
        ///     If human dies the sphere have to be deleted. So identify cells, update and redraw.
        /// </summary>
        public void DeleteMassFlightSphere() {
            //_cellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, _blackboard.Get(LastPosition));
            _dataSourceLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, _blackboard.Get(Human.Position));
            _owner.HasMassFlightSphere = false;
            _helperVariableMassFlightCells = new List<int>();
        }



        /// <summary>
        ///     Get the cell ids from behind the human. Precondition are an actual and old position of the human plus
        ///     a diffeence between them.
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="newPosition"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private List<int> GetSphereCellIDsExceptInFrontOfHuman(Point oldPosition, Point newPosition, int radius)
        {
            List<Point> spherePoints = _dataSourceLayer.GetNeighbourPointsInRange
                (_blackboard.Get(Human.CellIdOfPosition), radius);
            List<Point> filtered = new List<Point>();
            List<int> filteredIds = new List<int>();
            int newX = newPosition.X;
            int newY = newPosition.Y;
            int oldX = oldPosition.X;
            int oldY = oldPosition.Y;

            // the difference in x and y is the base of calculation.
            int xDiff = newX - oldX;
            int yDiff = newY - oldY;

            // calculation for walking horizonttal or vertical.
            if (xDiff == 0 || yDiff == 0)
            {
                if (xDiff == 0 && yDiff > 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.Y > newY));
                }
                else if (xDiff == 0 && yDiff < 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.Y < newY));
                }
                else if (yDiff == 0 && xDiff > 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X > newX));
                }
                else if (yDiff == 0 && xDiff < 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X < newX));
                }
            }
            else
            {
                // calculation for walking diagonal.
                if (xDiff < 0 && yDiff < 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X <= newX && point.Y <= newY));
                }
                else if (xDiff > 0 && yDiff < 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X >= newX && point.Y <= newY));
                }
                else if (xDiff < 0 && yDiff > 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X <= newX && point.Y >= newY));
                }
                else if (xDiff > 0 && yDiff > 0)
                {
                    filtered = spherePoints.FindAll(point => !(point.X >= newX && point.Y >= newY));
                }
            }
            foreach (Point point in filtered)
            {
                filteredIds.Add(CellLayerImpl.CalculateCellId(point));
            }
            return filteredIds;
        }

        /// <summary>
        ///     Reactive human will follow a mass flight leader until he is in an exit area. If
        ///     both targets are not given he cannot get a target.
        /// </summary>
        /// <param name="cellData"></param>
        private void ProcessSensorInformationsReactive(TCell cellData) {
            if (!cellData.DominantMassFlightLeaderCoordinates.IsEmpty || _blackboard.Get(Human.IsInExitArea)) {
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
                _owner.IsOnMassFlight = false;
            }
        }

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

        /// <summary>
        ///     The fear level changes on every tick. human can reduce their fear by one without any influence.
        ///     If there is an calming influence by a technical source or human the human reduce fear by more
        ///     than one.
        /// </summary>
        /// <param name="cellData"></param>
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