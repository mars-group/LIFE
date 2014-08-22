using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ESCTestLayer
{
    public class CollidabelElement
    {
        public int Id { get; set; }

        public int Type { get; set; }

        public Vector3f Dimension { get; set; }

        public Vector3f Position { get; set; }

        public Vector3f Direction { get; set; }

    }
}
