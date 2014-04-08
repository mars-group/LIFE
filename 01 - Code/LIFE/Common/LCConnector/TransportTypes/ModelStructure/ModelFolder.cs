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
            Name = name;

            IEnumerable<string> files = Directory.EnumerateFileSystemEntries(name);

            foreach (var file in files) {
                FileAttributes attributes = File.GetAttributes(file);
                string[] parts = file.Split(Path.DirectorySeparatorChar);
                if (attributes == FileAttributes.Directory) Contents.Add(new ModelFolder(parts[parts.Length - 1]));
                else Contents.Add(new ModelFile(file.Replace(name, parts[parts.Length - 1])));
            }
        }

        public ModelFolder() {
            Contents = new List<IModelDirectoryContent>();
        }
    }
}