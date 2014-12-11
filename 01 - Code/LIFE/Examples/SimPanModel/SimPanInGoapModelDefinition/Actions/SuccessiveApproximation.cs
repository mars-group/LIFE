using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     Search and walk an approximating direction.
    /// </summary>
    public class SuccessiveApproximation : AbstractGoapAction {
        private const int MaximumFailures = 3;
        private const int MaximumAttemps = 30;
        private readonly Human _human;
        private int _previousFailures;
        private int _previousAttemps;

        public SuccessiveApproximation
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))}) {
            _human = human;
            _previousFailures = 0;
        }

        public override bool ValidateContextPreconditions() {
            if (_previousFailures <= MaximumFailures && _previousAttemps <= MaximumAttemps) {
                return true;
            }
            return false;
        }

        public override void Execute() {
            _previousAttemps += 1;
            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _previousFailures += 1;
            }
            _human.MotorAndNavigation.ApproximateToTarget();
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOnExit);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new SuccessiveApproximation(_human);
        }
    }

}