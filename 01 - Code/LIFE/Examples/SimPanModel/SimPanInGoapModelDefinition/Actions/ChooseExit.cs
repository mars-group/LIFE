using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class ChooseExit : AbstractGoapAction {
        private readonly Human _human;

        public ChooseExit(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.KnowsExitLocation, true, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override void Execute() {
            _human.HumanBlackboard.Set(Human.Target, _human.ExitCoordinatesFromMemory);
            _human.HumanBlackboard.Set(Human.HasTarget, true);
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.HasTarget);
        }
    }

}