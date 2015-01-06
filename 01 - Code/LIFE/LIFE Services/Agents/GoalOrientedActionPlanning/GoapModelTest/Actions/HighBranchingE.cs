using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    internal class HighBranchingE : AbstractGoapAction {
        public HighBranchingE()
            : base(
                new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.E, true, typeof (Boolean)),
                }) {}

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}