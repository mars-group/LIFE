using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     This action is udes if a human is standing on an exit cell to get out of the simulation.
    /// </summary>
    public class GoOut : AbstractGoapAction {
        private readonly Human _human;

        public GoOut(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOnExit, true, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.IsOutSide, true, typeof (Boolean))}) {
            _human = human;
        }

        public override void Execute() {
            _human.LeaveByExit();
        }

        public override bool IsFinished() {
            return _human.HumanBlackboard.Get(Human.IsOutSide);
        }
    }

}