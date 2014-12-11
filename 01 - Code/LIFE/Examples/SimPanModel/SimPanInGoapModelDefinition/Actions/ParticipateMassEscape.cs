using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class ParticipateMassEscape : AbstractGoapAction {
        private readonly Human _human;

        public ParticipateMassEscape(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, true, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsInExitArea, true, typeof (Boolean))}) {
            _human = human;
        }

        public override bool ValidateContextPreconditions() {
            return _human.IsOnMassFlight;
        }

        public override void Execute() {
            if (_human.HumanBlackboard.Get(Human.Target).IsEmpty) {
                bool b = IsExecutable(_human.HumanBlackboard.Get(AbstractGoapSystem.Worldstate));
                bool c = ValidateContextPreconditions();
                int a = 1;
            }
            _human.MotorAndNavigation.ApproximateToTarget();
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsInExitArea);
        }
    }

}