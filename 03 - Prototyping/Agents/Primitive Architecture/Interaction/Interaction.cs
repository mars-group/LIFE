using System.Linq;

namespace Primitive_Architecture.Interaction {
  internal abstract class Interaction {
    
    private readonly string _id;
    protected readonly IGenericAPI Source;
    protected readonly IGenericAPI[] Targets;

    protected Interaction(string id, IGenericAPI source, IGenericAPI[] targets) {
      _id = id;
      Source = source;
      Targets = targets;
    }


    public abstract bool IsExecutable();
    public abstract void Execute();

    public new string ToString() {
      string targets;
      if (Targets.Length == 1) targets = Targets[0].ToString();
      else {
        targets = Targets.Aggregate("[", (current, next) => current + (", " + next.ToString()));
        targets += "]";
      }
      return "[Interaction] Source \"" + Source.ToString() + "\" executed \"" + _id + "\" on target \"" + targets + "\"";
    }
  }
}