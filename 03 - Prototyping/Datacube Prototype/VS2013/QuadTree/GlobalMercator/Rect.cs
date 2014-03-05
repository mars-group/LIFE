using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMercatorLib
{
    public class Rect
    {
        public Point TopLeft { get; set; }
        public Point BottomRight { get; set; }

        public Rect(Point tl, Point br)
        {
            this.TopLeft = tl;
            this.BottomRight = br;
        }

        public Double Left
        {
            get
            {
                return TopLeft.X;
            }
        }

        public Double Top
        {
            get
            {
                return TopLeft.Y;
            }
        }

        public Double Right
        {
            get
            {
                return BottomRight.X;
            }
        }

        public Double Bottom
        {
            get
            {
                return BottomRight.Y;
            }
        }

        public new string ToString()
        {
            return "[TL:" + TopLeft.ToString() + ", BR:" + BottomRight.ToString() + "]";
        }
    }
}
