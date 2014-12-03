using System;
using System.Drawing;
using CellLayer;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {

    /// <summary>
    ///     Represens one single human in the simulation. A human is shown as a point ini the simulation view.
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

        public readonly Guid AgentID = Guid.NewGuid();
        public readonly Blackboard HumanBlackboard = new Blackboard();
        private readonly PerceptionAndMemory _sensor;
        private readonly MotorAndNavigation _motorAndNavigation;
        public Point Position;
        public int CellId;
        public bool CanMove = true;
        public bool IsAlive = true;
        public int ResistanceToPressure = 10;
        public int VitalEnergy = 10;
        public int Strength = 5;


        
        public Human(CellLayerImpl cellLayer) {

            _sensor = new PerceptionAndMemory(cellLayer, this);
            _motorAndNavigation = new MotorAndNavigation(cellLayer, this);

            _motorAndNavigation.GetRandomPositionInCellWorld();

            //AbstractGoapSystem goapActionSystem = GoapActionSystem.Implementation.GoapComponent.LoadGoapConfiguration
            //  ("eine klasse", "ein namespace", blackboard);
        }

        #region IAgent Members

        public void Tick() {
           
            if (IsAlive) {
                _sensor.SenseAndProcessInformations();

                if (HumanBlackboard.Get(MovementFailed)) {
                    
                }

                if (HumanBlackboard.Get(HasTarget))
                {
                    SuccessiveApproximation();
                } else {
                    WalkRandom();
                }
            }
        }

        #endregion

        

        public void SetTarget(Point targetPosition) {
            HumanBlackboard.Set(Target, targetPosition);
        }

        public void CreatePressure() {
            
        }


        /// <summary>
        ///     Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (CanMove) {
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
            if (CanMove){
                _motorAndNavigation.ApproximateToTarget(aggressiveMode: true);
            }
        }
    }

}