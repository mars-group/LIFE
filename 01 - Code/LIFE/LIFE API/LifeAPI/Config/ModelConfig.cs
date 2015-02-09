using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LifeAPI.Config
{
    /// <summary>
    /// A MARS LIFE model configuration. 
    /// </summary>
    [Serializable]
    public class ModelConfig
    {
        /// <summary>
        /// The model's layer configurations
        /// </summary>
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
        /// <summary>
        /// The time one tick spans.
        /// </summary>
        [XmlElement("OneTickTimeSpan")]
        public string OneTickTimeSpanTicks
        {
            get { return _oneTickTimeSpan.ToString(); }
            set { _oneTickTimeSpan = TimeSpan.Parse(value); }
        }


        /// <summary>
        /// Creates new ModelConfig with a 1 sec timespan.
        /// </summary>
        /// <param name="layerConfigs"></param>
        public ModelConfig(List<LayerConfig> layerConfigs)
        {
            _oneTickTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
            LayerConfigs = layerConfigs;
        }

        /// <summary>
        /// Creates a new ModelConfig with a 1 sec timespan 
        /// and empty LayerConfigs.
        /// </summary>
        public ModelConfig() {
            _oneTickTimeSpan = new TimeSpan(0, 0, 0, 1, 0);
            LayerConfigs = new List<LayerConfig>();
        }

        /// <summary>
        /// Adds a LayerConfig to this ModelConfig
        /// </summary>
        /// <param name="layerConfig"></param>
        public void AddLayerConfig(LayerConfig layerConfig)
        {
            LayerConfigs.Add(layerConfig);
        }
    }
}
