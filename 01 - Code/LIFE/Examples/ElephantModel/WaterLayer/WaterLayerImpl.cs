// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System.Collections.Generic;
using System.Windows;
using LayerAPI.Layer;
using Mono.Addins;
using TwoDimEnvironment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace WaterLayer {
    [Extension(typeof (ISteppedLayer))]
    public class WaterLayerImpl : ISteppedLayer {
        private ITwoDimEnvironment<Waterhole> environment;

        private List<Waterhole> _waterholes;
        private long _currentTick;

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            environment = new TwoDimEnvironmentUseCase<Waterhole>();
            _waterholes = new List<Waterhole>();

            Waterhole p = new Waterhole(48, 52, new Size(5.0, 5.0));
            registerAgentHandle.Invoke(this, p);
            _waterholes.Add(p);
            environment.Add(p);

            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion

        public List<TWaterhole> getAllWaterholes() {
            List<Waterhole> allWaterholes = environment.GetAll();
            List<TWaterhole> result = new List<TWaterhole>();
            allWaterholes.ForEach(w => result.Add(new TWaterhole(w)));
            return result;
        }

        public List<TWaterhole> Probe(double x, double y, double distance) {
            List<Waterhole> holes = environment.Find(new Rect(x, y, distance, distance));
            List<TWaterhole> result = new List<TWaterhole>();
            holes.ForEach(h => result.Add(new TWaterhole(h)));
            return result;
        }
    }
}