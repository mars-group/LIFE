﻿using System;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     This class holds the content of a model and is able to write it to a stream.
    /// </summary>
    [Serializable]
    public class ModelContent {
        private readonly ModelFolder _root;

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
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            EmptyDirectory(targetDirectory);
            foreach (var modelDirectoryContent in _root.Contents) {
                Write(modelDirectoryContent, targetDirectory);
            }
        }

        private static void EmptyDirectory(string targetDirectory) {


            var dirInfo = new DirectoryInfo(targetDirectory);

            foreach (var file in dirInfo.GetFiles())
            {
                file.Delete();
            }
            foreach (var dir in dirInfo.GetDirectories())
            {
                dir.Delete(true);
            }


        }

        private static void Write(IModelDirectoryContent dirContent, string path) {
            if (dirContent.Type == ContentType.File) {
                var file = dirContent as ModelFile;
                var stream = File.Open(path + Path.DirectorySeparatorChar + file.Name, FileMode.Create);
                stream.Write(file.Content, 0, file.Content.Length);
            }
            else {
                var folder = dirContent as ModelFolder;
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