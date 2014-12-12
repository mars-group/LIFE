using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     Follow the path across the cell field into the exit area.
    /// </summary>
    public class FollowPathToExitArea : AbstractGoapAction {
        private const int MaximumAttemps = 20;
        private const int MaximumFailedMovementsPerStep = 5;
        private readonly Human _human;
        private int _previousAttemps;
        private int _previousFailedMovements;

        public FollowPathToExitArea
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasPath, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            if (_previousAttemps <= MaximumAttemps) {
                if (_previousFailedMovements <= MaximumFailedMovementsPerStep) {
                    return true;
                }
            }
            return false;
        }

        public override void Execute() {
            _previousAttemps += 1;

            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _human.MotorAndNavigation.TryWalkNextDirectionOfPlan(aggressiveMode: true);
                _previousFailedMovements += 1;
            }
            else {
                _human.MotorAndNavigation.TryWalkNextDirectionOfPlan();
                _previousFailedMovements = 0;
            }
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsInExitArea);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new FollowPathToExitArea(_human);
        }
    }

}