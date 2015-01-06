using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {

    public class ActionBuyToy : AbstractGoapAction {
        public ActionBuyToy()
            : base(new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.HasMoney, false, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasToy, true, typeof (Boolean)),
                }) {}

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}