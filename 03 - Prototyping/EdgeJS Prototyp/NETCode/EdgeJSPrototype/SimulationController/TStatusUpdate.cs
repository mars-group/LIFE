using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimulationController
{
    public class TStatusUpdate
    {
        public string Name { get; private set; }

        public TStatusUpdate(string name) {
            Name = name;
        }
    }
}
