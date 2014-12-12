// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 21.11.2014
//  *******************************************************/

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ForestLayer.Agents;
using ForestLayer.TransportTypes;
using Hik.Collections;
using LayerAPI.Layer;
using mars.rock.drill;
using Mono.Addins;

[assembly: Addin]
[assembly: AddinDependency("LayerContainer", "0.1")]

namespace AWRShowcase {
    [Extension(typeof (ISteppedLayer))]
    public class ForestLayer : ISteppedLayer {
        private const int ScalingFactor = 100;
        private readonly ThreadSafeSortedList<Guid, Tree> _treeList;
        private readonly Random _rand;

        public ForestLayer() {
            _treeList = new ThreadSafeSortedList<Guid, Tree>();
            _rand = new Random();
        }

        #region ISteppedLayer Members

        /// <summary>
        ///     Inits the forest layer from data placed in the MARS ROCK db.
        /// </summary>
        /// <returns><c>true</c>, if layer was inited, <c>false</c> otherwise.</returns>
        /// <param name="layerInitData">Layer init data.</param>
        /// <param name="registerAgentHandle">Register agent handle.</param>
        /// <param name="unregisterAgentHandle">Unregister agent handle.</param>
        public bool InitLayer<I>
            (I layerInitData, RegisterAgent registerAgentHandle, UnregisterAgent unregisterAgentHandle) {
            Drill.InitializeConnection("rock.mars.haw-hamburg.de", "mars", "rock");
            Cube cubeAwr = Drill.Cubes.FirstOrDefault(cube => cube.CubeName == "awr");
            if (cubeAwr == null) throw new IOException("Could not load data from database.");
            DataTable resultTable = cubeAwr.GetData();
            Parallel.For
                (0,
                    resultTable.Rows.Count,
                    delegate(int i) {
                        DataRow row = resultTable.Rows[i];
                        double biomass = double.Parse(row["t_biomass"].ToString());
                        double height = double.Parse(row["t_height"].ToString());
                        double diameter = double.Parse(row["t_diameter"].ToString());
                        double crownDiameter = double.Parse(row["t_crown"].ToString());
                        double lat = double.Parse(row["s_lat"].ToString());
                        double lon = double.Parse(row["s_lon"].ToString());

                        Parallel.For
                            (0,
                                ScalingFactor,
                                delegate(int j) {
                                    Tree tree = new Tree(height, diameter, crownDiameter, 5, biomass, lat, lon);
                                    _treeList[tree.TreeId] = tree;
                                });
                    });

            foreach (Tree tree in _treeList.GetAllItems()) {
                registerAgentHandle.Invoke(this, tree);
            }
            Console.WriteLine("Initialized " + _treeList.Count + " trees!");
            return true;
        }

        public long GetCurrentTick() {
            throw new NotImplementedException();
        }

        public void SetCurrentTick(long currentTick) {
            throw new NotImplementedException();
        }

        #endregion

        public TTree GetTree() {
            return new TTree(_treeList.GetAllItems().ToArray()[_rand.Next(_treeList.Count)]);
        }

        public bool CutTree(Guid treeId) {
            return _treeList.Remove(treeId);
        }
    }
}