using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;
using CellLayer.TransportTypes;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Represens one single human in the simulation. A human is shown as a point in the simulation view.
    /// </summary>
    public class Human : IAgent {
        public const int CalmingRadius = 2;

        public static readonly BlackboardProperty<Boolean> IsOnExit =
            new BlackboardProperty<Boolean>("IsOnExit");

        public static readonly BlackboardProperty<Boolean> IsOutSide =
            new BlackboardProperty<Boolean>("IsOutSide");

        public static readonly BlackboardProperty<Boolean> IsInExitArea =
            new BlackboardProperty<Boolean>("IsInExitArea");

        public static readonly BlackboardProperty<Boolean> HasPath =
            new BlackboardProperty<Boolean>("HasPath");

        public static readonly BlackboardProperty<Boolean> HasTarget =
            new BlackboardProperty<Boolean>("HasTarget");

        public static readonly BlackboardProperty<Boolean> KnowsExitLocation =
            new BlackboardProperty<Boolean>("KnowsExitLocation");

        public static readonly BlackboardProperty<Point> Target =
            new BlackboardProperty<Point>("Target");

        public static readonly BlackboardProperty<Boolean> MovementFailed =
            new BlackboardProperty<Boolean>("MovementFailed");

        public static readonly BlackboardProperty<Point> Position =
            new BlackboardProperty<Point>("Position");

        public static readonly BlackboardProperty<Point> LastPosition =
            new BlackboardProperty<Point>("LastPosition");

        public static readonly BlackboardProperty<int> CellIdOfPosition =
            new BlackboardProperty<int>("CellIdOfPosition");


        public static readonly BlackboardProperty<int> ResistanceToPressure =
            new BlackboardProperty<int>("ResistanceToPressure");

        public static readonly BlackboardProperty<int> VitalEnergy =
            new BlackboardProperty<int>("VitalEnergy");

        public static readonly BlackboardProperty<int> Strength =
            new BlackboardProperty<int>("Strength");

        public static readonly BlackboardProperty<Boolean> CanMove =
            new BlackboardProperty<Boolean>("CanMove");

        public static readonly BlackboardProperty<bool> IsAlive =
            new BlackboardProperty<Boolean>("IsAlive");

        public static readonly BlackboardProperty<int> FearLevel =
            new BlackboardProperty<int>("FearLevel");

        public static readonly BlackboardProperty<bool> HasCalmingSphere =
            new BlackboardProperty<bool>("HasCalmingSphere");

        public static readonly BlackboardProperty<bool> HasMassFlightSphere =
            new BlackboardProperty<bool>("HasMassFlightSphere");

        public static readonly BlackboardProperty<int> PauseMassFlightSphere =
            new BlackboardProperty<int>("PauseMassFlightSphere");

        public static readonly BlackboardProperty<bool> IsOnMassFlight =
            new BlackboardProperty<bool>("IsOnMassFlight");

        public static readonly BlackboardProperty<CellLayerImpl.Direction> RandomDirectionToFollow =
            new BlackboardProperty<CellLayerImpl.Direction>("RandomDirectionToFollow");

        public static readonly BlackboardProperty<CellLayerImpl.BehaviourType> BehaviourType =
            new BlackboardProperty<CellLayerImpl.BehaviourType>("BehaviourType");


        public readonly Guid AgentID = Guid.NewGuid();
        private readonly Blackboard _blackboard = new Blackboard();
        private readonly PerceptionAndMemory _sensor;
        private readonly MotorAndNavigation _motorAndNavigation;
        private readonly CellLayerImpl _cellLayer;

        private List<int> _helperVariableCalmedCells = new List<int>();
        private List<int> _helperVariableMassFlightCells = new List<int>();

        /// <summary>
        ///     Create the start value und the sensor and motor classes.
        /// </summary>
        /// <param name="cellLayer"></param>
        /// <param name="behaviourType"></param>
        public Human
            (CellLayerImpl cellLayer, CellLayerImpl.BehaviourType behaviourType = CellLayerImpl.BehaviourType.Reactive) {
            // startvalues
            _blackboard.Set(ResistanceToPressure, 10);
            _blackboard.Set(VitalEnergy, 20);
            _blackboard.Set(Strength, 5);
            _blackboard.Set(CanMove, true);
            _blackboard.Set(IsAlive, true);
            _blackboard.Set(BehaviourType, behaviourType);
            _blackboard.Set(LastPosition, new Point());
            _blackboard.Set(IsOnMassFlight, false);
            _blackboard.Set(PauseMassFlightSphere,0);

            _cellLayer = cellLayer;

            //create the sensor
            _sensor = new PerceptionAndMemory(cellLayer, this, _blackboard);

            // create the moving and manipuliating system/ action trigger
            _motorAndNavigation = new MotorAndNavigation(cellLayer, this, _blackboard);

            // place the human on the cell field by random
            _motorAndNavigation.GetAnSetRandomPositionInCellWorld();
            _blackboard.Set(RandomDirectionToFollow, _motorAndNavigation.ChooseNewRandomDirection());

            //AbstractGoapSystem goapActionSystem = GoapActionSystem.Implementation.GoapComponent.LoadGoapConfiguration
            //  ("eine klasse", "ein namespace", blackboard);
        }

        #region IAgent Members

        public void Tick() {
            if (_blackboard.Get(IsAlive)) {
                _sensor.SenseCellInformations();
                ProcessSensorInformations();
            }

            if (_blackboard.Get(IsAlive) && !_blackboard.Get(IsOutSide)) {
                ChooseAction();
            }

            //_motorAndNavigation.Execute();
        }

        #endregion

        /// <summary>
        ///     This part will be replaced with the goap decisions.
        /// </summary>
        private void ChooseAction() {
            if (_blackboard.Get(IsOnExit)) {
                LeaveCellFieldByExit();
            }
            else {
                if (_blackboard.Get(HasCalmingSphere)) {
                    CreateOrUpdateCalmingSphere();
                }
                if (_blackboard.Get(HasMassFlightSphere) && _blackboard.Get(PauseMassFlightSphere) == 0) {
                    CreateOrUpdateMassFlightSphere();
                }

                if (_blackboard.Get(PauseMassFlightSphere) > 0) {
                    _blackboard.Set(PauseMassFlightSphere, _blackboard.Get(PauseMassFlightSphere) - 1);
                    DeleteMassFlightSphere();
                }

                if (_blackboard.Get(HasTarget)) {
                    SuccessiveApproximation();
                }
                else {
                    _motorAndNavigation.FollowDirectionOrSwitchDirection();

                    //WalkRandom();
                }
            }
        }

        /// <summary>
        ///     Set a target point.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void SetTarget(Point targetPosition) {
            _blackboard.Set(Target, targetPosition);
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetCalmingSphere() {
            _blackboard.Set(HasCalmingSphere, true);
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetMassFlightSphere() {
            _blackboard.Set(HasMassFlightSphere, true);
        }

        /// <summary>
        ///     Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (_blackboard.Get(CanMove)) {
                _motorAndNavigation.WalkAbsolutRandom();
            }
        }

        /// <summary>
        ///     Used if a panik event kills a human. do not change cell data
        ///     because the event agent changes a whoe set of cells after killing agents.
        /// </summary>
        public void GetKilledByPanicArea() {
            SetAsKilled(createObstacle: false);
        }

        /// <summary>
        ///     Create calming information area around the human. If the agent changes his position,
        ///     the spere must be updated and some cell have to loose effect or add the effect.
        ///     Identifies the cells which have to change value for cheaper calcutations.
        /// </summary>
        public void CreateOrUpdateCalmingSphere(int radius = CalmingRadius) {
            List<int> previousCalmedCellIds = _helperVariableCalmedCells;
            List<int> actualAffectedCellIds = _cellLayer.GetNeighbourCellIdsInRange
                (_blackboard.Get(CellIdOfPosition), radius);
            actualAffectedCellIds.Add(_blackboard.Get(CellIdOfPosition));

            //get all elements from previous cell id list which are not in actual cell id list
            List<int> cellIdsForEffectDeletion = previousCalmedCellIds.Except(actualAffectedCellIds).ToList();

            //get all elements from actual cell id list which are not in prevoiusc cell id list
            List<int> cellIdsForEffectAddition = actualAffectedCellIds.Except(previousCalmedCellIds).ToList();

            _cellLayer.DeleteHumanCalmingFromCells(cellIdsForEffectDeletion, AgentID);
            _cellLayer.AddHumanCalmingToCells(cellIdsForEffectAddition, AgentID);

            _helperVariableCalmedCells = actualAffectedCellIds;
        }

        /// <summary>
        ///     Update or add the massflight coordinates on the cells behind the human. For this calculation
        ///     the direction is needed. Direction is calculable by difference of positions.
        ///     All cells have to get updated because human is changing position and this position
        ///     is the populated and the old entrys get deleted information.
        /// </summary>
        /// <param name="massFlightRadius"></param>
        public void CreateOrUpdateMassFlightSphere(int massFlightRadius = 2) {
            List<int> previousCellIds = _helperVariableMassFlightCells;

            if (!_blackboard.Get(LastPosition).IsEmpty) {
                Point newPosition = _blackboard.Get(Position);
                Point oldPosition = _blackboard.Get(LastPosition);

                if (oldPosition != newPosition) {
                    List<int> actualCellIds = GetSphereCellIDsExceptInFrontOfHuman
                        (oldPosition, newPosition, massFlightRadius);
                    //int lastCell = CellLayerImpl.CalculateCellId(_blackboard.Get(LastPosition));

                    // add the old position to the cells to populate the information.
                    //actualCellIds.Add(lastCell);
                    _cellLayer.AddMassFlightToCells(actualCellIds, _blackboard.Get(Position));
                    _cellLayer.DeleteMassFlightFromCells(previousCellIds, _blackboard.Get(LastPosition));

                    _helperVariableMassFlightCells = actualCellIds;
                }
            }
        }

        /// <summary>
        ///     Get the cell ids from behind the human. Precondition are an actual and old position of the human plus
        ///     a diffeence between them.
        /// </summary>
        /// <param name="oldPosition"></param>
        /// <param name="newPosition"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        private List<int> GetSphereCellIDsExceptInFrontOfHuman(Point oldPosition, Point newPosition, int radius) {
            List<Point> spherePoints = _cellLayer.GetNeighbourPointsInRange(_blackboard.Get(CellIdOfPosition), radius);
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
            if (xDiff == 0 || yDiff == 0) {
                if (xDiff == 0 && yDiff > 0) {
                    filtered = spherePoints.FindAll(point => !(point.Y > newY));
                }
                else if (xDiff == 0 && yDiff < 0) {
                    filtered = spherePoints.FindAll(point => !(point.Y < newY));
                }
                else if (yDiff == 0 && xDiff > 0) {
                    filtered = spherePoints.FindAll(point => !(point.X > newX));
                }
                else if (yDiff == 0 && xDiff < 0) {
                    filtered = spherePoints.FindAll(point => !(point.X < newX));
                }
            }
            else {
                // calculation for walking diagonal.
                if (xDiff < 0 && yDiff < 0) {
                    filtered = spherePoints.FindAll(point => !(point.X <= newX && point.Y <= newY));
                }
                else if (xDiff > 0 && yDiff < 0) {
                    filtered = spherePoints.FindAll(point => !(point.X >= newX && point.Y <= newY));
                }
                else if (xDiff < 0 && yDiff > 0) {
                    filtered = spherePoints.FindAll(point => !(point.X <= newX && point.Y >= newY));
                }
                else if (xDiff > 0 && yDiff > 0) {
                    filtered = spherePoints.FindAll(point => !(point.X >= newX && point.Y >= newY));
                }
            }
            foreach (Point point in filtered) {
                filteredIds.Add(CellLayerImpl.CalculateCellId(point));
            }
            return filteredIds;
        }


        /// <summary>
        ///     Delete the calming sphere of the human.
        /// </summary>
        public void DeleteCalmingSphere() {
            _cellLayer.DeleteHumanCalmingFromCells(_helperVariableCalmedCells, AgentID);
            _helperVariableCalmedCells = new List<int>();
        }

        /// <summary>
        ///     If human dies the sphere have to be deleted. So identify cells, update and redraw.
        /// </summary>
        public void DeleteMassFlightSphere() {
            //_cellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, _blackboard.Get(LastPosition));
            _cellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, _blackboard.Get(Position));
            _helperVariableMassFlightCells = new List<int>();
        }

        /// <summary>
        ///     This movement type includes no planning. Distance is maximum one cell
        ///     or no movement if no closer cell is found than the actual position.
        /// </summary>
        public void SuccessiveApproximation() {
            if (_blackboard.Get(CanMove)) {
                _motorAndNavigation.ApproximateToTarget(aggressiveMode: true);
            }
        }

        /// <summary>
        ///     Rework the informations by the environment. Refresh dependent variables.
        ///     This step is needed after collecting sensor data from the cell.
        /// </summary>
        private void ProcessSensorInformations() {
            // Inspect the new data of the cell the agent is on.
            int cellID = _blackboard.Get(CellIdOfPosition);
            TCell cellData = _sensor.CellIdToData[cellID];

            RefreshTargetDepedingInformation(cellData);

            RefreshPressureDependingValues(cellData);

            if (_blackboard.Get(MovementFailed) && _blackboard.Get(HasMassFlightSphere)) {
                _blackboard.Set(PauseMassFlightSphere,2);
            }

            if (cellData.IsExit) {
                _blackboard.Set(IsOnExit, true);
            }
        }

        /// <summary>
        ///     If a human is on a exit cell he has to leave the field.
        /// </summary>
        public void LeaveCellFieldByExit() {
            _motorAndNavigation.LeaveByExit();
        }

        /// <summary>
        ///     Calculate cell pressure depending variables.
        /// </summary>
        /// <param name="cellData"></param>
        private void RefreshPressureDependingValues(TCell cellData) {
            // Check if the pressure is higher than the human can handle with
            if (cellData.PressureOnCell > _blackboard.Get(ResistanceToPressure)) {
                int vitalEnergy = _blackboard.Get(VitalEnergy);
                if (vitalEnergy < 5) {
                    _blackboard.Set(VitalEnergy, 0);
                    SetAsKilled();
                }
                else {
                    _blackboard.Set(VitalEnergy, vitalEnergy - 5);
                }
            }
        }

        /// <summary>
        ///     Refresh the data depending on target informations on the cell. A reactive human cannot access the technical target
        ///     information. He will only follow a mass flight information. an deliberativ or reflective human will only follow his
        ///     own information by memory of getting into the room or technical source.
        /// </summary>
        /// <param name="cellData"></param>
        private void RefreshTargetDepedingInformation(TCell cellData) {
            if (!cellData.DominantMassFlightLeaderCoordinates.IsEmpty && !_blackboard.Get(HasMassFlightSphere)) {
                _blackboard.Set(Target, cellData.DominantMassFlightLeaderCoordinates);
                _blackboard.Set(HasTarget, true);
                _blackboard.Set(IsOnMassFlight, true);
            }
            else {
                _blackboard.Set(Target, new Point());
                _blackboard.Set(HasTarget, false);
            }
        }

        /// <summary>
        ///     Set the human attributes to not alive and not moving. Depending on the death simtuation
        ///     decide if the cell should be updated to obstacle.
        /// </summary>
        /// <param name="createObstacle"></param>
        public void SetAsKilled(bool createObstacle = true) {
            _blackboard.Set(IsAlive, false);
            _blackboard.Set(CanMove, false);

            if (_blackboard.Get(HasCalmingSphere)) {
                _blackboard.Set(HasCalmingSphere, false);
                DeleteCalmingSphere();
            }

            if (_blackboard.Get(HasMassFlightSphere)) {
                _blackboard.Set(HasMassFlightSphere, false);
                DeleteMassFlightSphere();
            }

            if (createObstacle) {
                _cellLayer.SetCellStatus(_blackboard.Get(CellIdOfPosition), CellLayerImpl.CellType.Sacrifice);
                _cellLayer.DeleteAgentDraw(AgentID);
                _cellLayer.DeleteAgentIdFromCell(AgentID, _blackboard.Get(Position));
                _blackboard.Set(Position, new Point());
            }
            else {
                _cellLayer.UpdateAgentDrawStatus(AgentID, CellLayerImpl.BehaviourType.Dead);
            }
            HumanLayerImpl.Log.Info("i am " + AgentID + " dead.");
        }
    }

}