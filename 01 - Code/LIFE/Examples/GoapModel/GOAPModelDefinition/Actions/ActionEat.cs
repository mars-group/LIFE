using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GOAPModelDefinition.Worldstates;

namespace GOAPModelDefinition.Actions
{

    public class ActionEat : AbstractGoapAction
    {
        public ActionEat()
            : base(new List<WorldstateSymbol> { new WorldstateSymbol(WorldProperties.HasFood, true, typeof(Boolean)) },
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.Happy, true, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.HasFood, false, typeof (Boolean)),
                }) { }


        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}