using System;
using System.Collections.Generic;

namespace LifeAPI.Layer.GIS.ResultTypes
{
    [Serializable]
    public class GISQueryResult
    {
        public List<GISResultEntry> ResultEntries { get; private set; }

        public GISQueryResult(List<GISResultEntry> entries) {
            ResultEntries = entries;
        }
    }
}
