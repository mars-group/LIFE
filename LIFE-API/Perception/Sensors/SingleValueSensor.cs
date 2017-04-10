//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using LIFE.API.Layer.Data;

namespace LIFE.API.Perception.Sensors
{
    /// <summary>
    ///   Sensor for single value data layers.
    /// </summary>
    /// <typeparam name="T">Layer data type.</typeparam>
    public class SingleValueSensor<T>
    {
        private readonly ISingleValueLayer<T> _layer; // Layer reference.


        /// <summary>
        ///   Create a sensor for single value grids.
        /// </summary>
        /// <param name="layer">The data layer.</param>
        public SingleValueSensor(ISingleValueLayer<T> layer)
        {
            _layer = layer;
        }


        /// <summary>
        ///   Returns the current value of the layer.
        /// </summary>
        /// <returns>Layer value.</returns>
        public T GetValue()
        {
            return _layer.GetValue();
        }
    }
}