//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using LCConnector.TransportTypes;
using LCConnector.TransportTypes.ModelStructure;

namespace LCConnector {
    /// <summary>
    /// The LayerContainer's public interface. This may be jused locally or via the provided SCS service.
    /// </summary>
    public interface ILayerContainer {
        /// <summary>
        ///     Transmits the serialized model content for the next simulation run to the layer container.
        /// </summary>
        /// <param name="content">not null</param>
        void LoadModelContent(ModelContent content);

        /// <summary>
        ///     Orders the LayerContainer to instantiate a version of the given layer id from the model content.
        ///     If there already was an instance of that type instantiated, it will be overwritten by the new one.
        /// </summary>
        /// <param name="instanceId">A simulation wide unique layer identification.</param>
        void Instantiate(TLayerInstanceId instanceId);

        /// <summary>
        ///     Initializes the layer with the given id with the given init data.
        /// </summary>
        /// <param name="instanceId">not null</param>
        /// <param name="initData">not null</param>
        /// <exception cref="Exceptions.LayerNotInstantiatedException">If the layer was not yet instantiated.</exception>
        bool InitializeLayer(TLayerInstanceId instanceId, TInitData initData);

        /// <summary>
        ///     Calculate one simulation step.
        /// </summary>
        /// <returns>The duration of the tick execution in milliseconds.</returns>
        /// <exception cref="Exceptions.LayerNotInitializedException">If one of the layers not yet initialized.</exception>
        long Tick();

		/// <summary>
		/// Cleans up all Layers which implement the IDisposableLayer 
		/// interface.
		/// </summary>
		void CleanUp();

		/// <summary>
		/// Sets the mars config service address. Use this if you want to override the default setting
		/// which points to the MARSConfig Docker container
		/// </summary>
		/// <returns>The mars config service address.</returns>
		/// <param name="marsConfigServiceAddress">Mars config service address.</param>
		void SetMarsConfigServiceAddress(string marsConfigServiceAddress);
    }
}