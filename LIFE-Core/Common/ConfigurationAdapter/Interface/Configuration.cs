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
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;



[assembly: InternalsVisibleTo("MulticastAdapterTestProject")]

namespace ConfigurationAdapter.Interface {
    public static class Configuration {

        /// <summary>
        ///     Grants access to a config file. If no file exists, it will be created automatically.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T Load<T>(string fileName = null) {
            string path = fileName ?? "./config/" + typeof (T).Name + ".cfg";
            T result = default(T);

            XmlSerializer serializer;

            serializer = new XmlSerializer(typeof (T));
            if (File.Exists(path)) {
                using (var file = new FileStream(path, FileMode.Open)) {
                    result = (T) serializer.Deserialize(file);
                }
            }
            else {
                CreatePath("./config");
                using (FileStream file = new FileStream(path, FileMode.Create)) {
                    result = (T) typeof (T).GetTypeInfo().GetConstructor(new Type[0]).Invoke(new object[0]);
                    serializer.Serialize(file, result);
                }
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
            using (FileStream file = new FileStream(path, FileMode.OpenOrCreate)) {
                new XmlSerializer(typeof (T)).Serialize(file, t);
                file.Flush();
            }
        }

        /// <summary>
        ///     Create all folders in a path, if missing.
        /// </summary>
        /// <param name="path"></param>
        private static void CreatePath(string path) {
            path = path.TrimEnd('/', '\\');
            if (Directory.Exists(path)) return;
            if (Path.GetDirectoryName(path) != "") CreatePath(Path.GetDirectoryName(path)); // Make up to parent
            Directory.CreateDirectory(path); // Make this one
        }
    }
}