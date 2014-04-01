using NUnit.Framework;
using System;
using LayerRegistry.Interfaces;
using LayerRegistry.Implementation;
using ExampleLayer;
using LayerAPI.Interfaces;

namespace LayerContainerComponentTests
{
	[TestFixture ()]
	public class LayerRegistryTest
	{

		private ILayerRegistry _layerRegistry;

		ISteppedLayer _layer;

		[SetUpFixture]
		public void InitFixture(){
			_layerRegistry = new LayerRegistryComponent ();
			_layer = new ExampleLayer ();
		}

		[SetUp]
		public void Init(){

		}

		[Test ()]
		public void TestRegister ()
		{
			_layerRegistry.RegisterLayer (_layer);
			Assert.AreEqual(_layer, _layerRegistry.GetLayerInstance(_layer.GetType);
		}
	}
}

