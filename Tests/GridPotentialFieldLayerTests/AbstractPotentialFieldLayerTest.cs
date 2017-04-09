using LIFE.Components.GridPotentialFieldLayer;
using NUnit.Framework;

namespace GridPotentialFieldLayerTests
{
    [TestFixture]
    public class AbstractPotentialFieldLayerTest
    {
        [SetUp]
        public void Setup()
        {
            _layer = new ConcretePotentialFieldLayer();
            _layer.LoadField(TestPath);
        }

        private const string TestPath = "./input/testPotentialField.txt";
        private GridPotentialFieldLayer _layer;

        private class ConcretePotentialFieldLayer : GridPotentialFieldLayer
        {
        }

        [Test]
        public void ShouldFindMaxPotentialFieldWithEndlessSight()
        {
            var position = _layer.ExploreClosestWithEndlessSight(4);
            Assert.AreEqual(11, position);
        }

        [Test]
        public void ShouldReturnFieldPositionWithMaxPotential()
        {
            var position = _layer.ExploreClosestFullPotentialField(6);
            Assert.AreEqual(11, position);
        }

        [Test]
        public void ShouldReturnFlaseForNonMaxPotentialField()
        {
            var hasFullPotential = _layer.HasFullPotential(2);
            Assert.IsFalse(hasFullPotential);
        }

        [Test]
        public void ShouldReturnTrueForMaxPotentialField()
        {
            var hasFullPotential = _layer.HasFullPotential(11);
            Assert.IsTrue(hasFullPotential);
        }
    }
}