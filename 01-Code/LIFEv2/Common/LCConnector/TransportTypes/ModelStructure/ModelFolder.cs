﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System.Collections.Generic;
using System.IO;

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     A folder within a model directory.
    /// </summary>
    internal class ModelFolder : IModelDirectoryContent {
        public IList<IModelDirectoryContent> Contents { get; set; }

        public ModelFolder(string name) {
                Contents = new List<IModelDirectoryContent>();
                string[] parts = name.Split(Path.DirectorySeparatorChar);
                Name = parts[parts.Length - 1];

                IEnumerable<string> files = Directory.EnumerateFileSystemEntries(name);

                foreach (string file in files)
                {
                    FileAttributes attributes = File.GetAttributes(file);

                    if (attributes == FileAttributes.Directory)
                    {
                        Contents.Add(new ModelFolder(file));
                    }
                    else
                    {
                        Contents.Add(new ModelFile(file));
                    }
                }
        }

        public ModelFolder() {}

        #region IModelDirectoryContent Members

        public string Name { get; set; }

        public ContentType Type { get { return ContentType.Folder; } }

        #endregion
    }
}