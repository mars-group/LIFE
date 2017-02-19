//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using CommonTypes.DataTypes;
using SMConnector;
using SMConnector.TransportTypes;

namespace RuntimeEnvironment.Interfaces {
    /// <summary>
    /// The simulation manager runtime environment. Controls the main simulation task.
    /// </summary>
    public interface IRuntimeEnvironment {
        /// <summary>
        /// Starts a simulation with the provided model.
        /// </summary>
        /// <param name="simulationId">The unique simulation ID of this simulation run.</param>
        /// <param name="model">The model description to start with.</param>
        /// <param name="layerContainerNodes">The layer container instances to be used in this run.</param>
        /// <param name="nrOfTicks">The number of ticks to be simulated</param>
        /// <param name="scenarioConfigName">The scenarioconfig to use, if any</param>
        /// <param name="startPaused">Whether or not to start the simulation paused.</param>
        void StartWithModel(Guid simulationId,TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null, string scenarioConfigName = "", bool startPaused = false);

        /// <summary>
        /// Steps the simulation by <param name="nrOfTicks"/> or 1 tick if the parameter is not set.
        /// </summary>
        /// <param name="model">The model to be used</param>
        /// <param name="layerContainerNodes">The layerContainerNodes to be used</param>
        /// <param name="nrOfTicks">The number of ticks to be stepped</param>
        void StepSimulation(TModelDescription model, ICollection<TNodeInformation> layerContainerNodes, int? nrOfTicks = null);

        /// <summary>
        /// Pauses the current execution.
        /// </summary>
        /// <param name="model">The model whose simulation to pause.</param>
        void Pause(TModelDescription model);

        /// <summary>
        /// Resumes the current execution.
        /// </summary>
        /// <param name="model">The model whose simulation to continue.</param>
        void Resume(TModelDescription model);

        /// <summary>
        /// Aborts the current execution.
        /// </summary>
        /// <param name="model">The model whose simulation to abort.</param>
        void Abort(TModelDescription model);

		/// <summary>
		/// Waits for simulation to finish.
		/// This will block the current thread and return execution, when 
		/// the simulation is completed.
		/// </summary>
		/// <param name="model">Model.</param>
		void WaitForSimulationToFinish (TModelDescription model);

        /// <summary>
        /// Subscribes the provided delegate for a status update, regarding the simulation.
        /// </summary>
        /// <param name="statusUpdateAvailable"></param>
        void SubscribeForStatusUpdate(StatusUpdateAvailable statusUpdateAvailable);
    }
}