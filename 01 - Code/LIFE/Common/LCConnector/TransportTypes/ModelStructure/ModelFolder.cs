using System;
using System.Collections.Generic;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    /// A folder within a model directory.
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

            var files = Directory.EnumerateFileSystemEntries(name);
            foreach (var file in files) {
                FileAttributes attributes = File.GetAttributes(file);
                if (attributes == FileAttributes.Directory) {
                    Contents.Add(new ModelFolder(name + file));
                }
                else {
                    Contents.Add(new ModelFile(name + file));
                }
            }
        }

        public ModelFolder() {
            Contents = new List<IModelDirectoryContent>();
        }
    }
}