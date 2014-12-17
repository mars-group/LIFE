using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {

    public class ActionBuyFood : AbstractGoapAction {
        public ActionBuyFood()
            : base(new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasMoney, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.HasMoney, false, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasFood, true, typeof (Boolean)),
                }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}