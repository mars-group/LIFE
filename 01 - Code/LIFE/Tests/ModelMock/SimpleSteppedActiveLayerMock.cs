using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LCConnector.TransportTypes;
using LifeAPI.Agent;
using LifeAPI.Layer;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]
namespace ModelMock
{


    public class SimpleSteppedActiveLayerMock : ISteppedActiveLayer
    {
        
        private long _tick;

        public bool InitLayer(TInitData layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle)
        {
            _tick = 0;

            return true;
        }

        public long GetCurrentTick()
        {
            return _tick;
        }

        public void SetCurrentTick(long currentTick)
        {
            _tick = currentTick;
        }

        public void Tick()
        {
           _tick = _tick + 1;
            //Simulation some work
            Thread.Sleep(10);
        }

        public void PreTick()
        {
           
        }

        public void PostTick()
        {
           
        }
    }
}
