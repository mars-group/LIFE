using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using ModelMock;
using NUnit.Framework;
using RTEManager.Implementation;
using RTEManager.Interfaces;
using RTEManagerBlackBoxTest.Mocks;

namespace RTEManagerBlackBoxTest
{
    public class RTEManagerBlackBoxTest
    {


        [Test]
        public void TickLayerTest()
        {
            IRTEManager rteManager = new RTEManagerUseCase(new VisualizationAdapterInternalMock(), new NodeRegistryMock());
            var mockLayer = new SimpleSteppedActiveLayerMock();

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer"), 1);

            Assert.DoesNotThrow(() => rteManager.RegisterLayer(layerId, mockLayer));

            var mockTick = mockLayer.GetCurrentTick();
            int numberofTicks = new Random().Next(200, 500);

            for (int i = 0; i < numberofTicks; i++)
            {
                rteManager.AdvanceOneTick();
            }

            Assert.AreEqual(mockTick + numberofTicks, mockLayer.GetCurrentTick());
        }

        [Test]
        public void RegisterAndUnregisterLayerTest()
        {
            RTEManagerUseCase rteManager = new RTEManagerUseCase(new VisualizationAdapterInternalMock(), new NodeRegistryMock());
            var mockLayer = new SimpleSteppedActiveLayerMock();

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer"), 1);

            Assert.DoesNotThrow(() => rteManager.RegisterLayer(layerId, mockLayer));

            Assert.IsTrue(rteManager.GetLayers().Contains(mockLayer));

            Assert.DoesNotThrow(() => rteManager.UnregisterLayer(layerId));

            Assert.IsFalse(rteManager.GetLayers().Contains(mockLayer));
        }
        

    }
}
