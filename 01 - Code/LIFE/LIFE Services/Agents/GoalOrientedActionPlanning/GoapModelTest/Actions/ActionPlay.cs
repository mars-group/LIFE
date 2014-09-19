using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;
using GoapModelTest.Worldstates;

namespace GoapModelTest.Actions {
    public class ActionPlay : AbstractGoapAction {
        /// <summary>
        ///     the world state must be in this needed state before the action can be executet
        /// </summary>
        private readonly List<IGoapWorldstate> _preConditions = new List<IGoapWorldstate> {
            new HasToy(true)
        };

        /// <summary>
        ///     after execution the world changes partially
        /// </summary>
        private List<IGoapWorldstate> _effect = new List<IGoapWorldstate> {
            new Happy(true),
            new HasToy(false)
        };

        public ActionPlay(List<IGoapWorldstate> preconditionWorldstates, List<IGoapWorldstate> effectWorldstates) :
            base(preconditionWorldstates, effectWorldstates) {}

        public ActionPlay()
            : base(new List<IGoapWorldstate> {new HasToy(true)},
                new List<IGoapWorldstate> {
                    new Happy(true),
                    new HasToy(false)
                }) {}

        public override bool ValidateContextPreconditions() {
            return true;
        }

        public override bool ExecuteContextEffects() {
            throw new NotImplementedException();
        }

        public override void Execute() {
            throw new NotImplementedException();
        }

        public override int GetExecutionCosts() {
            throw new NotImplementedException();
        }

        public override int GetPriority() {
            throw new NotImplementedException();
        }
    }
}