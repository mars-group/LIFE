using System;
using System.Drawing;
using CellLayer;
using GoapCommon.Abstract;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace HumanLayer.Agents {
    /// <summary>
    /// Represens one single human in the simulation. A human is shown as a point ini the simulation view.
    /// </summary>
    public class Human : IAgent {
        public readonly Guid AgentID = Guid.NewGuid();
        public readonly Blackboard Blackboard = new Blackboard();
        private CellLayerImpl _cellLayer;
        private readonly PerceptionAndMemory _sensor;
        private readonly MotorAndNavigation _motor;
        public Point Position;
        public int CellId;
        public bool CanMove = true;
        public bool IsAlive = true;

       
        /// <summary>
        ///     blackboard property
        ///     save the current action. actions can use more than one tick, so they needed to be saved.
        /// </summary>
        public static readonly BlackboardProperty<Point> Target =
            new BlackboardProperty<Point>("Target");

        public Human(CellLayerImpl cellLayer) {
            _cellLayer = cellLayer;

            _sensor = new PerceptionAndMemory(_cellLayer, this);
            _motor = new MotorAndNavigation(_cellLayer, this);

            _motor.GetRandomPositionInCellWorld();

            //AbstractGoapSystem goapActionSystem = GoapActionSystem.Implementation.GoapComponent.LoadGoapConfiguration
            //  ("eine klasse", "ein namespace", blackboard);

        }

        #region IAgent Members

        public void Tick() {
            if (IsAlive) {
                _sensor.SenseCell();

                Blackboard.Set(Target,new Point(10,10));

                WalkRandom();
                GoToTarget();
                HumanLayerImpl.Log.Info("i " + AgentID + " changed to : (" + Position.X + "," + Position.Y + ")");
                
            }
        }

        private void GoToTarget() {
            if (CanMove) {
                _motor.ApproximateToTarget();
            }
        }

        #endregion

        /// <summary>
        ///  Choose a random direction with a cell with no agent on. Go one step.
        /// </summary>
        private void WalkRandom() {
            if (CanMove) {
                _motor.WalkRandom();
            }
        }

        /// <summary>
        /// Used if a panik event kills a human.
        /// </summary>
        public void GetKilled() {
            _sensor.SetAsKilled();
            HumanLayerImpl.Log.Info("i am " + AgentID + " dead.");
        }

        

        public void SuccessiveApproximation() {
            
        }

        
    }

}