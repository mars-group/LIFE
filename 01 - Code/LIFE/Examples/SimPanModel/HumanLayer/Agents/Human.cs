﻿using System;
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

        public Human
            (CellLayerImpl cellLayer, CellLayerImpl.BehaviourType behaviourType = CellLayerImpl.BehaviourType.Reactive) {
            // startvalues
            _blackboard.Set(ResistanceToPressure, 10);
            _blackboard.Set(VitalEnergy, 20);
            _blackboard.Set(Strength, 5);
            _blackboard.Set(CanMove, true);
            _blackboard.Set(IsAlive, true);
            _blackboard.Set(BehaviourType, behaviourType);

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

        private void ChooseAction() {
            if (_blackboard.Get(IsOnExit)) {
                LeaveCellFieldByExit();
            }
            else {
                if (_blackboard.Get(HasCalmingSphere)) {
                    CreateOrUpdateCalmingSphere();
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

        public void SetCalmingSphere() {
            _blackboard.Set(HasCalmingSphere, true);
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
        ///     Delete the calming sphere of the human.
        /// </summary>
        public void DeleteCalmingSphere() {
            _cellLayer.DeleteHumanCalmingFromCells(_helperVariableCalmedCells, AgentID);
            _helperVariableCalmedCells = new List<int>();
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

            if (cellData.IsExit) {
                _blackboard.Set(IsOnExit, true);
            }
        }

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