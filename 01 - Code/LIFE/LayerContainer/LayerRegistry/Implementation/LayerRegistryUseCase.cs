using System;
using System.Collections.Generic;
using System.Configuration;


namespace LayerRegistry.Implementation
{

    using LayerAPI.Interfaces;

    using Interfaces;

    /// <summary>
    /// TODO: Use http://nchord.sourceforge.net/ and store information in DHT
    /// </summary>
    class LayerRegistryUseCase : ILayerRegistry
    {
        private readonly int _chordPort;
        private readonly string _chordSeedHost;
        private readonly int _chordSeedPort;

        public LayerRegistryUseCase()
        {
            _chordPort = int.Parse(ConfigurationManager.AppSettings.Get("NChordPort"));
            _chordSeedPort = int.Parse(ConfigurationManager.AppSettings.Get("NChordSeedPort"));
            _chordSeedHost = ConfigurationManager.AppSettings.Get("NChordSeedHost");
        }

        public ILayer RemoveLayerInstance(Guid layerID)
        {
            throw new NotImplementedException();
        }

        public void ResetLayerRegistry()
        {
            throw new NotImplementedException();
        }

        public ILayer GetLayerInstance(Type parameterType)
        {
            throw new NotImplementedException();
        }

        public void RegisterLayer(ILayer layer)
        {
            throw new NotImplementedException();
        }

        #region Private Methods

        private void JoinChordRing()
        {

        }

        #endregion
    }
}
