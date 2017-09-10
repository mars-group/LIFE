namespace GpuEnvironment.Types
{
    public struct CollisionConstants
    {
        public uint numThreadsPerBlock;
        public uint numTotalElements;
        public float xMax;
        public float yMax;
        public float zMax;
        public float cellSizeX;
        public float cellSizeY;
        public float cellSizeZ;
        public uint xGridBoundary;
        public uint yGridBoundary;
        public uint zGridBoundary;
        public int numBlocks;
    }

    public struct CollisionTupel
    {
        public uint obj1;
        public uint obj2;
    }

    public struct ShapeTupel
    {
        public clRectangleShapeObject obj1;
        public clRectangleShapeObject obj2;
    }

    public struct clPoint3D
    {
        public float x;
        public float y;
        public float z;
    }

   
    public struct clRectangleShapeObject
    {
        public clPoint3D center;
        public clPoint3D leftBottomFront;
        public clPoint3D rigthTopRear;
    }
    
    public struct CollisionCell3D
    {
        public uint x;
        public uint y;
        public uint z;
    }

    // -- Classes for 2D hierarchical Gpu ESC

//    public struct clCircleShapeObject
//    {
//        public clPoint3D center;
//        public float radius;
//    }

    public struct CollisionCell2D
    {
        public uint x;
        public uint y;
    }
    public struct clPoint2D
    {
        public float x;
        public float y;
    }

}
