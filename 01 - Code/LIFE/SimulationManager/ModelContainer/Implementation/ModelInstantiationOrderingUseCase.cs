// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LCConnector.TransportTypes;
using LifeAPI.AddinLoader;
using log4net;
using ModelContainer.Implementation.Entities;
using Mono.Addins;
using SimulationManagerShared;
using SMConnector.TransportTypes;

[assembly: AddinRoot("LayerContainer", "0.1")]

namespace ModelContainer.Implementation {
    /// <summary>
    ///     This class reads one specific model and converts it into a representation that allows<br />
    ///     for instantiation order analysis.
    /// </summary>
    internal class ModelInstantiationOrderingUseCase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ModelInstantiationOrderingUseCase));

        private SimulationManagerSettings _settings;

        public ModelInstantiationOrderingUseCase(SimulationManagerSettings settings) {
            _settings = settings;

            Logger.Debug("instantiated.");
        }

        public IList<TLayerDescription> GetInstantiationOrder(TModelDescription description) {
            // Delete the addinDB
            if (Directory.Exists(_settings.AddinLibraryDirectoryPath + Path.DirectorySeparatorChar + "addin-db-001")) {
                Directory.Delete
                    (_settings.AddinLibraryDirectoryPath + Path.DirectorySeparatorChar + "addin-db-001", true);
            }
            if (Directory.Exists(_settings.AddinLibraryDirectoryPath + Path.DirectorySeparatorChar + "addin-db-002"))
            {
                Directory.Delete
                    (_settings.AddinLibraryDirectoryPath + Path.DirectorySeparatorChar + "addin-db-002", true);
            }


            // use AddinLoader from LIFEApi, because Mono.Addins may only load Plugins whose 
            // Interfaces originate from the Assembly they are attempted to be loaded from
            IAddinLoader addinLoader = AddinLoader.Instance;

            ExtensionNodeList nodes = addinLoader.LoadAllLayers(description.Name);

            ModelStructure modelStructure = new ModelStructure();

            foreach (TypeExtensionNode node in nodes) {
                Type type = node.Type;
                ConstructorInfo[] constructors = type.GetConstructors();
                // make sure all parameters in all constructors are Interfaces, throw exception otherwise
                if (
                    constructors.Any(
                        info => info.GetParameters().Any(parameterInfo => !parameterInfo.ParameterType.IsInterface)))
                {
                    throw new AllLayerConstructorParamtersNeedToBeInterfacesException(
                        "Make sure all your parameters in your Layer's constructor are interface types." +
                        "This is required for MARS LIFE's distribution to work properly."
                        );
                }
                TLayerDescription layerDescription = new TLayerDescription
                    (type.Name,
                        type.Assembly.GetName().Version.Major,
                        type.Assembly.GetName().Version.Minor,
                        type.Assembly.Location,
                        type.FullName,
                        type.AssemblyQualifiedName);

                if (constructors.Any(c => c.GetParameters().Length == 0))
                    modelStructure.AddLayer(layerDescription, type);
                else {
                    ParameterInfo[] paramList = constructors.First(c => c.GetParameters().Length > 0).GetParameters();
                    modelStructure.AddLayer(layerDescription, type, paramList.Select(p => p.ParameterType).ToArray());
                }
            }

            return modelStructure.CalculateInstantiationOrder();
        }
    }

    [Serializable]
    internal class AllLayerConstructorParamtersNeedToBeInterfacesException : Exception
    {
        public AllLayerConstructorParamtersNeedToBeInterfacesException(string s) : base(s)
        {
            
        }
    }
}