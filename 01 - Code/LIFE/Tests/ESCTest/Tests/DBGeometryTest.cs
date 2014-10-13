using System;
using System.Diagnostics;
using CommonTypes.DataTypes;
using ESCTest.Entities;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using NUnit.Framework;

namespace ESCTest.Tests {
    using System.Data.Entity.Spatial;

    public class DBGeometryTest
    {

        #region Setup / Tear down

        [SetUp]
        public void SetUp() {
//            var text = "LINE(29.11, 40.11)";
//            DbGeography.LineFromText(text, 0);  //I can generate a point with this code
        }


        [TearDown]
        public void TearDown() {}

        #endregion

        [Test]
        public void TestIntersection()
        {
            DbGeometry geom1 = DbGeometry.FromText("POLYGON ((0 0, 10 0, 10 10, 0 10, 0 0))");
            DbGeometry geom2 = DbGeometry.FromText("POLYGON ((2 2, 12 2, 12 12, 2 12, 2 2))");
            Console.WriteLine(geom1.Intersection(geom2));

            Assert.True(geom1.Intersects(geom2));
        }

      
    }
}