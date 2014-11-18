using System.Collections.Generic;
using System.Runtime.Serialization;
using GenericAgentArchitectureCommon.Interfaces;
using GoapCommon.Implementation;
using TypeSafeBlackboard;

namespace GoapCommon.Abstract {

    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public abstract class AbstractGoapSystem : IAgentLogic {
        public static BlackboardProperty<AbstractGoapAction> ActionForExecution =
            new BlackboardProperty<AbstractGoapAction>("ActionForExecution");

        public static readonly BlackboardProperty<List<WorldstateSymbol>> Worldstate =
            new BlackboardProperty<List<WorldstateSymbol>>("Worldstate");

        #region IAgentLogic Members

        /// <summary>
        ///     get next valid action
        /// </summary>
        /// <returns>IInteraction</returns>
        public IInteraction Reason() {
            return GetNextAction();
        }

        #endregion

        public abstract AbstractGoapAction GetNextAction();
    }

}