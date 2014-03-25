using System;
using System.Collections.Generic;
using System.Configuration;
using NChordLib;


namespace LayerRegistry.Implementation
{

    using LayerAPI.Interfaces;

    using Interfaces;

    /// <summary>
    /// TODO: Use http://nchord.sourceforge.net/ and store information in DHT
    /// </summary>
    class LayerRegistryUseCase : ILayerRegistry
    {

        public LayerRegistryUseCase()
        {
            ChordServer.LocalNode = new ChordNode(
                System.Net.Dns.GetHostName(),
                int.Parse(ConfigurationManager.AppSettings.Get("NChordPort"))
                );
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
    }
}
