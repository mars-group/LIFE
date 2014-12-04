// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using Autofac;
using ConfigurationAdapter.Interface;
using DistributedKeyValueStore.Implementation;
using DistributedKeyValueStore.Interface;
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
using RTEManager.Implementation;
using RTEManager.Interfaces;
using VisualizationAdapter.Implementation;
using VisualizationAdapter.Interface;

namespace LayerContainerFacade.Interfaces {
    public static class LayerContainerApplicationCoreFactory {
        private static IContainer _container;
        private static ContainerBuilder _containerBuilder;

        public static ILayerContainerFacade GetLayerContainerFacade() {
            if (_container == null) {
                if (_containerBuilder == null) _containerBuilder = new ContainerBuilder();

                _containerBuilder.RegisterType<DistributedKeyValueStoreComponent>()
                    .As<IDistributedKeyValueStore>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<NodeRegistryComponent>()
                    .As<INodeRegistry>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<MulticastAdapterComponent>()
                    .As<IMulticastAdapter>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<LayerRegistryComponent>()
                    .As<ILayerRegistry>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<PartitionManagerComponent>()
                    .As<IPartitionManager>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<RTEManagerComponent>()
                    .As<IRTEManager>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<VisualizationAdapterComponent>()
                    .As<IVisualizationAdapterInternal>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<LayerFactoryComponent>()
                    .As<ILayerFactory>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<LayerContainerFacadeImpl>()
                    .As<ILayerContainerFacade>()
                    .InstancePerDependency();

                // Make the configuration file available for all components
                LayerContainerSettings config = Configuration.Load<LayerContainerSettings>();
                _containerBuilder.RegisterInstance(config);
                _containerBuilder.RegisterInstance(config.NodeRegistryConfig);
                _containerBuilder.RegisterInstance(config.LayerRegistryConfig);
                _containerBuilder.RegisterInstance(config.GlobalConfig);
                _containerBuilder.RegisterInstance(config.MulticastSenderConfig);

                _container = _containerBuilder.Build();
            }

            return _container.Resolve<ILayerContainerFacade>();
        }
    }
}