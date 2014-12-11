
using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     If a human in reaktive mode has no target ther is no more option then running around and maybe
    ///     find the exit area.
    /// </summary>
    public class AimlessWandering : AbstractGoapAction {
        private readonly Human _human;

        public AimlessWandering(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, false, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            return !_human.IsOnMassFlight;
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsInExitArea);
        }

        public override void Execute() {
            _human.MotorAndNavigation.FollowDirectionOrSwitchDirection();
        }
    }

}