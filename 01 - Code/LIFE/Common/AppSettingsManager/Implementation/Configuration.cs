using System.IO;
using System.Xml.Serialization;

namespace AppSettingsManager.Implementation {
    public class Configuration<T> {
        private readonly XmlSerializer serializer;

        private FileStream file;
        /// <summary>
        /// This is the constructor for creating a new configuration file with the given filename.
        /// </summary>
        /// <param name="configuration">not null</param>
        /// <param name="fileName">not null</param>
        public Configuration(T configuration, string fileName) {
            serializer = new XmlSerializer(typeof (T));
            FileName = fileName;
            Content = configuration;
            file = new FileStream(FileName, FileMode.CreateNew);
            serializer.Serialize(file, configuration);
            file.Flush();
            file.Close();
        }

        /// <summary>
        /// This is the constructor to load an already existing configuration from the given filename.
        /// </summary>
        /// <param name="fileName">not null, must exist.</param>
        public Configuration(string fileName) {
            FileName = fileName;
            serializer = new XmlSerializer(typeof (T));
            file = new FileStream(FileName, FileMode.Open);
            Content = (T) serializer.Deserialize(file);
            file.Close();
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