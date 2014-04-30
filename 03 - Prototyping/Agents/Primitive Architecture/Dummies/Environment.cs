using Primitive_Architecture.Interfaces;

namespace Primitive_Architecture.Dummies {
  
  abstract class Environment : ITickClient{

    public abstract void Tick();

  }
}
