using System;

namespace LIFEGisLayerService.Implementation
{
    [Serializable]
    public class GISResultEntry
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public object Value { get; set; }
    }
}
