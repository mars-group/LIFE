// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Windows;
using LayerAPI.Layer;
using Mono.Addins;
using PlantLayer.Agents;
using TwoDimEnvironment;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace PlantLayer {
    [Extension(typeof (ISteppedLayer))]
    public class PlantLayerImpl : ISteppedLayer {
        private ITwoDimEnvironment<Plant> environment;
        private List<Plant> _plants;
        private long _currentTick;

        public PlantLayerImpl() {}

        #region ISteppedLayer Members

        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            environment = new TwoDimEnvironmentUseCase<Plant>();
            _plants = new List<Plant>();
            // create a 100x100 playboard and add plants everywhere but in the middle
            for (int x = 0; x < 100; x++) {
                for (int y = 0; y < 100; y++) {
                    if ((x < 48 && y < 48) || (x > 52 && y > 52)) {
                        Plant p = new Plant(x, y, new Size(1.0, 1.0));
                        registerAgentHandle.Invoke(this, p);
                        _plants.Add(p);
                        environment.Add(p);
                    }
                }
            }
            Console.WriteLine("PlantLayer just finished initializing!");
            return true;
        }

        public long GetCurrentTick() {
            return _currentTick;
        }

        public void SetCurrentTick(long currentTick) {
            _currentTick = currentTick;
        }

        #endregion

        public void Stomp(double x, double y, Size size, double force) {
            // check if something is there
            List<Plant> p = environment.Find(new Rect(x, y, size.Width, size.Height));
            // for everything found, stomp upon it and save it back to environment
            foreach (Plant plant in p) {
                plant.SubHealth(force);
                environment.Update(plant);
            }
        }

        public void Stomp(Rect bounds, double force) {
            Stomp(bounds.X, bounds.Y, bounds.Size, force);
        }

        public List<TPlant> GetAllPlants() {
            List<Plant> allPlants = environment.GetAll();
            List<TPlant> result = new List<TPlant>();
            allPlants.ForEach(p => result.Add(new TPlant(p)));
            return result;
        }
    }
}