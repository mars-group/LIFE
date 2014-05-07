using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Primitive_Architecture.Dummies;

namespace Primitive_Architecture.Perception {
  abstract class Halo {

    private Vector _position;
    //private Geometry _form;

    protected Halo(Vector position) {
      
    }


    public abstract bool IsInRange(Vector position);


  }
}
