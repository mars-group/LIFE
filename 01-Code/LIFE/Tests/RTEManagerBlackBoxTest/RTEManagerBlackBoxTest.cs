//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
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
            IRTEManager rteManager = new RTEManagerUseCase(new ResultAdapterInternalMock(), new NodeRegistryMock());
            var mockLayer = new SimpleSteppedActiveLayerMock();

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer", "MockLayer", "MockLayer"), 1);

            Assert.DoesNotThrow(() => rteManager.RegisterLayer(layerId, mockLayer));

            var mockTick = mockLayer.GetCurrentTick();
            int numberofTicks = new Random().Next(200, 500);

            for (int i = 1; i <= numberofTicks; i++)
            {
                rteManager.AdvanceOneTick();
            }

            // +1 because after executing numberOfTicks the counter will be +1 to that. 
            Assert.AreEqual(mockTick + numberofTicks + 1, mockLayer.GetCurrentTick());
        }

        [Test]
        public void RegisterAndUnregisterLayerTest()
        {
            RTEManagerUseCase rteManager = new RTEManagerUseCase(new ResultAdapterInternalMock(), new NodeRegistryMock());
            var mockLayer = new SimpleSteppedActiveLayerMock();

            var layerId = new TLayerInstanceId(new TLayerDescription("TestMockLayer", 1, 0, "MockLayer", "MockLayer", "MockLayer"), 1);

            Assert.DoesNotThrow(() => rteManager.RegisterLayer(layerId, mockLayer));

            Assert.DoesNotThrow(() => rteManager.UnregisterLayer(layerId));
        }
        

    }
}
