//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 23.01.2016
//  *******************************************************/
using System;
using ASC.Communication.ScsServices.Communication.Messages;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     Represents a SCS Service Application that is used to construct and manage a SCS service.
    /// </summary>
    public interface IAscServiceApplication {
        /// <summary>
        ///     This event is raised when an AddShadowAgentMessage has been received
        /// </summary>
        event EventHandler<AddShadowAgentEventArgs> AddShadowAgentMessageReceived;

        /// <summary>
        ///     This event is raised when an RemoveShadowAgentMessage has been received
        /// </summary>
        event EventHandler<RemoveShadowAgentEventArgs> RemoveShadowAgentMessageReceived;

        /// <summary>
        ///     Starts service application.
        /// </summary>
        void Start();

        /// <summary>
        ///     Stops service application.
        /// </summary>
        void Stop();

        /// <summary>
        ///     Adds a service object to this service application.
        ///     Only single service object can be added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be derived from AscService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
		/// <param name = "typeOfTServiceInterface">The optional Type of TServiceInterface</param>
		void AddService<TServiceInterface, TServiceClass>(TServiceClass service, Type typeOfTServiceInterface = null)
            where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class;

		/// <summary>
		/// Gets the service object reference identified by id.
		/// </summary>
		/// <returns>The service object reference</returns>
		/// <param name="id">The Guid of the service object</param>
		/// <typeparam name="TServiceInterface">The service's interface.</typeparam>
		/// <param name = "typeName">The optional typeName of TServiceInterface.</param>
		TServiceInterface GetServiceByID<TServiceInterface, TServiceClass>(Guid id, string typeName = "") 
			where TServiceClass : AscService, TServiceInterface
			where TServiceInterface : class;

		/// <summary>
		/// Check whether this ServiceApplication contains a service object identified
		/// by id and typeName.
		/// </summary>
		/// <returns><c>true</c>, if service is contained, <c>false</c> otherwise.</returns>
		/// <param name="id">Identifier.</param>
		/// <param name="typeName">Type name.</param>
		/// <typeparam name="TServiceInterface">ID of the Service.</typeparam>
		/// <typeparam name="TServiceClass">Optional typeName .</typeparam>
		bool ContainsService<TServiceInterface, TServiceClass>(Guid id, string typeName = "") 
			where TServiceClass : AscService, TServiceInterface
			where TServiceInterface : class;

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        bool RemoveService<TServiceInterface>() where TServiceInterface : class;

        bool RemoveService<TServiceInterface>(Guid serviceGuid) where TServiceInterface : class;
    }
}