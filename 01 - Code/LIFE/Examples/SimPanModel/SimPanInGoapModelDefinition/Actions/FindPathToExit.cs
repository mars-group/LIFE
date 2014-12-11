using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class FindPathToExit : AbstractGoapAction {
        private readonly Human _human;
        private bool _successOnPlanning = true;

        public FindPathToExit(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, true, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasPath, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            return _successOnPlanning;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override int GetExecutionCosts() {
            return 1;
        }

        public override void Execute() {
            _successOnPlanning = _human.MotorAndNavigation.PlanRoute(_human.HumanBlackboard.Get(Human.Target));
        }

        public override bool IsFinished() {
            return _successOnPlanning && _human.HasValidPath();
        }

        public override AbstractGoapAction GetResetCopy() {
            return new FindPathToExit(_human);
        }
    }

}