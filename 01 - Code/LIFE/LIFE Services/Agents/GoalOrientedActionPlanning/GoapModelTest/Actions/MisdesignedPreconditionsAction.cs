﻿using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Implementation;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {

    public class MisdesignedPreconditionsAction : AbstractGoapAction {
        public MisdesignedPreconditionsAction()
            : base(
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.A, true, typeof (Boolean)),
                    new WorldstateSymbol(WorldProperties.A, false, typeof (Boolean)),
                },
                new List<WorldstateSymbol> {
                    new WorldstateSymbol(WorldProperties.B, true, typeof (Boolean)),
                }) {}

        public override void Execute() {
            throw new NotImplementedException();
        }
    }

}