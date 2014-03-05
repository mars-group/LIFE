using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMercatorLib
{
    public struct Point
    {
        public Point(Double x, Double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public Double X { get; set; }
        public Double Y { get; set; }

        public new String ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }
}
