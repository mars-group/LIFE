// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ElephantLayer;
using ElephantLayer.TransportTypes;
using LifeAPI.Agent;
using PlantLayer;
using WaterLayer;

namespace ResultLayer {
    public class MatrixToFileResultAgent : IAgent {
        private readonly ElephantLayerImpl _elephantLayer;
        private readonly PlantLayerImpl _plantLayer;
        private WaterLayerImpl _waterLayer;

        private int _tickCount;

        private string _simrunDateTime;

        public MatrixToFileResultAgent
            (ElephantLayerImpl elephantLayer, PlantLayerImpl plantLayer, WaterLayerImpl waterlayer) {
            _elephantLayer = elephantLayer;
            _plantLayer = plantLayer;
            _waterLayer = waterlayer;
            _tickCount = 0;
            _simrunDateTime = DateTime.Now.ToString("HH:mm:ss tt zzz");
        }

        #region ITickClient implementation

        public void Tick() {
            //Console.WriteLine("MatrixWriter reportin' in!");
            // WriteOut Matrix for plant layer
            List<TPlant> plants = _plantLayer.GetAllPlants();

            StringBuilder stb = new StringBuilder();

            double x = 0.0;
            plants.ForEach
                (plant => {
                    if (plant.GetBounds().X > x) {
                        stb.Append("\r\n" + plant.GetHealth() + " ");
                        x = plant.GetBounds().X;
                    }
                    else stb.Append(plant.GetHealth() + " ");
                });

            WriteMatrixToFile("plantlayer", stb.ToString(), "Tick-" + _tickCount + ".asc");

            List<TElephant> elephants = _elephantLayer.GetAllElephants();

            StringBuilder stbEle = new StringBuilder();

            x = 0.0;
            elephants.ForEach
                (elephant => {
                    if (elephant.GetBounds().X > x) {
                        stbEle.Append("\r\n" + elephant.Center.X + "/" + elephant.Center.Y + " ");
                        x = elephant.GetBounds().X;
                    }
                    else stbEle.Append(elephant.Center.X + "/" + elephant.Center.Y + " ");
                });

            WriteMatrixToFile("elephantLayer", stbEle.ToString(), "Tick-" + _tickCount + ".asc");
            _tickCount++;
        }

        #endregion

        private void WriteMatrixToFile(string layerName, string matrix, string filename) {
            // Write the string to a file.
            string dirpath = "." + "/" + layerName + "MatrixOutputsFrom" + "/";
            if (!Directory.Exists(dirpath)) Directory.CreateDirectory(dirpath);
            string filepath = Path.Combine(dirpath, filename);
            File.WriteAllText(filepath, matrix);
        }

        public Guid ID { get; set; }
    }
}