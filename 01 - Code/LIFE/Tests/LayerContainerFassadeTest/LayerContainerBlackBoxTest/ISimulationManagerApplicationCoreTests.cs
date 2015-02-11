using LayerContainerFacade.Interfaces;
using NUnit.Framework;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;

namespace LayerContainerFassadeTest.LayerContainerBlackBoxTest
{
   
    public class ISimulationManagerApplicationCoreTests
    {
        //private ISimulationManagerApplicationCore _simcCore;
        //private ILayerContainerFacade _layerContainerFacade;


        //[TestFixtureSetUp]
        //public void InitSimCore()
        //{
        //    _simcCore = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore();
        //    _layerContainerFacade = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();
        //}

        //[TestFixtureTearDown]
        //public void TearDownSimCore()
        //{
        //    //no way to shut down simcore...    
        //}

        //private TModelDescription GetMockModel()
        //{
        //    string modelName = "ModelMock";

        //    TModelDescription model = null;
        //    foreach (var modelDescription in _simcCore.GetAllModels())
        //    {
        //        if (modelDescription.Name.Equals(modelName))
        //        {
        //            model = modelDescription;
        //        }
        //    }

        //    return model;
        //}


        //[Test]
        //public void TickSimulationTest()
        //{
        //    Assert.DoesNotThrow(() => _simcCore.StartSimulationWithModel(GetMockModel(), 1));
        //}

        //[Test]
        //public void LoadModelFromDirectoryTest()
        //{
        //    var model = GetMockModel();

        //    Assert.AreEqual("ModelMock", model.Name);
        //}


        //<summary>
        //Test is not active because api does not set running flag.
        //</summary>
        //[Test]
        //public void StopSimulationTest()
        //{
        //    

        //    _simcCore.StartSimulationWithModel(GetMockModel(), 100);

        //    Thread.Sleep(100);

        //    Assert.IsTrue(GetMockModel().Running);

        //    _simcCore.PauseSimulation(GetMockModel());

        //    Thread.Sleep(100);

        //    Assert.IsFalse(GetMockModel().Running);

        //    _simcCore.AbortSimulation(GetMockModel());
        //}
    }
}