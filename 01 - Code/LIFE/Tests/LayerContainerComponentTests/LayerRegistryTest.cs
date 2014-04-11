﻿using AppSettingsManager;
using ConfigurationAdapter.Interface;
using DistributedKeyValueStore.Implementation;
using LayerAPI.Interfaces;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using LayerRegistry.Interfaces.Config;
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

            _layerRegistry = new LayerRegistryComponent(
                new DistributedKeyValueStoreComponent(
                    new NodeRegistryComponent(
                        new MulticastAdapterComponent(new GlobalConfig(),new MulticastSenderConfig()), new NodeRegistryConfig())),
                        new LayerRegistryConfig()
                        );
            _layer = new ExampleLayer.ExampleLayer();
        }

        [Test]
        public void TestRegister() {
            _layerRegistry.RegisterLayer(_layer);
            Assert.AreEqual(_layer, _layerRegistry.GetLayerInstance(_layer.GetType()));
        }
    }
}