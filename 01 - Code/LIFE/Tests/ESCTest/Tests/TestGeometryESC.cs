using System;
using System.Linq;
using EnvironmentServiceComponent.Entities.Shape;
using EnvironmentServiceComponent.Implementation;
using ESCTest.Entities;
using GeoAPI.Geometries;
using LifeAPI.Spatial;
using NetTopologySuite.Geometries;
using NUnit.Framework;

namespace ESCTest.Tests {

    //public class TestGeometryESC : TestESC {
    //    #region Setup / Tear down

    //    [SetUp]
    //    public void SetUp() {
    //        _esc = new GeometryESC();
    //    }


    //    [TearDown]
    //    public void TearDown() {}

    //    #endregion

    //    [Test]
    //    public void TestMoveAndCollideWithOneOtherAgent() {
    //        ISpatialEntity a1 = GenerateAgent(0.9, 0.9);
    //        ISpatialEntity a2 = GenerateAgent(0.9, 0.9);

    //        Assert.True(_esc.Add(a1, new TVector(0, 0)));
    //        Assert.True(_esc.Add(a2, new TVector(0, 1)));

    //        IPoint oldCentroid = (a2.Shape as GeometryShape).Geometry.Centroid;
    //        MovementResult movementResult = _esc.Move(a2, new TVector(0, -1));
    //        Assert.False(movementResult.Success);
    //        Assert.True(movementResult.Collisions.Contains(a1));
    //        Assert.True((a2.Shape as GeometryShape).Geometry.Centroid.Equals(oldCentroid));
    //    }


    //    [Test]
    //    public void TestMoveAround() {
    //        GeometryAgent a1 = new GeometryAgent(2, 2);
    //        Assert.True(_esc.Add(a1, new TVector(1, 1)));
    //        Assert.True(_esc.Move(a1, new TVector(0, 1)).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(1, 2)));
    //        Assert.True(_esc.Move(a1, new TVector(0, -1)).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(1, 1)));
    //        Assert.True(_esc.Move(a1, new TVector(10, 0)).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(11, 1)));
    //        Assert.True(_esc.Move(a1, new TVector(-5, 0)).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(6, 1)));
    //        Assert.True(_esc.Move(a1, new TVector(-3.5d, 7.71d)).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Centroid.Equals(new Point(2.5d, 8.71d)));
    //    }

    //    protected void PrintAllAgents() {
    //        Console.WriteLine(_esc.ExploreAll().Count() + " Agents found.");
    //        foreach (ISpatialEntity entity in _esc.ExploreAll()) {
    //            GeometryShape geometryShape = entity.Shape as GeometryShape;
    //            Console.WriteLine(entity + " " + geometryShape.Geometry.Envelope);
    //        }
    //        Console.WriteLine("---");
    //    }

    //    [Test]
    //    public void TestResize() {
    //        GeometryAgent a1 = new GeometryAgent(2, 2);
    //        GeometryAgent a2 = new GeometryAgent(2, 2);
    //        Assert.True(_esc.Add(a1, new TVector(0, 0)));
    //        Assert.True(_esc.Add(a2, new TVector(3.1, 0)));

    //        TVector pos1 = a1.Shape.GetPosition();
    //        IGeometry newGeometry = MyGeometryFactory.Rectangle(6, 2, new Coordinate(pos1.X, pos1.Y));
    //        Assert.False(_esc.Resize(a2, new ExploreShape(newGeometry)));
    //        PrintAllAgents();
    //        TVector pos2 = a2.Shape.GetPosition();
    //        newGeometry = MyGeometryFactory.Rectangle(4, 2, new Coordinate(pos2.X, pos2.Y));
    //        Assert.True(_esc.Resize(a2, new ExploreShape(newGeometry)));
    //    }

    //    [Test]
    //    public void TestRotation() {
    //        GeometryAgent a1 = new GeometryAgent(4, 2);
    //        Assert.True(_esc.Add(a1, new TVector(0, 0)));
    //        Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, -1.9)));
    //        Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, 1.9)));
    //        Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, -1.9)));
    //        Assert.False((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, 1.9)));

    //        Direction direction = new Direction();
    //        direction.SetYaw(90);
    //        TVector rotationVector = direction.GetDirectionalVector().GetTVector();
    //        Assert.True(_esc.Move(a1, TVector.Origin, rotationVector).Success);
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, -1.9)));
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(-0.9, 1.9)));
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, -1.9)));
    //        Assert.True((a1.Shape as GeometryShape).Geometry.Intersects(new Point(0.9, 1.9)));
    //    }

    //    protected override ISpatialEntity GenerateAgent(double x, double y) {
    //        return GenerateAgent(x, y, CollisionType.MassiveAgent);
    //    }

    //    protected override ISpatialEntity GenerateAgent(double x, double y, CollisionType collisionType) {
    //        return new GeometryAgent(x, y, collisionType);
    //    }
    //}

}