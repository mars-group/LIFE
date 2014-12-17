using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {

    public class ActionPlayWithToy : AbstractGoapAction {
        public ActionPlayWithToy()
            : base(new List<WorldstateSymbol> {new WorldstateSymbol(WorldProperties.HasToy, true, typeof (Boolean))},
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasToy, false, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}