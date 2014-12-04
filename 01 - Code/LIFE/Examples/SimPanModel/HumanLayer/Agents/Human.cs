using System;
using System.Drawing;
using CellLayer;
using CellLayer.TransportTypes;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Represens one single human in the simulation. A human is shown as a point in the simulation view.
    /// </summary>
    public class Human : IAgent {
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

        public static readonly BlackboardProperty<Boolean> IsAlive =
            new BlackboardProperty<Boolean>("IsAlive");

        public static readonly BlackboardProperty<int> FearLevel =
            new BlackboardProperty<int>("FearLevel");


        public readonly Guid AgentID = Guid.NewGuid();
        private readonly Blackboard _blackboard = new Blackboard();
        private readonly PerceptionAndMemory _sensor;
        private readonly MotorAndNavigation _motorAndNavigation;
        private readonly CellLayerImpl _cellLayer;


        public Human(CellLayerImpl cellLayer) {

            // startvalues
            _blackboard.Set(ResistanceToPressure, 10);
            _blackboard.Set(VitalEnergy, 10);
            _blackboard.Set(Strength, 5);
            _blackboard.Set(CanMove, true);
            _blackboard.Set(IsAlive, true);
            _cellLayer = cellLayer;
            
            //create the sensor
            _sensor = new PerceptionAndMemory(cellLayer, this, _blackboard);

            // create the moving and manipuliating system/ action trigger
            _motorAndNavigation = new MotorAndNavigation(cellLayer, this, _blackboard);

            // place the human on the cell field by random
            _motorAndNavigation.GetAnSetRandomPositionInCellWorld();


            //AbstractGoapSystem goapActionSystem = GoapActionSystem.Implementation.GoapComponent.LoadGoapConfiguration
            //  ("eine klasse", "ein namespace", blackboard);
        }

        #region IAgent Members

        public void Tick() {
            if (_blackboard.Get(IsAlive)) {
                _sensor.SenseCellInformations();
                ProcessSensorInformations();
            }

            if (_blackboard.Get(IsAlive)) {
                ChooseAction();
            }
            //_motorAndNavigation.Execute();
        }

        #endregion

        private void ChooseAction() {
            if (_blackboard.Get(HasTarget)) {
                SuccessiveApproximation();
            }
            else {
                WalkRandom();
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
        ///     Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (_blackboard.Get(CanMove)) {
                _motorAndNavigation.WalkRandom();
            }
        }

        /// <summary>
        ///     Used if a panik event kills a human.
        /// </summary>
        public void GetKilled() {
            _sensor.SetAsKilled();
            HumanLayerImpl.Log.Info("i am " + AgentID + " dead.");
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
            // Check if there are point informations about a target. Refresh the corresponding boolean Value.
            Point target = _blackboard.Get(Target);
            _blackboard.Set(HasTarget, !target.IsEmpty);

            // Inspect the new data of the cell the agent is on.
            int cellID = _blackboard.Get(CellIdOfPosition);
            TCell cellData = _sensor.CellIdToData[cellID];

            RefreshPressureDependingValues(cellData);

            if (_blackboard.Get(VitalEnergy) == 0) {
                _blackboard.Set(IsAlive, false);
                _blackboard.Set(CanMove, false);
                _cellLayer.UpdateAgentDrawStatus(AgentID, CellLayerImpl.BehaviourType.Dead);
            }
        }

        /// <summary>
        ///     Calculate pressure depending variables.
        /// </summary>
        /// <param name="cellData"></param>
        private void RefreshPressureDependingValues(TCell cellData) {

            // Check if the pressure is higher than the human can handle with
            if (cellData.PressureOnCell > _blackboard.Get(ResistanceToPressure)) {
                int vitalEnergy = _blackboard.Get(VitalEnergy);
                if (vitalEnergy < 5) {
                    _blackboard.Set(VitalEnergy, 0);
                }
                else {
                    _blackboard.Set(VitalEnergy, vitalEnergy - 5);
                }
            }
        }
    }

}