// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace LIFE.Components.ObstacleLayer
{
    public class ObstacleMapToAscDto
    {
        public ObstacleMapToAscDto(double[] content, int numberOfGridCellsX, int numberOfGridCellsY, double topLat,
            double bottomLat, double leftLong, double rightLong, int cellSizeInM)
        {
            Content = content;
            NumberOfGridCellsX = numberOfGridCellsX;
            NumberOfGridCellsY = numberOfGridCellsY;
            TopLat = topLat;
            BottomLat = bottomLat;
            LeftLong = leftLong;
            RightLong = rightLong;
            CellSizeInM = cellSizeInM;
        }

        public double[] Content { get; private set; }
        public int NumberOfGridCellsX { get; private set; }
        public int NumberOfGridCellsY { get; private set; }
        public double TopLat { get; private set; }
        public double BottomLat { get; private set; }

        public double LeftLong { get; private set; }
        public double RightLong { get; private set; }
        public int CellSizeInM { get; private set; }
    }
}