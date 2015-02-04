using NUnit.Framework;
using SpatialAPI.Entities.Transformation;
using SpatialAPI.Shape;

namespace SpatialAPITests {


    public class CuboidTest {
        private const double Epsilon = 0.00001;
        //TODO add 3rd dimension and rotation to tests
        [Test]
        public void TestInner() {
            Cuboid b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Cuboid b2 = new Cuboid(new Vector3(1, 1), Vector3.Zero);

            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestOuter() {
            Cuboid b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Cuboid b2 = new Cuboid(new Vector3(4, 4), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestIntersectionByPart() {
            Cuboid b1 = new Cuboid(new Vector3(100, 100), new Vector3(50, 50));
            Cuboid b2 = new Cuboid(new Vector3(25, 25), new Vector3(1, 1));
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestTouching() {
            Cuboid b1 = new Cuboid(new Vector3(1, 1), Vector3.Zero);
            Cuboid b2 = new Cuboid(new Vector3(1, 1), new Vector3(2, 0));
            Assert.IsFalse(b1.IntersectsWith(b2));
            Assert.IsFalse(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestEquals() {
            Cuboid b1 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Cuboid b2 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }

        [Test]
        public void TestContains() {
            Cuboid b1 = new Cuboid(new Vector3(1, 1), Vector3.Zero);
            Cuboid b2 = new Cuboid(new Vector3(2, 2), Vector3.Zero);
            Assert.IsTrue(b1.IntersectsWith(b2));
            Assert.IsTrue(b2.IntersectsWith(b1));
        }
    }

}