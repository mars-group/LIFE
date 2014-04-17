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
    public class ForestPatch : QuadTreeNode<ForestPatch>, IHasRect
    {

        private List<TreeAgent> _trees;

        public List<TreeAgent> Trees
        {
            get { return _trees; }
            set { _trees = value; }
        }

        public ForestPatch(PointF p, int size)
            : base(new RectangleF(p.X, p.Y, size, size))
        {
          _trees = new List<TreeAgent>();
        }

        public ForestPatch(PointF p, int height, int witdh)
            : base(new RectangleF(p.X, p.Y, height, witdh))
        {
          
            _trees = new List<TreeAgent>();
        }

        public RectangleF Rectangle
        {
            get { return base.Bounds; }
            private set { }
        }


        public void addTreeToPatch(TreeAgent tree)
        {
            //TODO max Trees pro patch?
            _trees.Add(tree);
        }
    }
}
