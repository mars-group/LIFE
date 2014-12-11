using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

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

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsInExitArea);
        }

        public override void Execute() {
            _human.MotorAndNavigation.FollowDirectionOrSwitchDirection();
        }
    }

}