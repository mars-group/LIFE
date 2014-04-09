using System;
using System.IO;
using System.Xml.Serialization;

namespace ConfigurationAdapter.Interface {
    public class Configuration<T> {
        private readonly XmlSerializer serializer;

        private FileStream file;

        //TODO is (noch) nicht geil :-(
        /// <summary>
        ///     This is the constructor to load an already existing configuration from the given filename.
        ///     If the file, with the given filename does not exist a new file is writen from the default constructor of T.
        /// </summary>
        /// <param name="fileName">not null, must exist.</param>
        public Configuration(string fileName) {
            if (File.Exists(fileName)) {
                FileName = fileName;
                serializer = new XmlSerializer(typeof (T));
                file = new FileStream(FileName, FileMode.Open);
                Content = (T) serializer.Deserialize(file);
                file.Close();
            }
            else {
                serializer = new XmlSerializer(typeof (T));
                FileName = fileName;
                Content = (T) typeof (T).GetConstructor(new Type[0]).Invoke(new object[0]);
                file = new FileStream(FileName, FileMode.CreateNew);
                serializer.Serialize(file, Content);
                file.Flush();
                file.Close();
            }
        }

        public string FileName { get; protected set; }

        public T Content { get; protected set; }

        public void Save() {
            file = new FileStream(FileName, FileMode.Create);
            serializer.Serialize(file, Content);
            file.Flush();
        }
    }
}