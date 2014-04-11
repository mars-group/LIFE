using System;
using System.Data;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

[assembly: InternalsVisibleTo("MulticastAdapterTestProject")]

namespace ConfigurationAdapter.Interface
{
    public static class Configuration
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        #region invisible
        public string FileName { get; protected set; }
        public T Instance { get; protected set; }

        private readonly XmlSerializer serializer;
        private FileStream file;

        /// <summary>
        ///     Grants access to a configuration file.
        /// </summary>
        /// <remarks>If the file did not exist, it will be created automatically.</remarks>
        /// <param name="fileName">The path to the config file. If left out, it will be be</param>
        public Configuration(string fileName = null)
        {
            
            serializer = new XmlSerializer(typeof(T));
            GetConfigfile(FileName);
        }
        /// <summary>
        /// For testing only
        /// </summary>
        /// <param name="t"></param>
        public Configuration(T t) {
            Instance = t;
        } 


        private void GetConfigfile(string fileName)
        {

        }

        public void Save()
        {
            file = new FileStream(FileName, FileMode.Create);
            serializer.Serialize(file, Instance);
            file.Flush();
        }

        #endregion

        public static T GetConfiguration<T>(string file = null) {
            string path = file ?? "./" + typeof(T).Name + ".config";

            if (File.Exists(path))
            {
                try {
                    file = new FileStream(FileName, FileMode.Open);
                    Instance = (T) serializer.Deserialize(file);
                    file.Close();
                }
                catch (Exception exception) {
                    Logger.
                }
            }
            else
            {
                FileName = path;
                Instance = (T)typeof(T).GetConstructor(new Type[0]).Invoke(new object[0]);
                using (file = new FileStream(FileName, FileMode.OpenOrCreate))
                {
                    serializer.Serialize(file, Instance);
                    file.Flush();
                }



            }
        }

        public static void Save(T t) {
            
        }
    }
}