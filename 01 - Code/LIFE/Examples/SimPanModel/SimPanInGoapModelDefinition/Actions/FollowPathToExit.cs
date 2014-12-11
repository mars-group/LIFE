using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class FollowPathToExit : AbstractGoapAction {
        private const int MaximumAttemps = 50;
        private const int MaximumFailedMovementsPerStep = 2;
        private readonly Human _human;
        private int _previousAttemps;
        private int _previousFailedMovements;

        public FollowPathToExit
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasPath, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))}) {
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

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override void Execute() {
            _previousAttemps += 1;
            _human.MotorAndNavigation.TryWalkNextDirectionOfPlan();

            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _previousFailedMovements += 1;
            }
            else {
                _previousFailedMovements = 0;
            }
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOnExit);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new FollowPathToExit(_human);
        }
    }

}