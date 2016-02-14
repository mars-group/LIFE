//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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
