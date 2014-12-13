using System.Windows;
using LifeAPI.Spatial;

namespace EnvironmentServiceComponent.Implementation {

    public static class MyRectFactory {
        /// <summary>
        ///     Creates a rectangle with given dimension at origin.
        /// </summary>
        /// <param name="width">Horizontal dimension.</param>
        /// <param name="height">Vertical dimension.</param>
        /// <returns>A rectangle that is an IGeometry.</returns>
        public static Rect Rectangle(double width, double height) {
            Rect _bounds = new Rect();
            _bounds.X = - width/2;
            _bounds.Y = - height/2;
            _bounds.Width = width;
            _bounds.Height = height;
            return _bounds;
        }

        /// <summary>
        ///     Creates a rectangle with given dimension at origin.
        /// </summary>
        /// <param name="width">Horizontal dimension.</param>
        /// <param name="height">Vertical dimension.</param>
        /// <returns>A rectangle that is an IGeometry.</returns>
        public static Rect Rectangle(TVector position, TVector dimension) {
            Rect _bounds = new Rect();
            _bounds.X = position.X - dimension.X / 2;
            _bounds.Y = position.Y - dimension.Y / 2;
            _bounds.Width = dimension.X;
            _bounds.Height = dimension.Y;
            return _bounds;
        }
    }

}