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
        public const int Strength = 5;
        public const int ResistanceToPressure = 10;

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

        public static readonly BlackboardProperty<List<CellLayerImpl.Direction>> Path =
            new BlackboardProperty<List<CellLayerImpl.Direction>>("Path");

        public readonly Guid AgentID = Guid.NewGuid();
        public readonly Blackboard HumanBlackboard = new Blackboard();
        public readonly PerceptionAndMemory Sensor;
        public readonly MotorAndNavigation MotorAndNavigation;
        public readonly CellLayerImpl CellLayer;
        private readonly bool _helperIsGoap;
        private readonly AbstractGoapSystem _goapActionSystem;

        public bool IsAlive = true;
        public int VitalEnergy = 20;
        public bool CanMove = true;
        public CellLayerImpl.BehaviourType CurrentBehaviourType;

        private List<int> _helperVariableCalmedCells = new List<int>();
        private List<int> _helperVariableMassFlightCells = new List<int>();
        public bool HasMassFlightSphere;
        public int PauseMassFlightSphere;
        public bool HasCalmingSphere;
        public bool IsOnMassFlight;

        public Point MassFlightTargetCoordinates = new Point();
        public Point ExitCoordinatesFromMemory = new Point(11, 20);
        //public Point ExitCoordinatesFromTechnicalInformation = new Point();
        public int FearValue;
        public CellLayerImpl.Direction RandomDirectionToFollow;

        /// <summary>
        ///     Create the start value und the sensor and motor classes.
        /// </summary>
        /// <param name="isGoap"></param>
        /// <param name="cellLayer"></param>
        /// <param name="behaviourType"></param>
        public Human
            (bool isGoap,
                CellLayerImpl cellLayer,
                CellLayerImpl.BehaviourType behaviourType) {
            // startvalues
            CurrentBehaviourType = behaviourType;
            FearValue = GetRandomisedFearValue(behaviourType);
            HumanBlackboard.Set(LastPosition, new Point());
            IsOnMassFlight = false;
            HumanBlackboard.Set(IsOnExit, false);
            _helperIsGoap = isGoap;
            CellLayer = cellLayer;

            //create the sensor
            Sensor = new PerceptionAndMemory(cellLayer, this, HumanBlackboard);

            // create the moving and manipuliating system/ action trigger
            MotorAndNavigation = new MotorAndNavigation(cellLayer, this, HumanBlackboard);

            // place the human on the cell field by random
            MotorAndNavigation.GetAnSetRandomPositionInCellWorld();
            RandomDirectionToFollow = MotorAndNavigation.ChooseNewRandomDirection();

            const string namespaceOfModelDefinition = "SimPanInGoapModelDefinition";

           
            string nameOfConfigClass = null;
            if (isGoap) {

                switch (behaviourType) {
                    case CellLayerImpl.BehaviourType.Reactive:
                        nameOfConfigClass = "ReactiveConfig";
                        break;
                    case CellLayerImpl.BehaviourType.Deliberative:
                        nameOfConfigClass = "DeliberativeConfig";
                        HumanBlackboard.Set(KnowsExitLocation,true);
                        break;
                    case CellLayerImpl.BehaviourType.Reflective:
                        nameOfConfigClass = "ReflectiveConfig";
                        HumanBlackboard.Set(KnowsExitLocation, true);
                        HasCalmingSphere = true;
                        break;
                }

                if (nameOfConfigClass == null) {
                    throw new ArgumentException("Human: BehaviourType of agent is not known.");
                }
                _goapActionSystem =
                    GoapComponent.LoadGoapConfigurationWithSelfreference
                        (nameOfConfigClass, namespaceOfModelDefinition, HumanBlackboard, this);
            }
        }

        #region IAgent Members

        public void Tick() {
            if (IsAlive) {
                Sensor.SenseCellInformations();
                Sensor.ProcessSensorInformations();
            }

            if (IsAlive && _helperIsGoap && !HumanBlackboard.Get(IsOutSide))
            {
                AbstractGoapAction action = _goapActionSystem.GetNextAction();
                if (action != null) {
                    MotorAndNavigation.ExecuteGoapAction();
                }
            }
            else if (IsAlive && !HumanBlackboard.Get(IsOutSide)) {
                ChooseAction();
            }
        }

        #endregion

        /// <summary>
        ///     Get a randomised value depending on the behaviour type.
        /// </summary>
        /// <param name="behaviour"></param>
        /// <returns></returns>
        private int GetRandomisedFearValue(CellLayerImpl.BehaviourType behaviour) {
            Random rand = new Random();
            if (behaviour == CellLayerImpl.BehaviourType.Reactive) {
                return rand.Next(45, 101);
            }
            if (behaviour == CellLayerImpl.BehaviourType.Deliberative) {
                return rand.Next(15, 45);
            }
            if (behaviour == CellLayerImpl.BehaviourType.Reflective) {
                return rand.Next(1, 15);
            }
            throw new ArgumentException("Human: No matching in behaviour types at GetRandomisedFearValue");
        }

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
                    if (HasCalmingSphere) {
                        CreateOrUpdateCalmingSphere();
                    }
                    if (HasMassFlightSphere && PauseMassFlightSphere == 0)
                    {
                        CreateOrUpdateMassFlightSphere();
                    }

                    if (PauseMassFlightSphere > 0) {
                        PauseMassFlightSphere -= 1;
                        DeleteMassFlightSphere();
                    }

                    if (HumanBlackboard.Get(HasTarget)) {
                        if (CanMove) {
                            MotorAndNavigation.ApproximateToTarget(aggressiveMode: true);
                        }
                    }
                    else {
                        MotorAndNavigation.FollowDirectionOrSwitchDirection();

                        //WalkRandom();
                    }
                }
            }
        }

        public bool HasValidPath() {
            if (HumanBlackboard.Get(HasPath)) {
                if (HumanBlackboard.Get(Human.Path) != null && HumanBlackboard.Get(Human.Path).Count != 0) {
                    return true;
                } 
            }
            return false;
        }

        public bool DeleteFirstDirectionFromPath() {
            List<CellLayerImpl.Direction> directionsList = HumanBlackboard.Get(Path);
            if ( directionsList != null && directionsList.Count != 0) {
                directionsList.RemoveAt(0);
                HumanBlackboard.Set(Path, directionsList);
                }
            return false;
        }

        /// <summary>
        ///     Set a target point.
        /// </summary>
        /// <param name="targetPosition"></param>
        public void SetChosenTarget(Point targetPosition) {
            HumanBlackboard.Set(HasTarget, true);
            HumanBlackboard.Set(Target, targetPosition);
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetCalmingSphere() {
            HasCalmingSphere = true;
        }

        /// <summary>
        ///     The entry on the blackboard is the trigger for creation of a sphere.
        /// </summary>
        public void SetMassFlightSphere() {
            HasMassFlightSphere = true;
        }

        /// <summary>
        ///     Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (CanMove) {
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
            List<int> actualAffectedCellIds = CellLayer.GetNeighbourCellIdsInRange
                (HumanBlackboard.Get(CellIdOfPosition), radius);
            actualAffectedCellIds.Add(HumanBlackboard.Get(CellIdOfPosition));

            //get all elements from previous cell id list which are not in actual cell id list
            List<int> cellIdsForEffectDeletion = previousCalmedCellIds.Except(actualAffectedCellIds).ToList();

            //get all elements from actual cell id list which are not in prevoiusc cell id list
            List<int> cellIdsForEffectAddition = actualAffectedCellIds.Except(previousCalmedCellIds).ToList();

            CellLayer.DeleteHumanCalmingFromCells(cellIdsForEffectDeletion, AgentID);
            CellLayer.AddHumanCalmingToCells(cellIdsForEffectAddition, AgentID);

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
                    CellLayer.AddMassFlightToCells(actualCellIds, HumanBlackboard.Get(Position));
                    CellLayer.DeleteMassFlightFromCells(previousCellIds, HumanBlackboard.Get(LastPosition));

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
            List<Point> spherePoints = CellLayer.GetNeighbourPointsInRange
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
            CellLayer.DeleteHumanCalmingFromCells(_helperVariableCalmedCells, AgentID);
            HasCalmingSphere = false;
            _helperVariableCalmedCells = new List<int>();
        }

        /// <summary>
        ///     If human dies the sphere have to be deleted. So identify cells, update and redraw.
        /// </summary>
        public void DeleteMassFlightSphere() {
            //_cellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, _blackboard.Get(LastPosition));
            CellLayer.DeleteMassFlightFromCells(_helperVariableMassFlightCells, HumanBlackboard.Get(Position));
            HasMassFlightSphere = false;
            _helperVariableMassFlightCells = new List<int>();
        }

        /// <summary>
        ///     Set the human attributes to not alive and not moving. Depending on the death simtuation
        ///     decide if the cell should be updated to obstacle.
        /// </summary>
        /// <param name="createObstacle"></param>
        public void SetAsKilled(bool createObstacle = true) {
            IsAlive = false;
            CanMove = false;

            if (HasCalmingSphere) {
                HasCalmingSphere = false;
                DeleteCalmingSphere();
            }

            if (HasMassFlightSphere){
                HasMassFlightSphere = false;
                DeleteMassFlightSphere();
            }

            if (createObstacle) {
                CellLayer.SetCellStatus(HumanBlackboard.Get(CellIdOfPosition), CellLayerImpl.CellType.Sacrifice);
                CellLayer.DeleteAgentDraw(AgentID);
                CellLayer.DeleteAgentIdFromCell(AgentID, HumanBlackboard.Get(Position));
                HumanBlackboard.Set(Position, new Point());
            }
            else {
                CellLayer.UpdateAgentDrawStatus(AgentID, CellLayerImpl.BehaviourType.Dead);
            }
            HumanLayerImpl.Log.Info("i am " + AgentID + " dead.");
        }
    }

}