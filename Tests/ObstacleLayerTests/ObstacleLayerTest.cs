using System;
using System.Diagnostics;
using LIFE.API.Environment.GeoCommon;
using LIFE.Components.ObstacleLayer;
using Xunit;

namespace ObstacleLayerTests
{
    public class ObstacleLayerTest
    {
        [Fact]
        public void ObstacleLayerTest1()
        {
            var ob = new TestObstacleImplementation(0,1,0,1,10,10);

            var rating = ob.GetCellRating(new GeoCoordinate(0.5,0.5)));
            
            Assert.Equal(10, rating);
        }
    }
}