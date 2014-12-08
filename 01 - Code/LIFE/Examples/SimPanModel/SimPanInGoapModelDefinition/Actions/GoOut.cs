using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    public class GoOut : AbstractGoapAction {
        private readonly Human _human;
        
        public GoOut(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))},
                new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOutSide, true, typeof (Boolean))}) {
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
            _human.MotorAndNavigation.LeaveByExit();
        }
        
    }

}