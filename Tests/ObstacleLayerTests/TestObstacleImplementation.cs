using LIFE.Components.ObstacleLayer;

namespace ObstacleLayerTests
{
    public class TestObstacleImplementation : ObstacleMap
    {
        public TestObstacleImplementation(double topLat, double bottomLat, double leftLong, double rightLong, int cellSizeInM, double? defaultInitValue = null) : base(topLat, bottomLat, leftLong, rightLong, cellSizeInM, defaultInitValue)
        {
        }
    }
}