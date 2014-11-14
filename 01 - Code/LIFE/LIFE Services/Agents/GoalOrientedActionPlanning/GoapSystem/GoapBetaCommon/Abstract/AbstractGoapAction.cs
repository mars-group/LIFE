﻿using System;
using System.Collections.Generic;
using System.Linq;
using GoapBetaCommon.Implementation;
using GoapBetaCommon.Interfaces;

namespace GoapBetaCommon.Abstract {

    /// <summary>
    ///     nochmal überlegen welche Methoden auf den allgemeinen Objekten ausgeführt werden müssen
    ///     diese können allerdings auch per interface angeboten werden
    ///     für die abstrakte klasse ist allerdings interessant, welche methoden immer gleich sind für alle unterklassen und
    ///     wie den programmierern der actions somit arbeit abgenommen werden kann
    ///     auch das graphensystem sollte nur die methoden der abtracten benutzen
    /// </summary>
    public abstract class AbstractGoapAction : IGoapAction, IEquatable<AbstractGoapAction> {
        /// <summary>
        ///     get the immutable list of preconditions
        /// </summary>
        public List<WorldstateSymbol> PreConditions { get { return _preConditions; } }

        /// <summary>
        ///     get the immutable list of effects
        /// </summary>
        public List<WorldstateSymbol> Effects { get { return _effects; } }

        /// <summary>
        ///     partial state of the world must be fulfilled for execution
        /// </summary>
        private readonly List<WorldstateSymbol> _preConditions;

        /// <summary>
        ///     partial changes of the world state by execution
        /// </summary>
        private readonly List<WorldstateSymbol> _effects;

        protected AbstractGoapAction
            (List<WorldstateSymbol> preconditionWorldstates,
                List<WorldstateSymbol> effectWorldstates) {
            _preConditions = preconditionWorldstates;
            _effects = effectWorldstates;
        }

        #region IEquatable<AbstractGoapAction> Members

        public bool Equals(AbstractGoapAction other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return (GetType() == other.GetType()) && (_preConditions.All(i => other._preConditions.Contains(i)) &&
                                                      (other._preConditions.All(i => _preConditions.Contains(i))));
        }

        #endregion

        #region IGoapAction Members

        /// <summary>
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public bool IsExecutable(List<WorldstateSymbol> sourceWorldState) {
            return IsSubset(_preConditions, sourceWorldState);
        }

        /// <summary>
        ///     create a new list of cloned worldstates based on the sourceWorldstate
        ///     without the elements with same type in effect list and add effect list
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public List<WorldstateSymbol> GetResultingWorldstate(List<WorldstateSymbol> sourceWorldState) {
            List<WorldstateSymbol> resultingWorldStates =
                (from worldstate in _effects select worldstate).ToList();

            List<Type> typesInEffectList =
                (from worldstate in _effects select worldstate.GetType()).ToList();

            resultingWorldStates.AddRange
                (from worldState in sourceWorldState
                    where !typesInEffectList.Contains(worldState.GetType())
                    select worldState);

            return resultingWorldStates;
        }

        public abstract bool ValidateContextPreconditions();

        public abstract bool ExecuteContextEffects();

        public abstract void Execute();

        public abstract int GetExecutionCosts();

        public abstract int GetPriority();

        #endregion

        public ISet<Type> GetAffectingWorldstateTypes() {
            HashSet<Type> types = new HashSet<Type>();

            foreach (WorldstateSymbol goapWorldstate in _effects) {
                types.Add(goapWorldstate.GetType());
            }
            foreach (WorldstateSymbol goapWorldstate in _preConditions) {
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        /// <summary>
        ///     check if a state could be the resulting state after executing this action
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsEffectCorrespondingToState(List<WorldstateSymbol> state) {
            return IsSubset(_effects, state);
        }

        public bool IsSatisfyingStateByEffects(List<WorldstateSymbol> state) {
            return IsSubset(state, _effects);
        }

        private bool IsSubset(List<WorldstateSymbol> potentiallySubSet, List<WorldstateSymbol> enclosingSet) {
            return (potentiallySubSet.Where(x => enclosingSet.Contains(x)).Count() ==
                    potentiallySubSet.Count());
        }

        public override string ToString() {
            return string.Format("PreConditions: {0}, Effects: {1}", PreConditions, Effects);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            if (obj.GetType() != GetType()) {
                return false;
            }
            return Equals((AbstractGoapAction) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_preConditions.GetHashCode()*397) ^ _effects.GetHashCode();
            }
        }

        public static bool operator ==(AbstractGoapAction left, AbstractGoapAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(AbstractGoapAction left, AbstractGoapAction right) {
            return !Equals(left, right);
        }
    }

}