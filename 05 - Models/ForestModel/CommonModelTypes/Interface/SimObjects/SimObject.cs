using System.Drawing;
using QuadTreeLib;

namespace CommonModelTypes.Interface.SimObjects
{
    public abstract class SimObject : IHasRect
    {
        public int ID { get; private set; }
        private RectangleF _bounds;




        public SimObject(int id, Point point, int size)
        {
            ID = id;
          _bounds = new RectangleF(point.X, point.Y, size, size);
        }

        public SimObject(int id, RectangleF bounds)
        {
            _bounds = bounds;
            ID = id;
        }

        public SimObject(int id, Point point, int width, int height)
        {
            ID = id;
            _bounds = new RectangleF(point.X, point.Y, width, height) ;
        }

        public RectangleF Rectangle
        {
            get { return _bounds; }
            private set { }
        }
    }
}
