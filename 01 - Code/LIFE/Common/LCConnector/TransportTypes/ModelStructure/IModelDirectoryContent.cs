namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    /// the interface of an item that can occur in a model folder.
    /// </summary>
    internal interface IModelDirectoryContent {
        /// <summary>
        /// The item's filename in the fileystem.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Is it a file or a folder?
        /// </summary>
        ContentType Type { get; }
    }
}