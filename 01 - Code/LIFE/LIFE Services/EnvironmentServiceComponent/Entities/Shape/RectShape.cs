using System;
using System.Windows;
using CSharpQuadTree;
using SpatialCommon.Interfaces;
using SpatialCommon.TransportTypes;

namespace EnvironmentServiceComponent.Entities.Shape {

    public class RectShape : IShape, IQuadObject {
        private Rect _bounds;

        protected RectShape(Rect bounds) {
            _bounds = bounds;
        }

        public RectShape(TVector position, TVector dimension) {
//            _bounds = new Rect(position.X - dimension.X / 2, position.Y - dimension.Y / 2, dimension.X, dimension.Y);
            _bounds.X = position.X - dimension.X/2;
            _bounds.Y = position.Y - dimension.Y/2;
            _bounds.Width = dimension.X;
            _bounds.Height = dimension.Y;
        }

        #region IQuadObject Members

        public Rect Bounds { get { return _bounds; } set { _bounds = value; } }
        public event EventHandler BoundsChanged;

        #endregion

        #region IShape Members

        public TVector GetPosition() {
            Point topLeft = _bounds.TopLeft;
            return new TVector(topLeft.X + _bounds.Width/2, topLeft.Y + _bounds.Height/2);
        }

        #endregion

        public override string ToString() {
            return String.Format("RectShape({0}, {1}->{2})", GetPosition(), _bounds.TopLeft, _bounds.BottomRight);
        }
    }

}