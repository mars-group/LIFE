﻿using System;
using System.Collections.Generic;
using System.Linq;
using Common.Interfaces;
using GoapComponent.GoapKnowledgeProcessingComponent;

namespace GoapComponent {

    /// <summary>
    ///     goap actions define a transition to another worldstate
    /// </summary>
    abstract class GoapAction : IEquatable<GoapAction>, IInteraction {
        protected readonly List<GoapWorldState> Preconditions = new List<GoapWorldState>();
        protected readonly List<GoapWorldState> Postconditions = new List<GoapWorldState>();

        /// <summary>
        ///     check if preconditions of action are subset of current world
        /// </summary>
        /// <param name="actualWorldstate"></param>
        /// <returns>bool</returns>
        public bool IsExecutable(List<GoapWorldState> actualWorldstate) {
            return Preconditions.All(actualWorldstate.Contains);
        }

//        internal Dictionary<Type,bool> PreconditionsWorldStateTypeToBool() {
//            var mappings = new Dictionary<Type, bool>();
//
//            foreach (GoapWorldState goapWorldState in preconditions) {
//                mappings.Add(goapWorldState.GetType(), goapWorldState.GetState());
//            }
//            return mappings;
//        }

        public void Execute() {
            throw new NotImplementedException();
        }

        public bool Equals(GoapAction other) {
            if (ReferenceEquals(this, other)) return true;
            return Equals(Preconditions, other.Preconditions) && Equals(Postconditions, other.Postconditions);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoapAction) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((Preconditions != null ? Preconditions.GetHashCode() : 0)*397) ^
                       (Postconditions != null ? Postconditions.GetHashCode() : 0);
            }
        }

        public static bool operator ==(GoapAction left, GoapAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoapAction left, GoapAction right) {
            return !Equals(left, right);
        }
    }
}