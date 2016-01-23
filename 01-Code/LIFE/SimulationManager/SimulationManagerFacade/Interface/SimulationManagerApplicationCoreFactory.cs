// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/


using AppSettingsManager;
using Autofac;
using ConfigurationAdapter.Interface;
using LayerNameService.Implementation;
using LNSConnector.Interface;
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

                builder.RegisterType<LayerNameServiceComponent>()
                    .As<ILayerNameService>()
                    .InstancePerLifetimeScope();

                builder.RegisterType<SimulationManagerApplicationCoreComponent>()
                    .As<ISimulationManagerApplicationCore>()
                    .InstancePerDependency();

                // Make the configurations available for all components
                GlobalConfig globalConfig = Configuration.Load<GlobalConfig>();
                builder.RegisterInstance(globalConfig);

                SimulationManagerSettings localConfig = Configuration.Load<SimulationManagerSettings>();
                builder.RegisterInstance(localConfig);
                builder.RegisterInstance(localConfig.NodeRegistryConfig);
                builder.RegisterInstance(localConfig.MulticastSenderConfig);

                container = builder.Build();
            }

            return container.Resolve<ISimulationManagerApplicationCore>();
        }
    }
}