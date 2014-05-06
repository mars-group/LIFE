
using System;

namespace SMConnector.TransportTypes
{
    [Serializable]
    public class TStatusUpdate
    {
        public string StatusMessage { get; set; }

        public TStatusUpdate(string msg) {
            StatusMessage = msg;
        }
    }
}
