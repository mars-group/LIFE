using System.IO;
using System.Linq;
using ConfigurationAdapter.Interface;
using LayerAPI.AddinLoader;
using LayerAPI.Interfaces;
using LCConnector.TransportTypes;
using log4net;
using ModelContainer.Implementation.Entities;
using Mono.Addins;
using Shared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation {
    /// <summary>
    ///     This class reads one specific model and converts it into a representation that allows<br />
    ///     for instantiation order analysis.
    /// </summary>
    internal class ModelInstantionOrderingUseCase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ModelInstantionOrderingUseCase));

        private Configuration<SimulationManagerSettings> _settings;
        private AddinLoader _addinLoader;

        public ModelInstantionOrderingUseCase(Configuration<SimulationManagerSettings> settings) {
            _settings = settings;
            _addinLoader = new AddinLoader();

            Logger.Debug("instantiated.");
        }

        private ModelStructure InitFromModel(TModelDescription description) {
            AddinManager.Initialize("./addinConfig", _settings.Content.ModelDirectoryPath + Path.DirectorySeparatorChar + description.Name);
            AddinManager.Registry.Update();
            var nodes = AddinManager.GetExtensionNodes(typeof (ILayer));

            var modelStructure = new ModelStructure();

            foreach (var node in nodes) {
                var type = node.GetType();
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

            return modelStructure;
        }
    }
}