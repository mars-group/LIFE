using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {

    public class ActionStealToy : AbstractGoapAction {
        public ActionStealToy()
            : base(new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.HasToy, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}