using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class DToE : AbstractGoapAction {
        public DToE()
            : base(
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.D, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.E, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}