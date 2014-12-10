// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     A file
    /// </summary>
    [Serializable]
    internal class ModelFile : IModelDirectoryContent {
        public byte[] Content { get; set; }

        public ModelFile(string name) {
            string[] parts = name.Split(Path.DirectorySeparatorChar);
            Name = parts[parts.Length - 1];
            Content = File.ReadAllBytes(name);
        }

        public ModelFile() {}

        #region IModelDirectoryContent Members

        public string Name { get; set; }

        public ContentType Type { get { return ContentType.File; } }

        #endregion
    }
}