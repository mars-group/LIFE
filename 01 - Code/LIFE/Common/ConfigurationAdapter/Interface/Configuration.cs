using System;
using System.IO;
using System.Xml.Serialization;

namespace ConfigurationAdapter.Interface {
    public class Configuration<T> {
        public string FileName { get; protected set; }
        public T Content { get; protected set; }

        private readonly XmlSerializer serializer;
        private FileStream file;

        /// <summary>
        ///     Grants access to a configuration file.
        /// </summary>
        /// <remarks>If the file did not exist, it will be created automatically.</remarks>
        /// <param name="fileName">The path to the config file. If left out, it will be be</param>
        public Configuration(string fileName = null) {
            FileName = fileName ?? "./" + typeof (T).Name + ".config";
            serializer = new XmlSerializer(typeof (T));
            GetConfigfile(FileName);
        }

        private void GetConfigfile(string fileName) {
            if (File.Exists(fileName)) {
                FileName = fileName;
                file = new FileStream(FileName, FileMode.Open);
                Content = (T) serializer.Deserialize(file);
                file.Close();
            }
            else {
                FileName = fileName;
                Content = (T) typeof (T).GetConstructor(new Type[0]).Invoke(new object[0]);
                file = new FileStream(FileName, FileMode.OpenOrCreate);
                serializer.Serialize(file, Content);
                file.Flush();
                file.Close();
            }
        }

        public void Save() {
            file = new FileStream(FileName, FileMode.Create);
            serializer.Serialize(file, Content);
            file.Flush();
        }
    }
}