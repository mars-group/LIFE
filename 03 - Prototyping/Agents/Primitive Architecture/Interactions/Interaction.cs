﻿using Primitive_Architecture.Agents;

namespace Primitive_Architecture.Interactions {
  internal abstract class Interaction {
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