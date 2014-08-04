using GenericAgentArchitecture.Agents;
using GenericAgentArchitecture.Interfaces;

namespace GenericAgentArchitecture.Interactions {
  public abstract class Interaction : IInteraction{
    protected readonly IGenericAPI Source;
    public Agent Target;

    protected Interaction(IGenericAPI source) {
      Source = source;
    }


    public abstract bool CheckPreconditions();
    public abstract bool CheckTrigger();
    public abstract void Execute();
  }
}