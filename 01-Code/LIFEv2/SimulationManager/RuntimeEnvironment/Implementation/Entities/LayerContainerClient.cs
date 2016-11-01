//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Net;
using CommonTypes;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using RuntimeEnvironment.Interfaces.Exceptions;

namespace RuntimeEnvironment.Implementation.Entities {
    internal class LayerContainerClient {
        private readonly IScsServiceClient<ILayerContainer> _layerContainer;

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer, ModelContent content) {
            _layerContainer = layerContainer;
			// set timeout to infinite
			_layerContainer.Timeout = -1;
            _layerContainer.Connect();
			// set the config service address for the layerContainer. It might have been overriden by a 
			// parameter during startup through the MARSLocalStarter
			_layerContainer.ServiceProxy.SetMarsConfigServiceAddress(MARSConfigServiceSettings.Address);
            _layerContainer.ServiceProxy.LoadModelContent(content);
        }

        public void Instantiate(TLayerInstanceId instanceId) {
            _layerContainer.ServiceProxy.Instantiate(instanceId);
        }

        public long Tick() {
            return _layerContainer.ServiceProxy.Tick();
        }

		public void CleanUp(){
			_layerContainer.ServiceProxy.CleanUp ();
		}

        public ILayerContainer Proxy {
            get { return _layerContainer.ServiceProxy; }
        }

        public void Initialize(TLayerInstanceId layerInstanceId, TInitData initData)
        {
            if(!Proxy.InitializeLayer(layerInstanceId, initData)){
                throw new LayerFailedToInitializeException("Layer " 
                    + layerInstanceId.LayerDescription.Name 
                    + " failed to initialize. Please review your InitLayer Code! MARS LIFE will shut down now."
                    );
            }
        }
    }

}