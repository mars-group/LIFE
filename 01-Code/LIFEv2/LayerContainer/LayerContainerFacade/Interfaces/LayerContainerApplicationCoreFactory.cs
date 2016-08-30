//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System;
using Autofac;
using ConfigurationAdapter;
using ConfigurationAdapter.Interface;
using LayerContainerFacade.Implementation;
using LayerContainerShared;
using LayerFactory.Implementation;
using LayerFactory.Interface;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using PartitionManager.Implementation;
using PartitionManager.Interfaces;
using ResultAdapter.Implementation;
using ResultAdapter.Interface;
using RTEManager.Implementation;
using RTEManager.Interfaces;

namespace LayerContainerFacade.Interfaces {
    /// <summary>
    /// The LayerContainerCore factory constructs a layer container instance 
    /// via an IoC container and provides a reference.
    /// </summary>
    public static class LayerContainerApplicationCoreFactory {
        private static IContainer _container;
        /// <summary>
        /// Instantiates a LayercontainerFacade instance and returns the reference. Use this to
        /// retreive a new LayerContainer or to reset an old one.
        /// </summary>
        /// <returns>A reference to an ILayerContainerFacade instance.</returns>
        public static ILayerContainerFacade GetLayerContainerFacade(string clusterName = null) {
            if (_container != null) return _container.Resolve<ILayerContainerFacade>();
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterType<NodeRegistryComponent>()
                .As<INodeRegistry>()
                .InstancePerLifetimeScope()
                .WithParameter(new TypedParameter(typeof(string), clusterName));

            containerBuilder.RegisterType<MulticastAdapterComponent>()
                .As<IMulticastAdapter>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<LayerRegistryComponent>()
                .As<ILayerRegistry>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<PartitionManagerComponent>()
                .As<IPartitionManager>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<RTEManagerComponent>()
                .As<IRTEManager>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<ResultAdapterComponent>()
                .As<IResultAdapter>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<LayerFactoryComponent>()
                .As<ILayerFactory>()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterType<LayerContainerFacadeImpl>()
                .As<ILayerContainerFacade>()
                .InstancePerDependency();

            // Make the configuration file available for all components

            LayerContainerSettings config = Configuration.Load<LayerContainerSettings>();
            containerBuilder.RegisterInstance(config);
            containerBuilder.RegisterInstance(config.NodeRegistryConfig);
            containerBuilder.RegisterInstance(config.GlobalConfig);
            containerBuilder.RegisterInstance(config.MulticastSenderConfig);

            _container = containerBuilder.Build();


            var cont = _container.Resolve<ILayerContainerFacade>();
            Console.WriteLine("Done creating LayerContainer");
            return cont;
        }
    }
}