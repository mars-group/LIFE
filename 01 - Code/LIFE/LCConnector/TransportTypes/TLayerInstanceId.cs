namespace LCConnector.TransportTypes
{
    public class TLayerInstanceId
    {
        /// <summary>
        /// The layer's identity.
        /// </summary>
        public TLayerId LayerId { get; protected set; }

        /// <summary>
        /// The instance's unique number.
        /// </summary>
        public int InstanceNr { get; protected set; }
    }
}
