using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class HToI : AbstractGoapAction {
        public HToI()
            : base(
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.H, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.I, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}