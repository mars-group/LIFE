﻿using Autofac;
using LayerContainerController.Implementation;
using LayerContainerController.Interfaces;
using LayerContainerFacade.Implementation;
using LayerContainerFacade.Interfaces;
using LayerFactory.Implementation;
using LayerFactory.Interface;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using PartitionManager.Implementation;
using PartitionManager.Interfaces;
using RTEManager.Implementation;
using RTEManager.Interfaces;

namespace LayerContainerFacade
{
    public static class ApplicationCoreFactory
    {
        private static IContainer _container;
        private static ContainerBuilder _containerBuilder;

        /// <summary>
        /// Returns a new instance of a LayerContainerFacade.
        /// Containing all depenencies.
        /// </summary>
        /// <returns></returns>
        public static ILayerContainerFacade GetLayerContainerFacade()
        {
            if (_container == null)
            {
                if (_containerBuilder == null)
                {
                    _containerBuilder = new ContainerBuilder();
                }

                _containerBuilder.RegisterType<LayerContainerControllerComponent>()
                    .As<ILayerContainerController>()
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

                _containerBuilder.RegisterType<LayerContainerFacadeImplementation>()
                    .As<ILayerContainerFacade>()
                    .InstancePerDependency();

                _containerBuilder.RegisterType<LayerFactoryComponent>()
                    .As<ILayerFactory>()
                    .InstancePerLifetimeScope();


                _container = _containerBuilder.Build();
            }


            return _container.Resolve<ILayerContainerFacade>();
        }
    }
}