using System.Collections.Generic;
using GenericAgentArchitectureCommon.Interfaces;
using GoapBetaCommon.Implementation;
using TypeSafeBlackboard;

namespace GoapBetaCommon.Abstract {

    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public abstract class AbstractGoapSystem : IAgentLogic {
        public static BlackboardProperty<AbstractGoapAction> ActionForExecution =
            new BlackboardProperty<AbstractGoapAction>();

        public static readonly BlackboardProperty<List<WorldstateSymbol>> Worldstate =
            new BlackboardProperty<List<WorldstateSymbol>>();

        #region IAgentLogic Members

        /// <summary>
        ///     get next valid action
        /// </summary>
        /// <returns>IGoapAction</returns>
        public IInteraction Reason() {
            return GetNextAction();
        }

        #endregion

        public abstract AbstractGoapAction GetNextAction();
    }

}