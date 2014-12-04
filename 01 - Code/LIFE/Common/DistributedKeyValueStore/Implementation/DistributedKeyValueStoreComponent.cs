// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 13.05.2014
//  *******************************************************/

using System.Collections.Generic;
using DistributedKeyValueStore.Interface;
using NodeRegistry.Interface;

namespace DistributedKeyValueStore.Implementation {
    public class DistributedKeyValueStoreComponent : IDistributedKeyValueStore {
        private readonly IDistributedKeyValueStore _distributedKeyValueStore;

        public DistributedKeyValueStoreComponent(INodeRegistry nodeRegistry) {
            _distributedKeyValueStore = new DistributedKeyValueStoreUseCase(nodeRegistry);
        }

        #region IDistributedKeyValueStore Members

        public void Put(string key, string value) {
            _distributedKeyValueStore.Put(key, value);
        }

        public IList<string> Get(string key) {
            return _distributedKeyValueStore.Get(key);
        }

        #endregion
    }
}