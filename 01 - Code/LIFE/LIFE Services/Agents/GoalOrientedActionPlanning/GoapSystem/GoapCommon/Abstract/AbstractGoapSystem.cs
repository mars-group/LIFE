using System.Collections.Generic;
using GoapCommon.Implementation;
using TypeSafeBlackboard;

namespace GoapCommon.Abstract {

    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public abstract class AbstractGoapSystem  {
        /// <summary>
        ///     blackboard property
        ///     save the current action. actions can use more than one tick, so they needed to be saved.
        /// </summary>
        public static readonly BlackboardProperty<AbstractGoapAction> ActionForExecution =
            new BlackboardProperty<AbstractGoapAction>("ActionForExecution");

        /// <summary>
        ///     blackboard property
        ///     the current world state in the goap system
        /// </summary>
        public static readonly BlackboardProperty<List<WorldstateSymbol>> Worldstate =
            new BlackboardProperty<List<WorldstateSymbol>>("Worldstate");

        /// <summary>
        ///     main method for getting the next action for execution.
        ///     creates plan if not chosen or viable.
        ///     chooses goal if current goal is satisfied and finished.
        /// </summary>
        /// <returns></returns>
        public abstract AbstractGoapAction GetNextAction();

        /// <summary>
        ///     After this call the goap manager will create a new plan.
        /// </summary>
        public abstract void ForceReplanning();
    }

}