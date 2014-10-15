using System;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure
{
    /// <summary>
    ///     This class holds the content of a model and is able to write it to a stream.
    /// </summary>
    [Serializable]
    public class ModelContent
    {
        private readonly ModelFolder _root;

        public ModelContent(string modelPath)
        {
            _root = new ModelFolder(modelPath);
        }

        public ModelContent() { }

        /// <summary>
        /// Writes the complete content of the model folder into the target directory.<br/>
        /// If any file or directory already existed, it will be overwritten.
        /// </summary>
        /// <param name="targetDirectory"></param>
        public void Write(string targetDirectory)
        {
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }
            EmptyDirectory(targetDirectory);
            foreach (var modelDirectoryContent in _root.Contents)
            {
                Write(modelDirectoryContent, targetDirectory);
            }
        }

        private static void EmptyDirectory(string targetDirectory)
        {

            var dirInfo = new DirectoryInfo(targetDirectory);

            foreach (var file in dirInfo.GetFiles()) {
                WaitForFile(file.FullName);
                //    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", file.FullName));
                file.Delete();
            }

            foreach (var dir in dirInfo.GetDirectories()) {
                WaitForFile(dir.FullName);
                //    throw new IOException(string.Format("Could not delete {0} because it is used by someone else.", dir.FullName));
                dir.Delete(true);
            }

        }

        /// <summary>
        /// Blocks until the file is not locked any more.
        /// </summary>
        /// <param name="fullPath"></param>
        private static bool WaitForFile(string fullPath, int retryCount = 10, int sleep = 500)
        {
            var numTries = 0;
            while (numTries++ < retryCount)
            {
                try
                {
                    // Attempt to open the file exclusively.
                    using (var fs = new FileStream(fullPath,
                        FileMode.Open, FileAccess.ReadWrite,
                        FileShare.None, 100))
                    {
                        fs.ReadByte();

                        // If we got this far the file is ready
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    // Wait for the lock to be released
                    System.Threading.Thread.Sleep(sleep);
                }
            }
            return false;
        }

        private static void Write(IModelDirectoryContent dirContent, string path)
        {
            if (dirContent.Type == ContentType.File)
            {
                var file = dirContent as ModelFile;
                var stream = File.Open(path + Path.DirectorySeparatorChar + file.Name, FileMode.Create);
                
                stream.Write(file.Content, 0, file.Content.Length);

                stream.Close();
                
            }
            else
            {
                var folder = dirContent as ModelFolder;
                if (!Directory.Exists(path + Path.DirectorySeparatorChar + dirContent.Name))
                {
                    Directory.CreateDirectory(path + Path.DirectorySeparatorChar + dirContent.Name);
                }

                foreach (var content in folder.Contents)
                {
                    Write(content, path + Path.DirectorySeparatorChar + dirContent.Name);
                }
            }
        }
    }
}