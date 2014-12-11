using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class ParticipateMassEscape : AbstractGoapAction {
        private readonly Human _human;

        public ParticipateMassEscape
            (Human human) :
                base(
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, true, typeof (bool))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (bool))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            return _human.IsOnMassFlight;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override void Execute() {
            _human.MotorAndNavigation.ApproximateToTarget();
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsInExitArea);
        }
    }

}