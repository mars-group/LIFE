using System;
using LayerContainerFacade.Interfaces;
using LCConnector.TransportTypes;
using NUnit.Framework;


namespace LayerContainerComponentTests.LayerContainerBlackBoxTest
{
    [TestFixture]
    public class LayeContainerFassadeTest
    {
        [Test]
        public void LayerContainerFassadeInitLayerTest()
        {
            
           var layerContainerFassade = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();

            Console.WriteLine("init empty layer");
            Assert.DoesNotThrow(
                () =>
                    layerContainerFassade.InitializeLayer(
                        new TLayerInstanceId(),
                        new TInitData(true, new TimeSpan(1, 0, 0))));
            Console.WriteLine("done init layer");
        }
    }
}