using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LifeAPI.Config
{
    [Serializable]
    public class ModelConfig
    {
        public List<LayerConfig> LayerConfigs { get; set; }

        private TimeSpan _oneTickTimeSpan { get; set; }

        // Public Property - XmlIgnore as it doesn't serialize anyway
        [XmlIgnore]
        public TimeSpan OneTickTimeSpan
        {
            get { return _oneTickTimeSpan; }
            set { _oneTickTimeSpan = value; }
        }

        // Pretend property for serialization
        [XmlElement("OneTickTimeSpan")]
        public string OneTickTimeSpanTicks
        {
            get { return _oneTickTimeSpan.ToString(); }
            set { _oneTickTimeSpan = TimeSpan.Parse(value); }
        }


        public ModelConfig(List<LayerConfig> layerConfigs)
        {
            _oneTickTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
            LayerConfigs = layerConfigs;
        }

        public ModelConfig() {
            _oneTickTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
            LayerConfigs = new List<LayerConfig>();
        }

        public void AddLayerConfig(LayerConfig layerConfig)
        {
            LayerConfigs.Add(layerConfig);
        }
    }
}
