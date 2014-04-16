using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadTreeLib;

namespace QuadTreeLayer.Implementation
{
    public class QuadTreeLayerNode<T> : QuadTreeNode<T> where T : IHasRect
    {
        public QuadTreeLayerNode(RectangleF bounds) : base(bounds)
        {

        }




    }
}
