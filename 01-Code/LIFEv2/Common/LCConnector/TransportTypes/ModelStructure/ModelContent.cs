//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.IO;
using System.Threading;


namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     This class holds the content of a model and is able to write it to a stream.
    /// </summary>
    public class ModelContent {
        private readonly ModelFolder _root;

        public ModelContent(string modelPath) {
            _root = new ModelFolder(modelPath);
        }

        public ModelContent() {}

        /// <summary>
        ///     Writes the complete content of the model folder into the target directory.<br />
        ///     If any file or directory already existed, it will be overwritten.
        /// </summary>
        /// <param name="targetDirectory"></param>
        public void Write(string targetDirectory) {
            if (!Directory.Exists(targetDirectory)) Directory.CreateDirectory(targetDirectory);
            //EmptyDirectory(targetDirectory);
            foreach (var modelDirectoryContent in _root.Contents) {
                Write(modelDirectoryContent, targetDirectory);
            }
        }

        private static void EmptyDirectory(string targetDirectory) {
            DirectoryInfo dirInfo = new DirectoryInfo(targetDirectory);

            foreach (FileInfo file in dirInfo.GetFiles()) {
                WaitForFile(file.FullName);
                //    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", file.FullName));
                file.Delete();
            }

            foreach (DirectoryInfo dir in dirInfo.GetDirectories()) {
                WaitForFile(dir.FullName);
                //    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", dir.FullName));
                dir.Delete(true);
            }
        }

        /// <summary>
        ///     Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        private static bool WaitForFile(string fullPath, int retryCount = 10, int sleep = 500) {
            int numTries = 0;
            while (numTries++ < retryCount) {
                try {
                    // Attempt to open the file exclusively.
                    using (FileStream fs = new FileStream
                        (fullPath,
                            FileMode.Open,
                            FileAccess.ReadWrite,
                            FileShare.None,
                            100)) {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        return true;
                    }
                }
                catch (Exception) {
                    // Wait for the lock to be released
                    Thread.Sleep(sleep);
                }
            }
            return false;
        }

        private static void Write(IModelDirectoryContent dirContent, string path) {
            if (dirContent.Type == ContentType.File) {
                var file = dirContent as ModelFile;

                var locked = true;
                while (locked) {
                    try
                    {
                        FileStream stream;
                        using (stream = File.Open(path + Path.DirectorySeparatorChar + file.Name, FileMode.Create))
                        {
                            stream.Write(file.Content, 0, file.Content.Length);
                            locked = false;
                        }
                    }
                    catch (IOException) {
                        locked = true;
                    }
                }
            }
            else {
                var folder = dirContent as ModelFolder;
                if (!Directory.Exists(path + Path.DirectorySeparatorChar + dirContent.Name))
                    Directory.CreateDirectory(path + Path.DirectorySeparatorChar + dirContent.Name);

                foreach (IModelDirectoryContent content in folder.Contents) {
                    Write(content, path + Path.DirectorySeparatorChar + dirContent.Name);
                }
            }
        }
    }
}