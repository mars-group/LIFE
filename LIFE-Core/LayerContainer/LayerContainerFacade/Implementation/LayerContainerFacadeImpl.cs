//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using CommonTypes;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LayerContainerShared;
using LCConnector;
using LCConnector.TransportTypes.ModelStructure;
using LIFE.API.Layer.Initialization;
using NodeRegistry.Interface;
using PartitionManager.Interfaces;
using ResultAdapter.Interface;
using RTEManager.Interfaces;

namespace LayerContainerFacade.Implementation {
    [ScsService]
	internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;

        public LayerContainerFacadeImpl
            (LayerContainerSettings settings,
                IPartitionManager partitionManager,
                IRTEManager rteManager,
                IResultAdapter resultAdapter,
                INodeRegistry nodeRegistry) {

            _partitionManager = partitionManager;
            _rteManager = rteManager;
            var server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));
            server.AddService<ILayerContainer, LayerContainerFacadeImpl>(this);

            //Start server
            server.Start();

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