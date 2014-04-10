﻿using Autofac;
using ConfigurationAdapter.Interface;
using ModelContainer.Implementation;
using ModelContainer.Interfaces;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using RuntimeEnvironment.Implementation;
using RuntimeEnvironment.Interfaces;
using Shared;
using SimulationManagerFacade.Implementation;

namespace SimulationManagerFacade.Interface {
    /// <summary>
    ///     This class can be used to obtain all versions of the application core, either for testing or
    ///     for production use. It works via the factory pattern.
    /// </summary>
    public class ApplicationCoreFactory {
        private static IContainer container;

        public static IApplicationCore GetProductionApplicationCore() {
            if (container == null) {
                ContainerBuilder builder = new ContainerBuilder();

                builder.RegisterType<ModelContainerComponent>()
                    .As<IModelContainer>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<RuntimeEnvironmentComponent>()
                    .As<IRuntimeEnvironment>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<NodeRegistryUseCase>()
                    .As<INodeRegistry>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<ApplicationCoreComponent>()
                    .As<IApplicationCore>()
                    .InstancePerDependency();

                // Make the configuration file available for all components
                builder.RegisterInstance(new Configuration<SimulationManagerSettings>("config.xml"));

                container = builder.Build();
            }

            return container.Resolve<IApplicationCore>();
        }
    }
}