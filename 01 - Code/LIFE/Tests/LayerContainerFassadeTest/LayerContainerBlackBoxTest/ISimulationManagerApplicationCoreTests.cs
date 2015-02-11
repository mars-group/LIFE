using System;
using System.Threading;
using LayerContainerFacade.Interfaces;
using NUnit.Framework;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;

namespace LayerContainerFassadeTest.LayerContainerBlackBoxTest
{
    internal class ISimulationManagerApplicationCoreTests
    {
        private ISimulationManagerApplicationCore _simCore;
        private ILayerContainerFacade _layerCountainerCore;


        private TModelDescription GetMockModel()
        {
            string modelName = "ModelMock";

            TModelDescription model = null;
            foreach (var modelDescription in _simCore.GetAllModels())
            {
                if (modelDescription.Name.Equals(modelName))
                {
                    model = modelDescription;
                }
            }

            return model;
        }

        [SetUp]
        public void InitCore()
        {
            Console.WriteLine("try to init simcore");
            _simCore = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore();
            Console.WriteLine("simcore init done.");
            _layerCountainerCore = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();

            // parse for any given parameters and act accordingly
        }

        [Test]
        public void TickSimulationTest()
        {
           Assert.DoesNotThrow(() =>_simCore.StartSimulationWithModel(GetMockModel(), 1));   
        }

        [Test]
        public void LoadModelFromDirectoryTest()
        {
            var model = GetMockModel();

            Assert.AreEqual("ModelMock", model.Name);
        }

        [Test]
        public void StopSimulationTest()
        {
            _simCore.StartSimulationWithModel(GetMockModel(),100);
            
            Thread.Sleep(100);

            Assert.IsTrue(GetMockModel().Running);

            _simCore.PauseSimulation(GetMockModel());

            Thread.Sleep(100);

            Assert.IsFalse(GetMockModel().Running);

            _simCore.AbortSimulation(GetMockModel());

        }

    }
}