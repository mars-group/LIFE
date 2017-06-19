using System.Linq;
using EnvironmentServiceComponentTests.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities;
using LIFE.Components.ESC.SpatialAPI.Entities.Movement;
using LIFE.Components.ESC.SpatialAPI.Entities.Transformation;
using LIFE.Components.ESC.SpatialAPI.Environment;
using LIFE.Components.ESC.SpatialAPI.Shape;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests
{
    public abstract class TestEsc
    {
        protected IESC Esc;

        #region Add

        [Test]
        public void AddFirstAgent()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
        }

        [Test]
        public void AddTwoEntitiesAtAdjacentPositions()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.False(Esc.Add(a2, new Vector3(0.99, 0)));
            Assert.False(Esc.Add(a2, new Vector3(0, 0.99)));

            //finally not intersecting position
            Assert.True(Esc.Add(a2, new Vector3(1, 0)));
        }


        [Test]
        public void AddTwoEntitiesAtSamePosition()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.False(Esc.Add(a2, Vector3.Zero));
        }

        [Test]
        public void AddEntitiesAtRandomPositionToFillArea()
        {
            for (var i = 0; i < 4; i++)
            {
                ISpatialEntity a = new TestSpatialEntity(Vector3.One);
                Assert.True(Esc.AddWithRandomPosition(a, Vector3.Zero, new Vector3(1, 1), true));
            }
            Assert.True(Esc.ExploreAll().Count() == 4);

            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.False(Esc.AddWithRandomPosition(a2, Vector3.Zero, new Vector3(1, 1), true));
        }

        [Test]
        public void AddEntitiesAtRandomPosition()
        {
            const int amount = 100;
            for (var i = 0; i < amount; i++)
            {
                ISpatialEntity a = new TestSpatialEntity(Vector3.One);
                Assert.True(Esc.AddWithRandomPosition(a, Vector3.Zero, new Vector3(100, 100), false));
            }
            Assert.True(Esc.ExploreAll().Count() == amount);
        }

        [Test]
        public void AddNonCollidingEntitiestAtSamePosition()
        {
            ISpatialEntity a1 = new TestSpatialEntity(10, 10, CollisionType.StaticEnvironment);
            ISpatialEntity a2 = new TestSpatialEntity(10, 10, CollisionType.StaticEnvironment);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, Vector3.Zero));
        }

        [Test]
        public void AddTouching()
        {
            var a1 = new TestSpatialEntity(new Vector3(2, 2, 2));
            var a2 = new TestSpatialEntity(new Vector3(2, 2, 2));
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, new Vector3(2, 0, 0)));
            Esc.Remove(a2);
            Assert.True(Esc.Add(a2, new Vector3(0, 2, 0)));
            Esc.Remove(a2);
            Assert.True(Esc.Add(a2, new Vector3(0, 0, 2)));
        }

        #endregion

        #region move

        [Test]
        public void MoveAgentForCollision()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, Vector3.One));
            Assert.False(Esc.Move(a2, -Vector3.One).Success);
        }

        [Test]
        public void MoveTwoEntitiesOnSamePosition()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, new Vector3(1.5, 1.5)));

            Assert.False(Esc.Move(a1, new Vector3(1.5, 1.5)).Success);

            Assert.True(Esc.Move(a2, new Vector3(3, 3)).Success);
            Assert.True(Esc.Move(a1, new Vector3(1.5, 1.5)).Success);
        }

        #endregion

        #region explore

        [Test]
        public void ExploreWithDifferentAgentType()
        {
            var shape = BoundingBox.GenerateByCorners(Vector3.Zero, Vector3.One);

            ISpatialEntity a1 = new TestSpatialEntity(1, 1);
            ISpatialEntity a2 = new ExploreEntity(shape);

            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, Vector3.One));

            var exploreShape = BoundingBox.GenerateByCorners(Vector3.Zero, new Vector3(5, 5, 5));
            ISpatialEntity explore = new ExploreEntity(exploreShape);

            Assert.True(Esc.ExploreAll().ToList().Count == 2);
            Assert.True(Esc.Explore(explore.Shape).ToList().Count == 2);
           Assert.True(Esc.Explore(explore, typeof(TestSpatialEntity)).ToList().Count == 1);
        }


        [Test]
        public void ExploreEntity()
        {
            ISpatialEntity a1 = new TestSpatialEntity(2, 2);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, new Vector3(10, 10)));
            Assert.True(Esc.Explore(new ExploreEntity(a1) * 10).Count() == 2);
        }


        [Test]
        public void ExploreWithDifferentCollisionType()
        {
            var shape = BoundingBox.GenerateByCorners(Vector3.Zero, Vector3.One);

            ISpatialEntity a1 = new TestSpatialEntity(1, 1, CollisionType.Ghost);
            ISpatialEntity a2 = new TestSpatialEntity(1, 1, CollisionType.SelfCollision);
            ISpatialEntity a3 = new ExploreEntity(shape);

            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, Vector3.Zero));
            Assert.True(Esc.Add(a3, Vector3.Zero));

            Assert.True(Esc.Explore((IShape) null).ToList().Count == 0);
            Assert.True(Esc.Explore(shape).ToList().Count == 3);
            Assert.True(Esc.Explore(shape, CollisionType.SelfCollision).ToList().Count == 1);

            Assert.True(Esc.Explore(a3).ToList().Count == 1);
        }

        #endregion

        #region collision

        [Test]
        public void CollisionInner()
        {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(1, 1));
            Collision(new TestSpatialEntity(b1), new TestSpatialEntity(b2));
        }

        [Test]
        public void CollisionOuter()
        {
            var b1 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByDimension(new Vector3(0, 0), new Vector3(4, 4));
            Collision(new TestSpatialEntity(b1), new TestSpatialEntity(b2));
        }

        [Test]
        public void CollisionByPart()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(100, 100));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(-11.5, -11.5), new Vector3(13.5, 13.5));
            Collision(new TestSpatialEntity(b1), new TestSpatialEntity(b2));
        }

        [Test]
        public void Touching()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(2, 0), new Vector3(4, 2));

            var t1 = new TestSpatialEntity(b1);
            var t2 = new TestSpatialEntity(b2);

            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));

            Esc.Add(t1, t1.Shape.Position);
            Assert.IsFalse(Esc.Explore(b2).Any());
            Esc.Remove(t1);

            Esc.Add(t2, t2.Shape.Position);
            Assert.IsFalse(Esc.Explore(b1).Any());
            Esc.Remove(t2);
        }

        [Test]
        public void CollisionByIntersectingWithOneCorner()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0.18, 0.26), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(1, 1), new Vector3(3, 3));
            Collision(new TestSpatialEntity(b1), new TestSpatialEntity(b2));
        }

        [Test]
        public void CollisionEquals()
        {
            var b1 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            var b2 = BoundingBox.GenerateByCorners(new Vector3(0, 0), new Vector3(2, 2));
            Collision(new TestSpatialEntity(b1), new TestSpatialEntity(b2));
        }

        [Test]
        public void ResizeSingleEntity()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.AreEqual(Vector3.Zero, a1.Shape.Position);

            var newShape = BoundingBox.GenerateByDimension(Vector3.Zero, Vector3.Cube(2));
            Assert.True(Esc.Resize(a1, newShape));
            Assert.AreEqual(Vector3.Zero, a1.Shape.Position);
            Assert.AreEqual(newShape, a1.Shape);
        }

        [Test]
        public void ResizeWithNewShapeAtDifferentPosition()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.AreEqual(Vector3.Zero, a1.Shape.Position);

            var newShape = BoundingBox.GenerateByDimension(Vector3.One, Vector3.Cube(2));
            Assert.True(Esc.Resize(a1, newShape));
            Assert.AreEqual(Vector3.Zero, a1.Shape.Position);
        }

        [Test]
        public void ResizeWithCollision()
        {
            ISpatialEntity a1 = new TestSpatialEntity(Vector3.One);
            ISpatialEntity a2 = new TestSpatialEntity(Vector3.One);
            Assert.True(Esc.Add(a1, Vector3.Zero));
            Assert.True(Esc.Add(a2, Vector3.One));

            var newShape = BoundingBox.GenerateByDimension(Vector3.Zero, Vector3.Cube(2));
            Assert.False(Esc.Resize(a1, newShape));
        }

        protected void Collision(TestSpatialEntity t1, TestSpatialEntity t2)
        {
            var b1 = t1.Shape.Bounds;
            var b2 = t2.Shape.Bounds;
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));

            Esc.Add(t1, t1.Shape.Position);
            Assert.IsTrue(Esc.Explore(b2).First().Shape.Bounds.Equals(b1));
            Esc.Remove(t1);

            Esc.Add(t2, t2.Shape.Position);
            Assert.IsTrue(Esc.Explore(b1).First().Shape.Bounds.Equals(b2));
            Esc.Remove(t2);
        }

        #endregion
    }
}