﻿using System;
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
        public static T Load<T>(string fileName = null) {
            string path = fileName ?? "./config/" + typeof (T).Name + ".cfg";
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
                    CreatePath("./config");
                    file = new FileStream(path, FileMode.Create);
                    result = (T) typeof (T).GetConstructor(new Type[0]).Invoke(new object[0]);
                    serializer.Serialize(file, result);
                    file.Flush();
                }
            }
            catch (Exception exception) {
                Logger.Error(exception);
                throw exception;
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
            string path = fileName ?? "./config/" + typeof (T).Name + ".cfg";
            CreatePath("./config");
            using (var file = new FileStream(path, FileMode.OpenOrCreate)) {
                new XmlSerializer(typeof (T)).Serialize(file, t);
                file.Flush();
                file.Close();
            }
        }

        /// <summary>
        ///     Create all folders in a path, if missing.
        /// </summary>
        /// <param name="path"></param>
        private static void CreatePath(string path)
        {
            path = path.TrimEnd('/', '\\');
            if (Directory.Exists(path))
            {
                return;
            }
            if (Path.GetDirectoryName(path) != "") CreatePath(Path.GetDirectoryName(path)); // Make up to parent
            Directory.CreateDirectory(path); // Make this one
        }
    }
}