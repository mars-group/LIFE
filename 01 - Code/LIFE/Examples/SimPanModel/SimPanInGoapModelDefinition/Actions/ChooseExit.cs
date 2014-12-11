using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using HumanLayer.Agents;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition.Actions {

    /// <summary>
    ///     Reflective and deliberative humans can choose an exit. At least they can access the memory information
    ///     about the coordinates from the entrance.
    /// </summary>
    public class ChooseExit : AbstractGoapAction {
        private readonly Human _human;

        public ChooseExit(Human human) :
            base(
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.KnowsExitLocation, true, typeof (Boolean))},
            new List<WorldstateSymbol> {new WorldstateSymbol(Properties.HasTarget, true, typeof (Boolean))}) {
            _human = human;
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