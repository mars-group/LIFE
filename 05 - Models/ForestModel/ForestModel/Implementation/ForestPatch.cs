using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ForestModel.Implementation.Agents;
using QuadTreeLib;

namespace ForestModel.Implementation
{
    class ForestPatch : QuadTreeNode<ForestPatch>, IHasRect
    {


        private RectangleF _bounds;
        private List<TreeAgent> _treesInsidePatch;

        public ForestPatch(Point p, int size)
            : base(new RectangleF(p.X, p.Y, size, size))
        {
            _bounds = new RectangleF(p.X, p.Y, size, size);
            _treesInsidePatch = new List<TreeAgent>();
        }




        public RectangleF Rectangle
        {
            get { return _bounds; }
            private set { }
        }
    }
}
