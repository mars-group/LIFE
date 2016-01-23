using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GlobalMercatorLib
{
    /// <summary>
    /// Reference to a Tile X, Y index
    /// </summary>
    public class Tile
    {
        public Tile()
        {
        }

        public Tile(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
}
