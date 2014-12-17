using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {
    public class ActionClean : AbstractGoapAction
    {
        public ActionClean()
            : base(new List<WorldstateSymbol> { new WorldstateSymbol(WorldProperties.Happy, true, typeof(Boolean)) },
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, false, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean)),
                }) { }


        public override void Execute() {
            throw new NotImplementedException();
        }
    }
}