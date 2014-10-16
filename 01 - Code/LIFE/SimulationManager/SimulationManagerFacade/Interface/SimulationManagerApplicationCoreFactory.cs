using AppSettingsManager;
using Autofac;
using ConfigurationAdapter.Interface;
using ModelContainer.Implementation;
using ModelContainer.Interfaces;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using RuntimeEnvironment.Implementation;
using RuntimeEnvironment.Interfaces;
using SimulationManagerFacade.Implementation;
using SimulationManagerShared;

namespace SimulationManagerFacade.Interface {
    /// <summary>
    ///     This class can be used to obtain all versions of the application core, either for testing or
    ///     for production use. It works via the factory pattern.
    /// </summary>
    public class SimulationManagerApplicationCoreFactory {
        private static IContainer container;

        public static ISimulationManagerApplicationCore GetProductionApplicationCore() {
            if (container == null) {
                ContainerBuilder builder = new ContainerBuilder();

                builder.RegisterType<ModelContainerComponent>()
                    .As<IModelContainer>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<RuntimeEnvironmentComponent>()
                    .As<IRuntimeEnvironment>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<NodeRegistryComponent>()
                    .As<INodeRegistry>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<MulticastAdapterComponent>()
                    .As<IMulticastAdapter>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<SimulationManagerApplicationCoreComponent>()
                    .As<ISimulationManagerApplicationCore>()
                    .InstancePerDependency();

                // Make the configurations available for all components
                var globalConfig = Configuration.Load<GlobalConfig>();
                builder.RegisterInstance(globalConfig);

                var localConfig = Configuration.Load<SimulationManagerSettings>();
                builder.RegisterInstance(localConfig);
                builder.RegisterInstance(localConfig.NodeRegistryConfig);
                builder.RegisterInstance(localConfig.MulticastSenderConfig);
                
                container = builder.Build();
            }

            return container.Resolve<ISimulationManagerApplicationCore>();
        }
    }
}