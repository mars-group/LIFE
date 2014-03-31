using Autofac;
using LayerContainerController.Implementation;
using LayerContainerController.Interfaces;
using LayerContainerFacade.Implementation;
using LayerFactory.Implementation;
using LayerFactory.Interface;
using LayerRegistry.Implementation;
using LayerRegistry.Interfaces;
using PartitionManager.Implementation;
using PartitionManager.Interfaces;
using RTEManager.Implementation;
using RTEManager.Interfaces;

namespace LayerContainerFacade.Interfaces
{
    public static class ApplicationCoreFactory
    {
        private static IContainer _container;
        private static ContainerBuilder _containerBuilder;

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

                _containerBuilder.RegisterType<LayerFactoryComponent>()
                    .As<ILayerFactory>()
                    .InstancePerLifetimeScope();

                _containerBuilder.RegisterType<LayerContainerFacadeImpl>()
                    .As<ILayerContainerFacade>()
                    .InstancePerDependency();
            }

            return _container.Resolve<ILayerContainerFacade>();
        }
    }
}
