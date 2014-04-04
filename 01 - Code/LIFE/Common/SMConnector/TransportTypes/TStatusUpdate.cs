
namespace SMConnector.TransportTypes
{
    public class TStatusUpdate
    {
        public string StatusMessage { get; set; }

        public TStatusUpdate(string msg) {
            StatusMessage = msg;
        }
    }
}
