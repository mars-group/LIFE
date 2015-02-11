using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerContainerFacade.Interfaces;
using LCConnector.TransportTypes;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using SimulationManagerFacade.Implementation;
using SimulationManagerFacade.Interface;
using SMConnector.TransportTypes;

namespace SimulationManagerTest.SimulationManagerFassadeTest
{
    class SimulationManagerFassadeTest
    {
        //[Test]
        //public void IniMockLayerTest()
        //{

            
        //    Console.WriteLine("try to  get production ready core");
        //    var simManger = SimulationManagerApplicationCoreFactory.GetProductionApplicationCore();
        //    Console.WriteLine("done");

        //    var layerContainerFassade = LayerContainerApplicationCoreFactory.GetLayerContainerFacade();

        //    //layerContainerFassade.InitializeLayer(new TLayerInstanceId(),new TInitData(false,new TimeSpan(1,0,0)));

        //    var models = simManger.GetAllModels();
        //    int i = 0;

        //    for (int j = 0; j < models.Count; j++)
        //    {
        //        if (models.ToList()[j].Name == "ModelMock")
        //        {
        //            i = j;
        //        }
        //    }

            
            
        //}
    }
}
