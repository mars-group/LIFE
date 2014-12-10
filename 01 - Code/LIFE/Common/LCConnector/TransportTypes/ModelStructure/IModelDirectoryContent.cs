// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

namespace LCConnector.TransportTypes.ModelStructure {
    /// <summary>
    ///     the interface of an item that can occur in a model folder.
    /// </summary>
    internal interface IModelDirectoryContent {
        /// <summary>
        ///     The item's filename in the fileystem.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        ///     Is it a file or a folder?
        /// </summary>
        ContentType Type { get; }
    }
}