using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class EToF : AbstractGoapAction {
        public EToF()
            : base(
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.E, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.F, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}