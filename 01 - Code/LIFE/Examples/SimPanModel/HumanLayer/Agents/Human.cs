using System;
using System.Collections.Generic;
using System.Drawing;
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
        public const int CalmingRadius = HumanLayerImpl.CalmingRadius;
        public const int MassFlightRadius = HumanLayerImpl.MassFlightRadius;
        public const int Strength = HumanLayerImpl.Strength;
        public const int ResistanceToPressure = HumanLayerImpl.ResistanceToPressure;

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
        public readonly PerceptionAndMemory SensorAndMemory;
        public readonly MotorAndNavigation MotorAndNavigation;
        private readonly CellLayerImpl _cellLayer;

        private AbstractGoapSystem _goapActionSystem;
        public bool IsAlive = true;

        public int VitalEnergy = 20;
        private bool _canMove = true;
        public CellLayerImpl.BehaviourType CurrentBehaviourType;

        public bool HasMassFlightSphere;
        public int PauseMassFlightSphere;
        public bool HasCalmingSphere;
        public bool IsOnMassFlight;
        public Point ExitCoordinatesFromMemory = CellFieldStartConfig.ExitPoint;
        public int FearValue;


        /// <summary>
        ///     Create the start value und the sensor and motor classes.
        /// </summary>
        /// <param name="cellLayer"></param>
        /// <param name="behaviourType"></param>
        public Human
            (CellLayerImpl cellLayer,
                CellLayerImpl.BehaviourType behaviourType) {
            CurrentBehaviourType = behaviourType;
            _cellLayer = cellLayer;

            // startvalues
            FearValue = GetRandomisedFearValue(behaviourType);
            HumanBlackboard.Set(LastPosition, new Point());
            IsOnMassFlight = false;
            HumanBlackboard.Set(IsOnExit, false);

            // create the sensor and memory management
            SensorAndMemory = new PerceptionAndMemory(cellLayer, this, HumanBlackboard);

            // create the moving and manipuliating system/ action trigger
            MotorAndNavigation = new MotorAndNavigation(cellLayer, this, HumanBlackboard);

            // place the human on the cell field by random
            MotorAndNavigation.GetAnSetRandomPositionInCellWorld();
            //MotorAndNavigation.GetPosition(391);
            SensorAndMemory.CollectAndProcessSensorInformation();

            string nameOfConfigClass;

            switch (behaviourType) {
                case CellLayerImpl.BehaviourType.Reactive:
                    nameOfConfigClass = HumanLayerImpl.ReactiveConfig;
                    break;
                case CellLayerImpl.BehaviourType.Deliberative:
                    nameOfConfigClass = HumanLayerImpl.DeliberativeConfig;
                    HumanBlackboard.Set(KnowsExitLocation, true);
                    break;
                case CellLayerImpl.BehaviourType.Reflective:
                    nameOfConfigClass = HumanLayerImpl.ReflectiveConfig;
                    HumanBlackboard.Set(KnowsExitLocation, true);
                    HasCalmingSphere = true;
                    break;
                default:
                    throw new ArgumentException("Human: BehaviourType of agent is not known.");
            }

            object[] configClassParameter = { this, HumanBlackboard };
            _goapActionSystem =
                GoapComponent.LoadGoapConfigurationWithSelfReference
                    (nameOfConfigClass, HumanLayerImpl.NamespaceOfModelDefinition, HumanBlackboard, configClassParameter);

            FearValue = GetRandomisedFearValue(behaviourType);
        }

        #region IAgent Members

        public void Tick() {
            if (IsAlive && HumanBlackboard.Get(IsOutSide) == false) {
                SensorAndMemory.CollectAndProcessSensorInformation();

                if (IsAlive && _canMove){
                    _goapActionSystem.GetNextAction();
                    MotorAndNavigation.ExecuteGoapAction();
                }
                // update is only needed if the human has not left the simalution area
                if (HumanBlackboard.Get(IsOutSide) == false) {
                    UpdateBehaviourType();
                }
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
                return rand.Next
                    (HumanLayerImpl.UpperBoundOfDeliberativeBehaviourArea + 1, HumanLayerImpl.HighestBoundOfFear + 1);
            }
            if (behaviour == CellLayerImpl.BehaviourType.Deliberative) {
                return rand.Next
                    (HumanLayerImpl.UpperBoundOfReflectiveBehaviourArea + 1,
                        HumanLayerImpl.UpperBoundOfDeliberativeBehaviourArea + 1);
            }
            if (behaviour == CellLayerImpl.BehaviourType.Reflective) {
                return rand.Next
                    (HumanLayerImpl.LowestBoundOfFear, HumanLayerImpl.UpperBoundOfReflectiveBehaviourArea + 1);
            }
            throw new ArgumentException("Human: No matching in behaviour types at GetRandomisedFearValue");
        }

        /// <summary>
        ///     Check if depending on the fear value the agent got the right behaviour type. If the behaviour 
        ///     must be switched, updtae needed variables so goap can choose the correct actions.
        /// </summary>
        private void UpdateBehaviourType() {
            // Range for akinesia. Fear is between highest and above upper reactive bound.
            if (HumanLayerImpl.HighestBoundOfFear >= FearValue
                && FearValue > HumanLayerImpl.UpperBoundOfReactiveBehaviourArea) {
                if (_canMove == false) {
                    _canMove = false;
                }
                // change to reactive behaviour
                if (CurrentBehaviourType != CellLayerImpl.BehaviourType.Reactive) {
                    // Reset controlling values and create the matiching goap system.
                    IsOnMassFlight = false;
                    HumanBlackboard.Set(KnowsExitLocation, false);
                    HumanBlackboard.Set(Target, new Point());
                    HumanBlackboard.Set(HasTarget, false);
                    SensorAndMemory.DeleteCalmingSphere();
                    SensorAndMemory.DeleteMassFlightSphere();
                    MotorAndNavigation.DeletePath();

                    CurrentBehaviourType = CellLayerImpl.BehaviourType.Reactive;
                    SensorAndMemory.CollectAndProcessSensorInformation();
                    HumanLayerImpl.Log.Info("Changed behaviour type to akinesia");
                    
                    object[] configClassParameter = { this, HumanBlackboard };
                    _goapActionSystem = GoapComponent.LoadGoapConfigurationWithSelfReference
                        (HumanLayerImpl.ReactiveConfig,
                            HumanLayerImpl.NamespaceOfModelDefinition,
                            HumanBlackboard,
                            configClassParameter);

                    _cellLayer.UpdateAgentDrawStatus(AgentID, CurrentBehaviourType);
                }

                // Range for reactive behaviour.
            }
            else if (HumanLayerImpl.UpperBoundOfDeliberativeBehaviourArea >= FearValue &&
                     FearValue > HumanLayerImpl.UpperBoundOfDeliberativeBehaviourArea) {
                // change to reactive behaviour
                if (CurrentBehaviourType != CellLayerImpl.BehaviourType.Reactive) {
                    // Reset controlling values and create the matiching goap system.
                    IsOnMassFlight = false;
                    HumanBlackboard.Set(KnowsExitLocation, false);
                    HumanBlackboard.Set(Target, new Point());
                    HumanBlackboard.Set(HasTarget, false);
                    SensorAndMemory.DeleteCalmingSphere();
                    SensorAndMemory.DeleteMassFlightSphere();
                    MotorAndNavigation.DeletePath();

                    CurrentBehaviourType = CellLayerImpl.BehaviourType.Reactive;
                    SensorAndMemory.CollectAndProcessSensorInformation();
                    HumanLayerImpl.Log.Info("Changed behaviour type to reactive");
                    
                    object[] configClassParameter = {this, HumanBlackboard};
                    _goapActionSystem = GoapComponent.LoadGoapConfigurationWithSelfReference
                        (HumanLayerImpl.ReactiveConfig,
                            HumanLayerImpl.NamespaceOfModelDefinition,
                            HumanBlackboard,
                            configClassParameter
                );

                    _cellLayer.UpdateAgentDrawStatus(AgentID, CurrentBehaviourType);
                }

                // Range for deliberative behaviour.
            }
            else if (HumanLayerImpl.UpperBoundOfDeliberativeBehaviourArea >= FearValue &&
                     FearValue > HumanLayerImpl.UpperBoundOfReflectiveBehaviourArea) {
                // change to deliberative behaviour
                if (CurrentBehaviourType != CellLayerImpl.BehaviourType.Deliberative) {
                    // Reset controlling values and create the matiching goap system.
                    IsOnMassFlight = false;
                    HumanBlackboard.Set(KnowsExitLocation, true);
                    HumanBlackboard.Set(Target, new Point());
                    HumanBlackboard.Set(HasTarget, false);
                    SensorAndMemory.DeleteCalmingSphere();
                    MotorAndNavigation.DeletePath();

                    CurrentBehaviourType = CellLayerImpl.BehaviourType.Deliberative;
                    SensorAndMemory.CollectAndProcessSensorInformation();
                    HumanLayerImpl.Log.Info("Changed behaviour type to deliberative");

                    object[] configClassParameter = { this, HumanBlackboard };
                    _goapActionSystem = GoapComponent.LoadGoapConfigurationWithSelfReference
                        (HumanLayerImpl.DeliberativeConfig,
                            HumanLayerImpl.NamespaceOfModelDefinition,
                            HumanBlackboard,
                            configClassParameter);

                    _cellLayer.UpdateAgentDrawStatus(AgentID, CurrentBehaviourType);
                }


                // Range for reflective behaviour.
            }
            else if (HumanLayerImpl.UpperBoundOfReflectiveBehaviourArea >= FearValue
                     && FearValue <= HumanLayerImpl.LowestBoundOfFear) {
                // change to reflective behaviour
                if (CurrentBehaviourType != CellLayerImpl.BehaviourType.Reflective) {
                    // Reset controlling values and create the matiching goap system.
                    IsOnMassFlight = false;
                    HumanBlackboard.Set(KnowsExitLocation, true);
                    HumanBlackboard.Set(Target, new Point());
                    HumanBlackboard.Set(HasTarget, false);
                    HasCalmingSphere = true;
                    MotorAndNavigation.DeletePath();

                    CurrentBehaviourType = CellLayerImpl.BehaviourType.Reflective;
                    SensorAndMemory.CollectAndProcessSensorInformation();
                    HumanLayerImpl.Log.Info("Changed behaviour type to reflective");

                    object[] configClassParameter = { this, HumanBlackboard };
                    _goapActionSystem = GoapComponent.LoadGoapConfigurationWithSelfReference
                        (HumanLayerImpl.ReflectiveConfig,
                            HumanLayerImpl.NamespaceOfModelDefinition,
                            HumanBlackboard,
                            configClassParameter);

                    _cellLayer.UpdateAgentDrawStatus(AgentID, CurrentBehaviourType);
                }
            }
        }

        /// <summary>
        ///     Check if the path variable is not null and not empty.
        /// </summary>
        /// <returns></returns>
        public bool HasValidPath() {
            if (HumanBlackboard.Get(HasPath)) {
                if (HumanBlackboard.Get(Path) != null && HumanBlackboard.Get(Path).Count != 0) {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Is used if a human is on a exit cell and need to leave the field.
        /// </summary>
        public void LeaveByExit() {
            Point leavingPosition = HumanBlackboard.Get(Position);
            SensorAndMemory.DeleteCalmingSphere();
            SensorAndMemory.DeleteMassFlightSphere();
            HumanBlackboard.Set(IsOutSide, true);
            _canMove = false;
            MotorAndNavigation.DeleteHumanInWorld();
            HumanLayerImpl.Log.Info("Agent " + AgentID + " has left by exit on " + leavingPosition);
        }

        /// <summary>
        ///     Used if a panik event kills a human. do not change cell data
        ///     because the event agent changes a whoe set of cells after killing agents.
        /// </summary>
        public void GetKilledByPanicArea() {
            SetAsKilled(createObstacle: false);
        }

        /// <summary>
        ///     Set the human attributes to not alive and not moving. Depending on the death simtuation
        ///     decide if the cell should be updated to obstacle.
        /// </summary>
        /// <param name="createObstacle"></param>
        public void SetAsKilled(bool createObstacle = true) {
            IsAlive = false;
            _canMove = false;

            SensorAndMemory.DeleteCalmingSphere();
            SensorAndMemory.DeleteMassFlightSphere();
            
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