using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using log4net;

[assembly: InternalsVisibleTo("MulticastAdapterTestProject")]

namespace ConfigurationAdapter.Interface {
    public static class Configuration {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Configuration));

        /// <summary>
        ///     Grants access to a config file. If no file exists, it will be uatomatically created.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T GetConfiguration<T>(string fileName = null) {
            string path = fileName ?? "./" + typeof (T).Name + ".config";
            T result = default(T);

            XmlSerializer serializer;
            FileStream file = null;

            try {
                serializer = new XmlSerializer(typeof (T));
                if (File.Exists(path)) {
                    file = new FileStream(path, FileMode.Open);
                    result = (T) serializer.Deserialize(file);
                }
                else {
                    file = new FileStream(path, FileMode.OpenOrCreate);
                    result = (T) typeof (T).GetConstructor(new Type[0]).Invoke(new object[0]);
                    serializer.Serialize(file, result);
                    file.Flush();
                }
            }
            catch (Exception exception) {
                Logger.Error(exception);
            }
            finally {
                if (file != null) file.Close();
            }

            return result;
        }

        /// <summary>
        ///     Saves a configuration. If one already existed witht hat name, it will be overwritten.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="fileName"></param>
        public static void Save<T>(T t, string fileName = null) {
            string path = fileName ?? "./" + typeof (T).Name + ".config";
            using (var file = new FileStream(path, FileMode.OpenOrCreate)) {
                new XmlSerializer(typeof (T)).Serialize(file, t);
                file.Flush();
                file.Close();
            }
        }
    }
}