using System.Collections.Generic;
using GenericAgentArchitectureCommon.Interfaces;
using GoapBetaCommon.Interfaces;
using TypeSafeBlackboard;

namespace GoapBetaCommon.Abstract {
    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public abstract class AbstractGoapSystem : IAgentLogic {

        public static BlackboardProperty<AbstractGoapAction> ActionForExecution = new BlackboardProperty<AbstractGoapAction>();
        public static readonly BlackboardProperty<List<IGoapWorldProperty>> Worldstate = new BlackboardProperty<List<IGoapWorldProperty>>();
        
        /// <summary>
        ///     get next valid action
        /// </summary>
        /// <returns>IGoapAction</returns>
      public IInteraction Reason() {
        return GetNextAction();
      }

      public abstract AbstractGoapAction GetNextAction();
    }
}