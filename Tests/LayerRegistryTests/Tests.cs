using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LIFE.API.Layer;
using LIFE.API.Layer.Initialization;
using NUnit.Framework;

namespace LayerRegistryTests
{
    [TestFixture]
    public class Tests
    {
        private Dictionary<string, ILayer> _localLayers;

        [SetUp]
        public void Setup()
        {
            _localLayers = new Dictionary<string, ILayer>();
        }



        [Test]
        public void Test1()
        {
            var layer = new TestLayer();
            var layer2 = new TestLayer2();

            RegisterLayer(layer);
            RegisterLayer(layer2);


        }

        private void RegisterLayer(ILayer layer)
        {
            // store in Dict for local usage, by its direct type
            _localLayers.Add(layer.GetType().FullName, layer);
            Console.WriteLine(layer.GetType().FullName);
            // and by its direct interface type if any
            if (layer.GetType().GetTypeInfo().GetInterfaces().Length > 0) {
                var infs = layer.GetType().GetTypeInfo().GetInterfaces();
                foreach (var type in infs.Where(type => type.Namespace != null && !type.Namespace.StartsWith("LIFE"))) {
                    _localLayers.Add(type.FullName, layer);
                }
            }
        }


    }


    internal class TestLayer : ITestLayer
    {
        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            throw new System.NotImplementedException();
        }

        public long GetCurrentTick()
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentTick(long currentTick)
        {
            throw new System.NotImplementedException();
        }
    }


    internal class TestLayer2 : ITestLayer2
    {
        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            throw new System.NotImplementedException();
        }

        public long GetCurrentTick()
        {
            throw new System.NotImplementedException();
        }

        public void SetCurrentTick(long currentTick)
        {
            throw new System.NotImplementedException();
        }
    }

    internal interface ITestLayer : ISteppedLayer
    {
    }

    internal interface ITestLayer2 : ISteppedLayer
    {
    }
}