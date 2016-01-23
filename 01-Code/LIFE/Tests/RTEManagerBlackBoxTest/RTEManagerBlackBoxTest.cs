using System;
using LCConnector.TransportTypes;
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

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer", "MockLayer", "MockLayer"), 1);

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

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer", "MockLayer", "MockLayer"), 1);

            Assert.DoesNotThrow(() => rteManager.RegisterLayer(layerId, mockLayer));

            Assert.DoesNotThrow(() => rteManager.UnregisterLayer(layerId));
        }
        

    }
}
