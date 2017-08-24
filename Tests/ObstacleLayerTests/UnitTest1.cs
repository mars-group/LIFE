using System;
using LIFE.API.Environment.GeoCommon;
using LIFE.Components.ObstacleLayer;
using Xunit;

namespace ObstacleLayerTests
{
    public class UnitTest1
    {
        [Fact]
        public void ObstacleMapTest1()
        {
            var obstacleMap = new ObstacleMapLoader().LoadObstacleMap("./dgvm.csv");
            
            //top left corner
            Assert.Equal(15459,obstacleMap.GetCellRating(new GeoCoordinate(-21.74,30.75)));
            //top right corner
            Assert.Equal(16394,obstacleMap.GetCellRating(new GeoCoordinate(-21.74,32.25)));
            //bottom left corner
            Assert.Equal(11673,obstacleMap.GetCellRating(new GeoCoordinate(-25.25,30.75)));
            //bottom right corner
            Assert.Equal(11186,obstacleMap.GetCellRating(new GeoCoordinate(-25.25,32.25)));
        }
    }
}