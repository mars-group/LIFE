﻿using System;
using System.Collections.Generic;
using GoapActionSystem.Implementation;
using GoapCommon.Interfaces;
using GoapUser.Worldstates;

namespace GoapUser.Actions {
    internal class ActionPlay : AbstractGoapAction {
        /// <summary>
        ///     the world state must be in this needed state before the action can be executet
        /// </summary>
        private readonly List<IGoapWorldstate> _preConditions = new List<IGoapWorldstate> {
            new HasToy(true, WorldStateEnums.HasToy)
        };

        /// <summary>
        ///     after execution the world changes partially
        /// </summary>
        private List<IGoapWorldstate> _effect = new List<IGoapWorldstate> {
            new Happy(true, WorldStateEnums.Happy),
            new HasToy(false, WorldStateEnums.HasToy)
        };

        public ActionPlay(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates) :
            base(preconditionWorldstates, effectWorldstates) {}

        public ActionPlay()
            : base(new List<IGoapWorldstate> {new HasToy(true, WorldStateEnums.HasToy)},
                new List<IGoapWorldstate> {
                    new Happy(true, WorldStateEnums.Happy),
                    new HasToy(false, WorldStateEnums.HasToy)
                }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override bool Execute() {
            throw new NotImplementedException();
        }
    }
}