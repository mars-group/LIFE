using System;
using System.Collections.Generic;
using System.Linq;
using GoapCommon.Interfaces;

namespace GoapCommon.Abstract {
    /// <summary>
    ///     nochmal überlegen welche Methoden auf den allgemeinen Objekten ausgeführt werden müssen
    ///     diese können allerdings auch per interface angeboten werden
    ///     für die abstrakte klasse ist allerdings interessant, welche methoden immer gleich sind für alle unterklassen und
    ///     wie den programmierern der actions somit arbeit abgenommen werden kann
    ///     auch das graphensystem sollte nur die methoden der abtracten benutzen
    /// </summary>
    public abstract class AbstractGoapAction : IGoapAction, IEquatable<AbstractGoapAction> {
        /// <summary>
        ///     partial state of the world must be fulfilled for execution
        /// </summary>
        private readonly List<IGoapWorldstate> _preConditions;

        /// <summary>
        ///     partial changes of the world state by execution
        /// </summary>
        private readonly List<IGoapWorldstate> _effects;

        /// <summary>
        ///     get the immutable list of preconditions
        /// </summary>
        public List<IGoapWorldstate> PreConditions {
            get { return _preConditions; }
        }

        /// <summary>
        ///     get the immutable list of effects
        /// </summary>
        private List<IGoapWorldstate> Effects {
            get { return _effects; }
        }

        public ISet<Type> GetAffectingWorldstateTypes() {
            var types = new HashSet<Type>();

            foreach (var goapWorldstate in _effects) {
                types.Add(goapWorldstate.GetType());
            }
            foreach (var goapWorldstate in _preConditions) {
                types.Add(goapWorldstate.GetType());
            }
            return types;
        }

        protected AbstractGoapAction(List<IGoapWorldstate> preconditionWorldstates,
            List<IGoapWorldstate> effectWorldstates) {
            _preConditions = preconditionWorldstates;
            _effects = effectWorldstates;
        }

        /// <summary>
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public bool IsExecutable(List<IGoapWorldstate> sourceWorldState) {
            return IsSubset(_preConditions, sourceWorldState);
        }

        /// <summary>
        ///     check if a state could be the resulting state after executing this action
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        public bool IsEffectCorrespondingToState(List<IGoapWorldstate> state) {
            return IsSubset(_effects, state);
        }

        public bool IsSatisfyingStateByEffects(List<IGoapWorldstate> state) {
            return IsSubset(state, _effects);
        }

        private bool IsSubset(List<IGoapWorldstate> potentiallySubSet, List<IGoapWorldstate> enclosingSet) {
            return (potentiallySubSet.Where(x => enclosingSet.Contains(x)).Count() ==
                    potentiallySubSet.Count());
        }

        /// <summary>
        ///     create a new list of cloned worldstates based on the sourceWorldstate
        ///     without the elements with same type in effect list and add effect list
        /// </summary>
        /// <param name="sourceWorldState"></param>
        /// <returns></returns>
        public List<IGoapWorldstate> GetResultingWorldstate(List<IGoapWorldstate> sourceWorldState) {
            List<IGoapWorldstate> resultingWorldStates =
                (from worldstate in _effects select worldstate.GetClone()).ToList();

            List<Type> typesInEffectList =
                (from worldstate in _effects select worldstate.GetType()).ToList();

            resultingWorldStates.AddRange(from worldState in sourceWorldState
                where !typesInEffectList.Contains(worldState.GetType())
                select worldState.GetClone());

            return resultingWorldStates;
        }

        public List<IGoapWorldstate> GetSourceWorldstate(List<IGoapWorldstate> targetWorldstate) {
            // get all preconditions at first
            List<IGoapWorldstate> resultingWorldStates =
                (from worldstate in _preConditions select worldstate.GetClone()).ToList();

            List<Type> typesInPreconditionList =
                (from worldstate in _preConditions select worldstate.GetType()).ToList();


            resultingWorldStates.AddRange(from worldState in targetWorldstate
                where !typesInPreconditionList.Contains(worldState.GetType())
                select worldState.GetClone());

            return resultingWorldStates;
        }

        public override string ToString() {
            return string.Format("PreConditions: {0}, Effects: {1}", PreConditions, Effects);
        }

        public bool Equals(AbstractGoapAction other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return (GetType() == other.GetType()) && (_preConditions.All(i => other._preConditions.Contains(i)) &&
                                                      (other._preConditions.All(i => _preConditions.Contains(i))));
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
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


        public abstract bool ValidateContextPreconditions();

        public abstract bool ExecuteContextEffects();

        public abstract void Execute();

        public abstract int GetExecutionCosts();

        public abstract int GetPriority();
    }
}