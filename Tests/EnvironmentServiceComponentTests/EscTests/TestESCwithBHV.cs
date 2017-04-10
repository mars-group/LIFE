using LIFE.Components.ESC.BVH;
using LIFE.Components.ESC.Implementation;
using LIFE.Components.ESC.SpatialAPI.Entities;
using NUnit.Framework;

namespace EnvironmentServiceComponentTests.EscTests
{
    public class TestEscWithBhv : TestEsc
    {
        [SetUp]
        public void Init()
        {
            Esc = new TreeESC(new BoundingVolumeHierarchy<ISpatialEntity>());
        }

        //TODO That sh!t fails! 
        /*
        [Test]
        public void AddIntoBigDimension() {
          for (var i = 0; i < 10000; i++) {
            var b1 = BoundingBox.GenerateByDimension(Vector3.Zero, new Vector3(0.9, 0.9, 0.9));
            var t1 = new TestSpatialEntity(b1);
            Assert.IsTrue(Esc.Add(t1, new Vector3(i, i)));
          }
        }*/
    }
}