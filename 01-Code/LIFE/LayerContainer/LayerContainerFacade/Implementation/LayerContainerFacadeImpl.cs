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
using System.Reflection;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using LayerContainerFacade.Interfaces;
using LayerContainerShared;
using LCConnector;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using LIFEViewProtocol.Basics;
using PartitionManager.Interfaces;
using RTEManager.Interfaces;
using VisualizationAdapter.Interface;

namespace LayerContainerFacade.Implementation {
	internal class LayerContainerFacadeImpl : ScsService, ILayerContainerFacade {
        private readonly IPartitionManager _partitionManager;
        private readonly IRTEManager _rteManager;
        private IScsServiceApplication _server;



        public LayerContainerFacadeImpl
            (LayerContainerSettings settings,
                IPartitionManager partitionManager,
                IRTEManager rteManager,
                IVisualizationAdapter visualizationAdapter) {

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolverFix.HandleAssemblyResolve;
            _partitionManager = partitionManager;
            _rteManager = rteManager;
            _server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(settings.NodeRegistryConfig.NodeEndPointPort));

            _server.AddService<ILayerContainer, LayerContainerFacadeImpl>(this);

            //Start server
            _server.Start();
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

        public long Tick() {
            return _rteManager.AdvanceOneTick();
        }

		public void CleanUp ()
		{
			_rteManager.DisposeSuitableLayers ();
		}

        #endregion



    }

    public static class AssemblyResolverFix
    {
        //Looks up the assembly in the set of currently loaded assemblies,
        //and returns it if the name matches. Else returns null.
        public static Assembly HandleAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(ass => ass.FullName == args.Name);
        }
    }
}