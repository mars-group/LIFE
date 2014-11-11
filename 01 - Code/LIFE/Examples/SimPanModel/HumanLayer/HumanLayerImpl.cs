using System;
using System.IO;
using System.Reflection;
using CellLayer;
using DalskiAgent.Execution;
using DalskiAgent.Movement;
using HumanLayer.Agents;
using LayerAPI.Interfaces;
using log4net;
using log4net.Config;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace HumanLayer {
    [Extension(typeof (ISteppedLayer))]
    public class HumanLayerImpl : ISteppedLayer {
        public static float HumanDiameter = 0.5f;
        public static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly CellLayerImpl _cellLayer;

        public HumanLayerImpl(CellLayerImpl cellLayer) {
            XmlConfigurator.Configure(new FileInfo("conf.log4net"));
            _cellLayer = cellLayer;
        }

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {

            IExecution exec = new LayerExec(registerAgentHandle, unregisterAgentHandle, this);
            int xPos, yPos;


            Guid guid = new Guid();
            _cellLayer.GiveAndSetToRandomPosition(guid, 0.5f, out xPos, out yPos);
            SmartHuman human = new SmartHuman(exec, _cellLayer, new Vector(xPos, yPos), guid);
            

            

            /*
            Human humanAgent = new Human(_cellLayer);
            registerAgentHandle.Invoke(this, humanAgent);
            */
             return true;
             
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        #endregion

        


        
    }
}