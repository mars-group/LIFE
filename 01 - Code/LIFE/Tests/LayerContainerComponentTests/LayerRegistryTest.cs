using AppSettingsManager;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using LifeAPI.Layer;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using NUnit.Framework;

namespace LayerContainerComponentTests {
    [TestFixture]
    public class LayerRegistryTest {
        private ILayerRegistry _layerRegistry;

        private ISteppedLayer _layer;

        [SetUp]
        public void Init() {
            var nodeREgistryConfig = new NodeRegistryConfig();
            _layerRegistry = new LayerRegistryComponent(
                    new NodeRegistryComponent(
                        new MulticastAdapterComponent(new GlobalConfig(), new MulticastSenderConfig()), nodeREgistryConfig),
                        nodeREgistryConfig);
            _layer = new ExampleLayer.ExampleLayer();
        }

        [Test]
        public void TestRegister() {
            _layerRegistry.RegisterLayer(_layer);
			var retreivedLayer = _layerRegistry.GetLayerInstance (_layer.GetType ());
			Assert.AreEqual(_layer, retreivedLayer);
        }
    }
}