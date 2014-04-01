using NUnit.Framework;
using System;
using LayerRegistry.Interfaces;
using LayerRegistry.Implementation;
using ExampleLayer;
using LayerAPI.Interfaces;
using NodeRegistry.Implementation;

namespace LayerContainerComponentTests
{
	[TestFixture ()]
	public class LayerRegistryTest
	{

		private ILayerRegistry _layerRegistry;

		ISteppedLayer _layer;

		[SetUp]
		public void Init(){
			_layerRegistry = new LayerRegistryComponent(new NodeRegistryManager());
			_layer = new ExampleLayer.ExampleLayer();
		}

		[Test ()]
		public void TestRegister ()
		{
			_layerRegistry.RegisterLayer (_layer);
			Assert.AreEqual(_layer, _layerRegistry.GetRemoteLayerInstance(_layer.GetType()));
		}
	}
}