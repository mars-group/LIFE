//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;
using LifeAPI.Config;
using MARS.Shuttle.SimulationConfig.Interfaces;
using Newtonsoft.Json.Linq;
using SMConnector.TransportTypes;

namespace ModelContainer.Interfaces {
    /// <summary>
    ///     The event that is raised, if the model directory has been altered and there might be new models available or old
    ///     ones deleted.
    /// </summary>
    public delegate void ModelDirectoryChanged();

    /// <summary>
    ///     This is the interface for the model container component.
    /// </summary>
    /// <remarks>
    ///     It does what one would expect from a manager:<br />
    ///     * Getting a list of all available models,<br />
    ///     * adding new models<br />
    ///     * deleting models<br />
    ///     * It also actively scans the model directory for changes and informs possible interested listeners.
    /// </remarks>
    public interface IModelContainer {

        /// <summary>
        ///     Returns a TModelDescription for the given modelPath
        /// </summary>
        /// <returns>TModelDescription, throws Exception if not model found.</returns>
        TModelDescription GetModelDescription(string modelPath);

        /// <summary>
        ///     Returns the serialized contents of the given model.
        /// </summary>
        /// <param name="modelPath">must not be null</param>
        /// <returns>ModelContent, throws Exception if no model found</returns>
        ModelContent GetSerializedModel(TModelDescription modelPath);

        /// <summary>
        /// Returns a JObject SimConfig object created from a scenarioConfig from the MARS Cloud
        /// if available.
        /// returns null otherwise.
        /// </summary>
        /// <param name="modelId"></param>
        /// <param name="scenarioConfigId"></param>
        /// <returns>The Simconfig object or null</returns>
        JObject GetSimulationConfig(TModelDescription modelId, string scenarioConfigId);

        /// <summary>
        /// Returns the ModelConfig for the given modelDescription
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        ModelConfig GetModelConfig(TModelDescription modelId);

        /// <summary>
        ///     Calculates a feasible instantion order for the layers the model consists of.
        /// </summary>
        /// <param name="model">not null</param>
        /// <returns>empty, if no </returns>
        IList<TLayerDescription> GetInstantiationOrder(TModelDescription model);
    }
}