﻿using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class ToF : AbstractGoapAction {
        public ToF()
            : base(
                new List<WorldstateSymbol>(),
                new List<WorldstateSymbol> { new WorldstateSymbol(WorldProperties.F, true, typeof (Boolean)),
                }) {}

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}