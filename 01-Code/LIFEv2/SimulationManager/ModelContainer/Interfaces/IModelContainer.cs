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
        ///     Registers the callback. In the event, that the underlying filesystem has changed, callbacks are informed.
        /// </summary>
        /// <see cref="ModelDirectoryChanged" />
        void RegisterForModelListChange(Action callback);

        /// <summary>
        ///     Returns a list of all available models.
        /// </summary>
        /// <returns>empty, if none found</returns>
        ICollection<TModelDescription> GetAllModels();

        /// <summary>
        ///     Returns the serialized contents of the given model.
        /// </summary>
        /// <param name="modelID">must not be null</param>
        /// <returns>null, if model not found.</returns>
        ModelContent GetModel(TModelDescription modelID);

        /// <summary>
        /// Returns the SimConfig object created with MARS SHUTTLE if available,
        /// returns null otherwise.
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns>The Simconfig object or null</returns>
        ISimConfig GetShuttleSimConfig(TModelDescription modelId, string simConfigName);

        /// <summary>
        /// Returns the ModelConfig for the given modelDescription
        /// </summary>
        /// <param name="modelId"></param>
        /// <returns></returns>
        ModelConfig GetModelConfig(TModelDescription modelId);

        /// <summary>
        ///     Copies the contents of filePath into a folder into a model folder with the same name.
        /// </summary>
        /// <param name="filePath">not null</param>
        TModelDescription AddModelFromDirectory(string filePath);

        /// <summary>
        ///     Deletes the model and the folder it is contained in from the model folder.
        /// </summary>
        /// <param name="model"></param>
        void DeleteModel(TModelDescription model);

        /// <summary>
        ///     Calculates a feasible instantion order for the layers the model consists of.
        /// </summary>
        /// <param name="model">not null</param>
        /// <returns>empty, if no </returns>
        IList<TLayerDescription> GetInstantiationOrder(TModelDescription model);
    }
}