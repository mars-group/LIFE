using System;
using System.Windows;
using CSharpQuadTree;
using OctreeFlo.Implementation;
using SpatialCommon.Shape;
using SpatialCommon.Transformation;
using Direction = SpatialCommon.Transformation.Direction;

namespace OctreeTest.OctreeFlo
{
    class ChimeraQuadObject: IQuadObjectFlo, IQuadObject, IShape
    {
        private readonly BoundingBox _bounds;
        public Rect BoundsRect { get; set; }

        public Vector3 Position
        {
            get { return _bounds.Position; }
        }

        public Direction Rotation
        {
            get { return _bounds.Rotation; }
        }

        public BoundingBox Bounds { get { return _bounds; } set { } }
        public bool IntersectsWith(IShape shape)
        {
            return _bounds.IntersectsWith(shape);
        }

        public IShape Transform(Vector3 movement, Direction rotation)
        {
            return _bounds.Transform(movement, rotation);
        }

        public event EventHandler BoundsChanged;

        public ChimeraQuadObject(Vector3 leftBottomFront, Vector3 rightTopRear) {
            _bounds = BoundingBox.GenerateByCorners(leftBottomFront, rightTopRear);
            BoundsRect = new Rect(leftBottomFront.X, leftBottomFront.Y, _bounds.Dimension.X, _bounds.Dimension.Y);
        }

        public static ChimeraQuadObject ByDim(Vector3 position, Vector3 dimension) {
            var box = BoundingBox.GenerateByDimension(position, dimension);
            return new ChimeraQuadObject(box.LeftBottomFront, box.RightTopRear);
        }
    }
}

