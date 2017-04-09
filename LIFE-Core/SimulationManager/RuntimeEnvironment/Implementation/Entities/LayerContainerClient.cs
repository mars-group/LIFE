//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using CommonTypes;
using Hik.Communication.ScsServices.Client;
using LCConnector;
using LCConnector.TransportTypes.ModelStructure;
using LIFE.API.Layer.Initialization;
using RuntimeEnvironment.Interfaces.Exceptions;

namespace RuntimeEnvironment.Implementation.Entities
{
    internal class LayerContainerClient
    {
        private readonly IScsServiceClient<ILayerContainer> _layerContainer;

        private readonly ILayerContainer _proxy;

        public LayerContainerClient(ILayerContainer inMemorylayerContainer, ModelContent content, string resultConfigId)
        {
            _proxy = inMemorylayerContainer;
            _proxy.LoadModelContent(content);
            _proxy.Initialize(resultConfigId);
        }

        public LayerContainerClient(IScsServiceClient<ILayerContainer> layerContainer, ModelContent content,
            string resultConfigId)
        {
            _layerContainer = layerContainer;
            // set timeout to infinite
            _layerContainer.Timeout = -1;
            _layerContainer.Connect();
            // set the config service address for the layerContainer. It might have been overriden by a
            // parameter during startup through the MARSLocalStarter
            _layerContainer.ServiceProxy.SetMarsConfigServiceAddress(MARSConfigServiceSettings.Address);
            _proxy = _layerContainer.ServiceProxy;
            _proxy.LoadModelContent(content);
            _proxy.Initialize(resultConfigId);
        }

        public void Instantiate(TLayerInstanceId instanceId)
        {
            _proxy.Instantiate(instanceId);
        }

        public long Tick(long amountOfTicks = 1)
        {
            return _proxy.Tick(amountOfTicks);
        }

        public void CleanUp()
        {
            _proxy.CleanUp();
        }

        public ILayerContainer Proxy
        {
            get { return _proxy; }
        }

        public void Initialize(TLayerInstanceId layerInstanceId, TInitData initData)
        {
            if (!Proxy.InitializeLayer(layerInstanceId, initData))
            {
                throw new LayerFailedToInitializeException("Layer "
                                                           + layerInstanceId.LayerDescription.Name
                                                           +
                                                           " failed to initialize. Please review your InitLayer Code! MARS LIFE will shut down now."
                );
            }
        }
    }
}