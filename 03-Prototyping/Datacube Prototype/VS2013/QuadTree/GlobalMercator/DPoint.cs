using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMercatorLib
{
    public struct DPoint
    {
        public DPoint(Double x, Double y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Double X { get; set; }
        public Double Y { get; set; }

        public static implicit operator DPoint(Point p)
        {
            return new DPoint(p.X, p.Y);
        }
    }
}
