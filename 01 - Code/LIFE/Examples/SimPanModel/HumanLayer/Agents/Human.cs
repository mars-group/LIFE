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
        public const int CalmingRadius = 2;
        public const int MassFlightRadius = 2;
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
        public readonly PerceptionAndMemory SensorAndMemory;
        public readonly MotorAndNavigation MotorAndNavigation;
        public readonly CellLayerImpl CellLayer;
        private readonly bool _helperIsGoap;
        private readonly AbstractGoapSystem _goapActionSystem;

        public bool IsAlive = true;
        public int VitalEnergy = 20;
        public bool CanMove = true;
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
            SensorAndMemory = new PerceptionAndMemory(cellLayer, this, HumanBlackboard);

            // create the moving and manipuliating system/ action trigger
            MotorAndNavigation = new MotorAndNavigation(cellLayer, this, HumanBlackboard);

            // place the human on the cell field by random
            MotorAndNavigation.GetAnSetRandomPositionInCellWorld();

            if (isGoap) {
                string nameOfConfigClass = null;
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

                _goapActionSystem =
                    GoapComponent.LoadGoapConfigurationWithSelfreference
                        (nameOfConfigClass, HumanLayerImpl.NamespaceOfModelDefinition, HumanBlackboard, this);

                FearValue = GetRandomisedFearValue(behaviourType);
            }
        }

        #region IAgent Members

        public void Tick() {
            if (_helperIsGoap && IsAlive && !HumanBlackboard.Get(IsOutSide)) {
                SensorAndMemory.SenseCellInformations();
                SensorAndMemory.ProcessSensorInformations();

                AbstractGoapAction action = _goapActionSystem.GetNextAction();

                MotorAndNavigation.ExecuteGoapAction();
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
            CanMove = false;

            if (HasCalmingSphere) {
                HasCalmingSphere = false;
                SensorAndMemory.DeleteCalmingSphere();
            }

            if (HasMassFlightSphere) {
                HasMassFlightSphere = false;
                SensorAndMemory.DeleteMassFlightSphere();
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