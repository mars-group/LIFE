using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class CToD : AbstractGoapAction {
        public CToD()
            : base(
                new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.C, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.D, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}