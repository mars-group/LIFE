using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GoapGraphConnector.CustomQuickGraph
{
    class GoapVertex
    {
        private readonly string _name;

        public GoapVertex(string name)
        {
            _name = name;
        }

        public string Name {
            get { return _name; }
        }
    }
}
