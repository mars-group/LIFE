using System;
using System.Collections.Generic;

namespace LifeAPI.Config
{
    [Serializable]
    public class ModelConfig
    {
        public List<LayerConfig> LayerConfigs { get; set; }

        public ModelConfig(List<LayerConfig> layerConfigs)
        {
            LayerConfigs = layerConfigs;
        }

        public ModelConfig() {
            LayerConfigs = new List<LayerConfig>();
        }

        public void AddLayerConfig(LayerConfig layerConfig)
        {
            LayerConfigs.Add(layerConfig);
        }
    }
}
