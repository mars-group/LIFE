﻿using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapActionSystem.Implementation {
    /// <summary>
    /// null action class
    /// </summary>
    internal class SurrogateAction : AbstractGoapAction {
        public SurrogateAction()
            : base(new List<IGoapWorldProperty>(), new List<IGoapWorldProperty>()) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            return true;
        }

        public override void Execute() {
        }

        public override int GetExecutionCosts() {
            return 0;
        }

        public override int GetPriority() {
            return 0;
        }
    }
}