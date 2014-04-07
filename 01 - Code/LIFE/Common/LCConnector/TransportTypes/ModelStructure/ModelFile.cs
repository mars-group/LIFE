using System;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    /// A file
    /// </summary>
    [Serializable]
    internal class ModelFile : IModelDirectoryContent {
        public string Name { get; set; }

        public ContentType Type {
            get { return ContentType.File; }
        }

        public byte[] Content { get; set; }

        public ModelFile(string name) {
            Name = name;
            Content = File.ReadAllBytes(name);
        }

        public ModelFile() {}
    }
}