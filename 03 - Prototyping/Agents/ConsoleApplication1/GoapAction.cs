using System;
using System.Collections.Generic;
using System.Linq;
using GoapComponent.GoapKnowledgeProcessingComponent;
using Common.Interfaces;

namespace GoapComponent  {
    /// <summary>
    ///     goap actions define a transition to another worldstate
    /// </summary>
    internal abstract class GoapAction : IEquatable<GoapAction>, IInteraction {
        protected List<GoapWorldState> preconditions;
        protected List<GoapWorldState> postconditions;

        /// <summary>
        ///     check if preconditions of action are subset of current world
        /// </summary>
        /// <param name="actualWorldstate"></param>
        /// <returns>bool</returns>
        protected bool IsExecutable(List<GoapWorldState> actualWorldstate) {
            if (preconditions.All(actualWorldstate.Contains)) return true;
            return false;
        }


        public bool Equals(GoapAction other) {
            if (ReferenceEquals(this, other)) return true;
            return Equals(preconditions, other.preconditions) && Equals(postconditions, other.postconditions);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((GoapAction) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return ((preconditions != null ? preconditions.GetHashCode() : 0)*397) ^
                       (postconditions != null ? postconditions.GetHashCode() : 0);
            }
        }

      public void Execute() {
        throw new NotImplementedException();
      }

      public static bool operator ==(GoapAction left, GoapAction right) {
            return Equals(left, right);
        }

        public static bool operator !=(GoapAction left, GoapAction right) {
            return !Equals(left, right);
        }
    }
}