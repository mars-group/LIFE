using System;
using System.Diagnostics;
using CommonTypes.DataTypes;
using ESCTest.Entities;
using ESCTestLayer.Implementation;
using ESCTestLayer.Interface;
using NUnit.Framework;

namespace ESCTest.Tests {
    using System.Data.Entity.Spatial;
    using GenericAgentArchitecture.Movement;
    using GenericAgentArchitecture.Perception;
    using LayerAPI.Interfaces;

    public class Sharp3DTest
    {

        #region Setup / Tear down

        [SetUp]
        public void SetUp() {

        }


        [TearDown]
        public void TearDown() {}

        #endregion

        [Test]
        public void TestAddingSpatialEntities()
        {
            var octree = new Octree(new AABB());
            var sphere = new SphereSpatialEntity(new Vector(0, 0, 0), 5);
            octree.Insert(sphere);
//            Assert.True(octree.Intersects(geom2));
        }

      
    }
}