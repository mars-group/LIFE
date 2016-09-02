//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     A file
    /// </summary>
    internal class ModelFile : IModelDirectoryContent
    {
        public byte[] Content { get; set; }


        public ModelFile(string name) {
            var parts = name.Split(Path.DirectorySeparatorChar);
            Name = parts[parts.Length - 1];
            Content = File.ReadAllBytes(name);
        }

        public ModelFile() {}

        #region IModelDirectoryContent Members



        public string Name { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter))]
        public ContentType Type { get { return ContentType.File; } }

        #endregion
    }
}