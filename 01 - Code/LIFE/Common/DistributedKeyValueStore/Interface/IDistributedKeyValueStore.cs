// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System.Collections.Generic;

namespace DistributedKeyValueStore.Interface {
    public interface IDistributedKeyValueStore {
        /// <summary>
        ///     Stores string value under key into the DHT
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        void Put(string key, string value);

        /// <summary>
        ///     Returns the entries for key as a list of strings
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A list containing all entries, an empty list if no entries are found</returns>
        IList<string> Get(string key);
    }
}