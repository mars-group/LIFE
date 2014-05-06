using System;
using System.Collections.Generic;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     A folder within a model directory.
    /// </summary>
    [Serializable]
    internal class ModelFolder : IModelDirectoryContent {
        public string Name { get; set; }

        public ContentType Type {
            get { return ContentType.Folder; }
        }

        public IList<IModelDirectoryContent> Contents { get; set; }

        public ModelFolder(string name) : this() {
            string[] parts = name.Split(Path.DirectorySeparatorChar);
            Name = parts[parts.Length - 1];

            IEnumerable<string> files = Directory.EnumerateFileSystemEntries(name);

            foreach (var file in files) {
                FileAttributes attributes = File.GetAttributes(file);
                
                if (attributes == FileAttributes.Directory) Contents.Add(new ModelFolder(file));
                else Contents.Add(new ModelFile(file));
            }
        }

        public ModelFolder() {
            Contents = new List<IModelDirectoryContent>();
        }
    }
}