using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using CellLayer;
using GoapActionSystem.Implementation;
using GoapCommon.Abstract;
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

        public static readonly BlackboardProperty<Point> SelfChosenTarget =
            new BlackboardProperty<Point>("SelfChosenTarget");

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
        public readonly Blackboard HumanBlackboard = new Blackboard();
        public readonly PerceptionAndMemory Sensor;
        public readonly MotorAndNavigation MotorAndNavigation;
        private readonly CellLayerImpl _cellLayer;

        private readonly bool _helperIsGoap;
        private readonly AbstractGoapSystem _goapActionSystem;
        private List<int> _helperVariableCalmedCells = new List<int>();
        private List<int> _helperVariableMassFlightCells = new List<int>();

        /// <summary>
        ///     Create the start value und the sensor and motor classes.
        /// </summary>
        /// <param name="cellLayer"></param>
        /// <param name="behaviourType"></param>
        public Human(bool isGoap, CellLayerImpl cellLayer, CellLayerImpl.BehaviourType behaviourType = CellLayerImpl.BehaviourType.Reactive) {
            // startvalues
            HumanBlackboard.Set(ResistanceToPressure, 10);
            HumanBlackboard.Set(VitalEnergy, 20);
            HumanBlackboard.Set(Strength, 5);
            HumanBlackboard.Set(CanMove, true);
            HumanBlackboard.Set(IsAlive, true);
            HumanBlackboard.Set(BehaviourType, behaviourType);
            HumanBlackboard.Set(LastPosition, new Point());
            HumanBlackboard.Set(IsOnMassFlight, false);
            HumanBlackboard.Set(PauseMassFlightSphere, 0);
            HumanBlackboard.Set(IsOnExit, false);

            _helperIsGoap = isGoap;
            _cellLayer = cellLayer;

            //create the sensor
            Sensor = new PerceptionAndMemory(cellLayer, this, HumanBlackboard);

            // create the moving and manipuliating system/ action trigger
            MotorAndNavigation = new MotorAndNavigation(cellLayer, this, HumanBlackboard);

            // place the human on the cell field by random
            MotorAndNavigation.GetAnSetRandomPositionInCellWorld();
            HumanBlackboard.Set(RandomDirectionToFollow, MotorAndNavigation.ChooseNewRandomDirection());

            const string namespaceOfModelDefinition = "SimPanInGoapModelDefinition";
            const string nameOfConfigClass = "ReactiveConfig";

            if (isGoap) {
                _goapActionSystem =
                    GoapComponent.LoadGoapConfigurationWithSelfreference
                        (nameOfConfigClass, namespaceOfModelDefinition, HumanBlackboard, this);
            }
        }

        #region IAgent Members

        public void Tick() {
            if (HumanBlackboard.Get(IsAlive)) {
                Sensor.SenseCellInformations();
                Sensor.ProcessSensorInformations();
            }

            if (_helperIsGoap && !HumanBlackboard.Get(IsOutSide))
            {
                _goapActionSystem.GetNextAction();
            }

            if (HumanBlackboard.Get(IsAlive) && !HumanBlackboard.Get(IsOutSide)) {
                ChooseAction();
            }


            //_motorAndNavigation.Execute();
        }

        #endregion

        /// <summary>
        ///     This part will be replaced with the goap decisions.
        /// </summary>
        private void ChooseAction() {
            if (_helperIsGoap) {
                AbstractGoapAction currentAction = HumanBlackboard.Get(AbstractGoapSystem.ActionForExecution);
                currentAction.Execute();
            }
            else {
                if (HumanBlackboard.Get(IsOnExit)) {
                    MotorAndNavigation.LeaveByExit();
                }
                else {
                    if (HumanBlackboard.Get(HasCalmingSphere)) {
                        CreateOrUpdateCalmingSphere();
                    }
                    if (HumanBlackboard.Get(HasMassFlightSphere) && HumanBlackboard.Get(PauseMassFlightSphere) == 0) {
                        CreateOrUpdateMassFlightSphere();
                    }

                    if (HumanBlackboard.Get(PauseMassFlightSphere) > 0) {
                        HumanBlackboard.Set(PauseMassFlightSphere, HumanBlackboard.Get(PauseMassFlightSphere) - 1);
                        DeleteMassFlightSphere();
                    }

                    if (HumanBlackboard.Get(HasTarget)) {
                        SuccessiveApproximation();
                    }
                    else {
                        MotorAndNavigation.FollowDirectionOrSwitchDirection();

                        //WalkRandom();
                    }
                }
            }
        }

        /// <summary>
        ///     Set a target point.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void SetChosenTarget(Point targetPosition) {
            HumanBlackboard.Set(SelfChosenTarget, targetPosition);
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetCalmingSphere() {
            HumanBlackboard.Set(HasCalmingSphere, true);
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetMassFlightSphere() {
            HumanBlackboard.Set(HasMassFlightSphere, true);
        }

        /// <summary>
        ///     Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (HumanBlackboard.Get(CanMove)) {
                MotorAndNavigation.WalkAbsolutRandom();
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
                (HumanBlackboard.Get(CellIdOfPosition), radius);
            actualAffectedCellIds.Add(HumanBlackboard.Get(CellIdOfPosition));

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

            if (!HumanBlackboard.Get(LastPosition).IsEmpty) {
                Point newPosition = HumanBlackboard.Get(Position);
                Point oldPosition = HumanBlackboard.Get(LastPosition);

                if (oldPosition != newPosition) {
                    List<int> actualCellIds = GetSphereCellIDsExceptInFrontOfHuman
                        (oldPosition, newPosition, massFlightRadius);
                    //int lastCell = CellLayerImpl.CalculateCellId(_blackboard.Get(LastPosition));

                    // add the old position to the cells to populate the information.
                    //actualCellIds.Add(lastCell);
                    _cellLayer.AddMassFlightToCells(actualCellIds, HumanBlackboard.Get(Position));
                    _cellLayer.DeleteMassFlightFromCells(previousCellIds, HumanBlackboard.Get(LastPosition));

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
            List<Point> spherePoints = _cellLayer.GetNeighbourPointsInRange
                (HumanBlackboard.Get(CellIdOfPosition), radius);
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
            _cellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, HumanBlackboard.Get(Position));
            _helperVariableMassFlightCells = new List<int>();
        }

        /// <summary>
        ///     This movement type includes no planning. Distance is maximum one cell
        ///     or no movement if no closer cell is found than the actual position.
        /// </summary>
        public void SuccessiveApproximation() {
            if (HumanBlackboard.Get(CanMove)) {
                MotorAndNavigation.ApproximateToTarget(aggressiveMode: true);
            }
        }

        /// <summary>
        ///     Set the human attributes to not alive and not moving. Depending on the death simtuation
        ///     decide if the cell should be updated to obstacle.
        /// </summary>
        /// <param name="createObstacle"></param>
        public void SetAsKilled(bool createObstacle = true) {
            HumanBlackboard.Set(IsAlive, false);
            HumanBlackboard.Set(CanMove, false);

            if (HumanBlackboard.Get(HasCalmingSphere)) {
                HumanBlackboard.Set(HasCalmingSphere, false);
                DeleteCalmingSphere();
            }

            if (HumanBlackboard.Get(HasMassFlightSphere)) {
                HumanBlackboard.Set(HasMassFlightSphere, false);
                DeleteMassFlightSphere();
            }

            if (createObstacle) {
                _cellLayer.SetCellStatus(HumanBlackboard.Get(CellIdOfPosition), CellLayerImpl.CellType.Sacrifice);
                _cellLayer.DeleteAgentDraw(AgentID);
                _cellLayer.DeleteAgentIdFromCell(AgentID, HumanBlackboard.Get(Position));
                HumanBlackboard.Set(Position, new Point());
            }
            else {
                _cellLayer.UpdateAgentDrawStatus(AgentID, CellLayerImpl.BehaviourType.Dead);
            }
            HumanLayerImpl.Log.Info("i am " + AgentID + " dead.");
        }
    }

}