using Autofac;
using LayerContainerController.Implementation;
using LayerContainerController.Interfaces;
using LayerContainerFacade.Implementation;
using LayerContainerFacade.Interfaces;
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
                    .InstancePerDependency();

                _containerBuilder.RegisterType<LayerRegistryComponent>()
                    .As<ILayerRegistry>()
                    .InstancePerDependency();

                _containerBuilder.RegisterType<PartitionManagerComponent>()
                    .As<IPartitionManager>()
                    .InstancePerDependency();

                _containerBuilder.RegisterType<RTEManagerComponent>()
                    .As<IRTEManager>()
                    .InstancePerDependency();

                _containerBuilder.RegisterType<LayerContainerFacadeImplementation>()
                    .As<ILayerContainerFacade>()
                    .InstancePerDependency();
            }

            _container = _containerBuilder.Build();

            return _container.Resolve<ILayerContainerFacade>();
        }
    }
}
