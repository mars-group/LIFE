﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using CommonTypes;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LayerContainerShared;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using ResultAdapter.Interface;
using RTEManager.Interfaces;

namespace LayerContainerFacade.Implementation {
    [ScsService]
	internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;
        private IScsServiceApplication _server;



        public LayerContainerFacadeImpl
            (LayerContainerSettings settings,
                IPartitionManager partitionManager,
                IRTEManager rteManager,
                IResultAdapter resultAdapter,
                INodeRegistry nodeRegistry) {

            _partitionManager = partitionManager;
            _rteManager = rteManager;
            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));
            _server.AddService<ILayerContainer, LayerContainerFacadeImpl>(this);

            //Start server
            _server.Start();

            nodeRegistry.JoinCluster();
        }

        #region ILayerContainerFacade Members

        public void LoadModelContent(ModelContent content) {
            _partitionManager.LoadModelContent(content);
        }

        public void Instantiate(TLayerInstanceId instanceId) {
            _partitionManager.AddLayer(instanceId);
        }

        public bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData) {
            return _rteManager.InitializeLayer(instanceId, initData);
        }
        public long Tick(long amountOfTicks = 1)
        {
            return _rteManager.Advance(amountOfTicks);
        }
        public void CleanUp ()
		{
			_rteManager.DisposeSuitableLayers ();
		}

		public void SetMarsConfigServiceAddress(string marsConfigServiceAddress)
		{
			MARSConfigServiceSettings.Address = marsConfigServiceAddress;
		}

		#endregion



	}

}