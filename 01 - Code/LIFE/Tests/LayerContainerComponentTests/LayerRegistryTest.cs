using DistributedKeyValueStore.Implementation;
using LayerAPI.Interfaces;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using NodeRegistry.Implementation;
using NUnit.Framework;

namespace LayerContainerComponentTests {
    [TestFixture]
    public class LayerRegistryTest {
        private ILayerRegistry _layerRegistry;

        private ISteppedLayer _layer;

        [SetUp]
        public void Init() {
            _layerRegistry = new LayerRegistryComponent(new DistributedKeyValueStoreComponent(new NodeRegistryManager()));
            _layer = new ExampleLayer.ExampleLayer();
        }

        [Test]
        public void TestRegister() {
            _layerRegistry.RegisterLayer(_layer);
            Assert.AreEqual(_layer, _layerRegistry.GetRemoteLayerInstance(_layer.GetType()));
        }
    }
}