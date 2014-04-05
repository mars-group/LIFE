namespace LCConnector.TransportTypes {
    public class TLayerId {
        /// <summary>
        /// The layer's name.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// In a version xx.yyyyyy, this is the xx.
        /// </summary>
        public int MajorVersion { get; protected set; }
        
        /// <summary>
        /// /// In a version xx.yyyyyy, this is the yyyyyy.
        /// </summary>
        public int MinorVersion { get; protected set; }

        /// <summary>
        /// The filename of the original dll holding the binary code.
        /// </summary>
        public string FileName { get; protected set; }
    }
}