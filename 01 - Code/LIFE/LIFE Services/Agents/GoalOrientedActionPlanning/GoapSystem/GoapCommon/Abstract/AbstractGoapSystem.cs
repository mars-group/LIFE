using GenericAgentArchitectureCommon.Interfaces;
using GoapCommon.Interfaces;

namespace GoapCommon.Abstract {
    /// <summary>
    ///     action system is the common type of dicision making agent component
    /// </summary>
    public abstract class AbstractGoapSystem : IAgentLogic{
        
      
      /// <summary>
        ///     get next valid action
        /// </summary>
        /// <returns>IGoapAction</returns>
      public IInteraction Reason() {
        return GetNextAction();
      }

      public abstract IGoapAction GetNextAction();
    }
}