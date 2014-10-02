﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using LayerAPI.AddinLoader;
using LCConnector.TransportTypes;
using log4net;
using ModelContainer.Implementation.Entities;
using Mono.Addins;

using SMConnector.TransportTypes;
using SimulationManagerShared;
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
			Directory.Delete (_settings.AddinLibraryDirectoryPath + Path.DirectorySeparatorChar + "addin-db-001", true);

            // use AddinLoader from LIFEApi, because Mono.Addins may only load Plugins whose 
            // Interfaces originate from the Assembly they are attempted to be loaded from
            IAddinLoader addinLoader = new AddinLoader
                (_settings.AddinLibraryDirectoryPath,
                "." + Path.DirectorySeparatorChar + "addins" + Path.DirectorySeparatorChar + description.Name);

            addinLoader.UpdateAddinRegistry();

            var nodes = addinLoader.LoadAllLayers();
            var modelStructure = new ModelStructure();

            foreach (TypeExtensionNode node in nodes) {
                var type = node.Type;
                var constructors = type.GetConstructors();
                var layerDescription = new TLayerDescription(type.Name, type.Assembly.GetName().Version.Major,
                    type.Assembly.GetName().Version.Minor, type.Assembly.Location);

                if (constructors.Any(c => c.GetParameters().Length == 0))
                    modelStructure.AddLayer(layerDescription, type);
                else {
                    var paramList = constructors.First(c => c.GetParameters().Length > 0).GetParameters();
                    modelStructure.AddLayer(layerDescription, type, paramList.Select(p => p.ParameterType).ToArray());
                }
            }

            return modelStructure.CalculateInstantiationOrder();
        }
    }
}