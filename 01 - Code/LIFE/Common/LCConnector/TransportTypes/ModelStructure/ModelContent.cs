using System;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     This class holds the content of a model and is able to write it to a stream.
    /// </summary>
    [Serializable]
    public class ModelContent {
        private ModelFolder _root;

        public ModelContent(string modelPath) {
            _root = new ModelFolder(modelPath);
        }

        public ModelContent() {}

        /// <summary>
        /// Writes the complete content of the model folder into the target directory.<br/>
        /// If any file or directory already existed, it will be overwritten.
        /// </summary>
        /// <param name="targetDirectory"></param>
        public void Write(string targetDirectory) {
            foreach (var modelDirectoryContent in _root.Contents) {
                Write(modelDirectoryContent, targetDirectory);
            }
        }

        private void Write(IModelDirectoryContent dirContent, string path) {
            if (dirContent.Type == ContentType.File) {
                ModelFile file = dirContent as ModelFile;
                FileStream stream = File.Open(path + Path.DirectorySeparatorChar + file.Name, FileMode.Create);
                stream.WriteAsync(file.Content, 0, file.Content.Length);
            }
            else {
                ModelFolder folder = dirContent as ModelFolder;
                if (!Directory.Exists(path + Path.DirectorySeparatorChar + dirContent.Name))
                {
                    Directory.CreateDirectory(path + Path.DirectorySeparatorChar + dirContent.Name);
                }

                foreach (var content in folder.Contents) {
                    Write(content, path + Path.DirectorySeparatorChar + dirContent.Name);
                }
            }
        }
    }
}