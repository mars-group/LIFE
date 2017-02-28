//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using LNSConnector.Interface;
using LNSConnector.TransportTypes;

namespace LayerNameService.Implementation {

  internal class LayerNameServiceUseCase : ILayerNameService {

    private readonly IDictionary<string, List<TLayerNameServiceEntry>> _layerMap;

    public LayerNameServiceUseCase() {
      _layerMap = new ConcurrentDictionary<string, List<TLayerNameServiceEntry>>();
    }

    public TLayerNameServiceEntry ResolveLayer(string layerType) {
      return _layerMap[layerType].FirstOrDefault();
    }

    public void RegisterLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry) {
      if (!_layerMap.ContainsKey(layerType))
        _layerMap[layerType] = new List<TLayerNameServiceEntry>();
      _layerMap[layerType].Add(layerNameServiceEntry);
    }

    public void RemoveLayer(string layerType, TLayerNameServiceEntry layerNameServiceEntry) {
      if (_layerMap[layerType].Count <= 1)
        _layerMap.Remove(layerType);
      else
        _layerMap[layerType].Remove(layerNameServiceEntry);
    }
  }
}