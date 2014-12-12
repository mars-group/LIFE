using System;
using System.Collections.Generic;
using LayerAPI.Interfaces;

namespace LayerAPI.Config
{
    [Serializable]
    public class ModelConfig
    {
        public IDictionary<string, LayerConfig> LayerConfigs { get; set; }

        public ModelConfig(IDictionary<string, LayerConfig> layerConfigs)
        {
            LayerConfigs = layerConfigs;
        }

        public ModelConfig() {
            LayerConfigs = new Dictionary<string, LayerConfig>();
        }

        public void AddLayerConfig(string layerName, LayerConfig layerConfig)
        {
            LayerConfigs.Add(new KeyValuePair<string, LayerConfig>(layerName, layerConfig));
        }
    }
}
