using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions {

    public class ActionThinkPositiv : AbstractGoapAction {
        public ActionThinkPositiv()
            : base(new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean)),
                }) {}


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}