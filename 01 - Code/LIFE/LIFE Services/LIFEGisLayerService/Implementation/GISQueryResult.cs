using System;
using System.Collections.Generic;
using LifeAPI.Layer.GIS;

namespace LIFEGisLayerService.Implementation
{
    [Serializable]
    public class GISQueryResult : IGISQueryResult
    {
        public List<GISResultEntry> ResultEntries { get; private set; }

        public GISQueryResult(List<GISResultEntry> entries) {
            ResultEntries = entries;
        }
    }
}
