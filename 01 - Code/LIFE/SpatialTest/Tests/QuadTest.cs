using LifeAPI.Spatial;
using LifeAPI.Spatial.Shape;
using NUnit.Framework;

namespace SpatialTest.Tests {

    public class QuadTest {
        [Test]
        public void TestContains() {
            TVector dimension1 = new TVector(10, 10, 10);
            TVector dimension2 = new TVector(1, 1, 1);
            Quad big = new Quad(dimension1, position: new TVector(0, 0, 0), rotation: new Direction());
            Quad contained = new Quad(dimension2, new TVector(0, 0, 0), new Direction());
            Assert.True(big.CollidesWith(contained));

            contained = new Quad(dimension2, new TVector(5, 5, 5), new Direction());
            Assert.True(big.CollidesWith(contained));
        }

        [Test]
        public void TestTouches() {
            TVector dimension = new TVector(10, 10, 10);
            Quad quad1 = new Quad(dimension, new TVector(0, 0, 0), new Direction());
            Quad quad2 = new Quad(dimension, new TVector(10, 10, 10), new Direction());
            Assert.False(quad1.CollidesWith(quad2)); // touches is no collision
        }

        [Test]
        public void TestIntersects() {
            TVector dimension = new TVector(10, 10, 10);
            Quad quad1 = new Quad(dimension, new TVector(0, 0, 0), new Direction());
            Quad quad2  = new Quad(dimension, new TVector(10, 9.99999, 9.99999), new Direction());
            Assert.False(quad1.CollidesWith(quad2));
            quad2 = new Quad(dimension, new TVector(9.99999, 10, 9.99999), new Direction());
            Assert.False(quad1.CollidesWith(quad2));
            quad2 = new Quad(dimension, new TVector(9.99999, 9.99999, 10), new Direction());
            Assert.False(quad1.CollidesWith(quad2));
            quad2 = new Quad(dimension, new TVector(9.99999, 9.99999, 9.99999), new Direction());
            Assert.True(quad1.CollidesWith(quad2));
        }
    }

}