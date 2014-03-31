using LayerContainerController.Implementation;
using LayerContainerController.Interfaces;
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
            }

            return _container.Resolve<ILayerContainerFacade>();
        }
    }
}
