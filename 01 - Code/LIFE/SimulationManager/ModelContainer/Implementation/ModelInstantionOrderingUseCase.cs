using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConfigurationAdapter.Interface;
using LCConnector.TransportTypes;
using log4net;
using Mono.Addins;
using Shared;
using SMConnector.TransportTypes;

namespace ModelContainer.Implementation
{
    /// <summary>
    /// This class reads one specific model and converts it into a manner representation that allows<br/>
    /// for instantiation order analysis.
    /// </summary>
    internal class ModelInstantionOrderingUseCase {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ModelInstantionOrderingUseCase));

        private Configuration<SimulationManagerSettings> _settings;

        public ModelInstantionOrderingUseCase(Configuration<SimulationManagerSettings> settings) {
            _settings = settings;

            Logger.Debug("instantiated.");
        }

        TModelStructure InitFromModel(TModelDescription description) {
            //AddinManager.
        }
    }

    internal class TModelStructure : IEnumerable<TLayerDescription> {


        public IEnumerator<TLayerDescription> GetEnumerator() {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
