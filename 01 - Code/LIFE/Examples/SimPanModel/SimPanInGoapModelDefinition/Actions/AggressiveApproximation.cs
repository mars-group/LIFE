using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     Is used for humans in reactive behaviour mode. If the human fails in moving he will add pressure to
    ///     the cell in the chosen direction.
    /// </summary>
    public class AggressiveApproximation : AbstractGoapAction {
        private const int MaximumAttemps = 30;
        private readonly Human _human;
        private int _previousAttemps;

        public AggressiveApproximation
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))}) {
            _human = human;
            _previousAttemps = 0;
        }

        public override bool ValidateContextPreconditions() {
            if (_previousAttemps <= MaximumAttemps) {
                return true;
            }
            return false;
        }

        public override void Execute() {
            _previousAttemps += 1;
            if (_human.HumanBlackboard.Get(Human.MovementFailed)) {
                _human.MotorAndNavigation.ApproximateToTarget(aggressiveMode: true);
            }
            else {
                _human.MotorAndNavigation.ApproximateToTarget();
            }
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOnExit);
        }

        public override AbstractGoapAction GetResetCopy() {
            return new AggressiveApproximation(_human);
        }
    }

}