﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 26.01.2016
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Server;
using ASC.Communication.ScsServices.Communication.Messages;

[assembly: InternalsVisibleTo("AgentShadowingServiceTests")]
[assembly: InternalsVisibleTo("PerfTester")]
namespace ASC.Communication.ScsServices.Service
{

    /// <summary>
    ///     Implements IAscServiceApplication and provides all functionallity.
    /// </summary>
	internal class AscServiceApplication : IAscServiceApplication {
        #region Public events

        public event EventHandler<AddShadowAgentEventArgs> AddShadowAgentMessageReceived;

        public event EventHandler<RemoveShadowAgentEventArgs> RemoveShadowAgentMessageReceived;

        #endregion

        #region Private fields

        /// <summary>
        ///     Underlying IAscServer object to accept and manage client connections.
        /// </summary>
        private readonly IAscServer _ascServer;

        /// <summary>
        ///     User service objects that is used to invoke incoming method invocation requests.
        ///     Key1: Service interface type's name.
        ///     Key2: ID of the ServiceObjects Instance, encoded as a GUID
        ///     Value: Service object.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, ServiceObject>> _serviceObjects;

        private readonly IMessenger _messenger;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new AscServiceApplication object.
        /// </summary>
        /// <param name="ascServer">Underlying IAscServer object to accept and manage client connections</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if ascServer argument is null</exception>
        public AscServiceApplication(IAscServer ascServer) {
            if (ascServer == null) throw new ArgumentNullException("ascServer");

            _ascServer = ascServer;
            _messenger = _ascServer.GetMessenger();
           // _messenger.MessageReceived += SASControl_OnMessageReceived;
            //_messenger.MessageReceived += Client_MessageReceived;
            _messenger.MessageReceived += Msg_Received;

            _serviceObjects = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, ServiceObject>>();
        }



        #endregion

        #region Public methods

        /// <summary>
        ///     Starts service application.
        /// </summary>
        public void Start() {
            _ascServer.Start();
        }

        /// <summary>
        ///     Stops service application.
        /// </summary>
        public void Stop() {
            _ascServer.Stop();
        }

        /// <summary>
        ///     Adds a service object to this service application.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be delivered from AscService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if service argument is null</exception>
        /// <exception cref="Exception">Throws Exception if service is already added before</exception>
		public void AddService<TServiceInterface, TServiceClass>(TServiceClass service, Type typeOfTServiceInterface = null)
            where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class {
            if (service == null) throw new ArgumentNullException("service");

			if (typeOfTServiceInterface == null) {
				typeOfTServiceInterface = typeof(TServiceInterface);
			}

            // check if service is cacheable
            var cacheableService = service as ICacheable;

			var serviceDict = _serviceObjects.GetOrAdd (typeOfTServiceInterface.Name, new ConcurrentDictionary<Guid, ServiceObject> ());
            if (cacheableService != null) {
				serviceDict.GetOrAdd (service.ServiceID, new CacheableServiceObject (typeOfTServiceInterface, service));
            }
            else {
				serviceDict.GetOrAdd (service.ServiceID, new ServiceObject (typeOfTServiceInterface, service));
            }
        }

		public TServiceInterface GetServiceByID<TServiceInterface, TServiceClass> (Guid id, string typeName = "")
			where TServiceClass : AscService, TServiceInterface
			where TServiceInterface : class
		{
			if (typeName == "") {
				typeName = typeof(TServiceInterface).Name;
			}
			return (TServiceClass)_serviceObjects [typeName] [id].Service;
		}

		public bool ContainsService<TServiceInterface, TServiceClass> (Guid id, string typeName = "")
			where TServiceClass : AscService, TServiceInterface
			where TServiceInterface : class
		{
			if (typeName == "") {
				typeName = typeof(TServiceInterface).Name;
			}
		    return _serviceObjects.ContainsKey(typeName) && _serviceObjects [typeName].ContainsKey (id);
		}

        /// <summary>
        ///     Removes a previously added service object from this service application.
        ///     It removes object according to interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <returns>True: removed. False: no service object with this interface</returns>
        public bool RemoveService<TServiceInterface>()
            where TServiceInterface : class
        {
            ConcurrentDictionary<Guid, ServiceObject> bla;
            return _serviceObjects.TryRemove(typeof (TServiceInterface).Name, out bla);
        }

        public bool RemoveService<TServiceInterface>(Guid serviceGuid) where TServiceInterface : class
        {
            ServiceObject bla;
            return _serviceObjects[typeof (TServiceInterface).Name].TryRemove(serviceGuid, out bla);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Casts received messages, and fires corresponding events if messages
        /// SAS control messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SASControl_OnMessageReceived(object sender, MessageEventArgs e)
        {
            var addShadowAgentMessage = e.Message as AddShadowAgentMessage;
            if (addShadowAgentMessage != null)
            {
                var handler = AddShadowAgentMessageReceived;
                if (handler != null) handler(this, new AddShadowAgentEventArgs(addShadowAgentMessage));
            }


            var removeShadowAgentMessage = e.Message as RemoveShadowAgentMessage;
            if (removeShadowAgentMessage != null)
            {
                var handler = RemoveShadowAgentMessageReceived;
                if (handler != null) handler(this, new RemoveShadowAgentEventArgs(removeShadowAgentMessage));
            }

        }

        private void ProcessRemoteInvokeMessage(IAscMessage msg) {
 
            //Cast message to AscRemoteInvokeMessage and check it
            var invokeMessage = msg as AscRemoteInvokeMessage;
            if (invokeMessage == null) return;

            try
            {
                // check whether we are responsible for the real object
                if (!_serviceObjects[invokeMessage.ServiceInterfaceName].ContainsKey(invokeMessage.ServiceID))
                {
                    // we are not. So simply return. This is not an error condition, since the ServiceID field was set.
                    return;

                }

                //Get service object
                ServiceObject serviceObject;
                if (invokeMessage.ServiceID.Equals(Guid.Empty))
                {
                    // we are not looking for a specific implementation, but just for any, so use first found
                    serviceObject = _serviceObjects[invokeMessage.ServiceInterfaceName].First().Value;
                }
                else
                {
                    serviceObject = _serviceObjects[invokeMessage.ServiceInterfaceName][invokeMessage.ServiceID];
                }

                if (serviceObject == null)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException("There is no service with name '" + invokeMessage.ServiceInterfaceName + "'"));
                    return;
                }

                //Invoke method
                try
                {
                    // store RequestReplyMessenger in ServiceObject to publish changes in its properties
                    /*var cacheableServiceObject = serviceObject as CacheableServiceObject;
                    if (cacheableServiceObject != null)
                        cacheableServiceObject.AddClient(client.ClientId, requestReplyMessenger.Messenger);
                    */
                    object returnValue;

                    try
                    {
                        returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);

                    }
                    finally
                    {
                        //Set CurrentClient as null since method call completed
                        //serviceObject.Service.CurrentClient = null;
                    }

                    //Send method invocation return value to the client
                    SendInvokeResponse(_messenger, invokeMessage, returnValue, null);
                }
                catch (TargetInvocationException ex)
                {
                    var innerEx = ex.InnerException;
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException(
                            innerEx.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, innerEx));
                }
                catch (Exception ex)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException(
                            ex.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex)
            {
                SendInvokeResponse(_messenger, invokeMessage, null,
                    new AcsRemoteException("An error occured during remote service method call.", ex));
            }
            
        }

        /// <summary>
        ///     Handles MessageReceived events of all clients, evaluates each message,
        ///     finds appropriate service object and invokes appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_MessageReceived(object sender, MessageEventArgs e) {

        }

        private void Msg_Received(object sender, MessageEventArgs e)
        {

            //Cast message to AscRemoteInvokeMessage and check it
            var invokeMessage = e.Message as AscRemoteInvokeMessage;
            if (invokeMessage == null) return;
            // TODO: Why on earth does this error not get thrown?
            /*{
                SendInvokeResponse(_messenger, invokeMessage, null,
                    new AcsRemoteException("The received message was not of type AscRemoteInvokeMessage"));
            } */

            try
            {

				// check whether we are responsible for the real object
				if (!_serviceObjects[invokeMessage.ServiceInterfaceName].ContainsKey(invokeMessage.ServiceID))
				{
					// we are not. So simply return. This is not an error condition, since the ServiceID field was set.
					return;

				}


                //Get service object
                ServiceObject serviceObject;
                if (invokeMessage.ServiceID.Equals(Guid.Empty))
                {
                    // we are not looking for a specific implementation, but just for any, so use first found
                    serviceObject = _serviceObjects[invokeMessage.ServiceInterfaceName].First().Value;
                }
                else
                {
                    serviceObject = _serviceObjects[invokeMessage.ServiceInterfaceName][invokeMessage.ServiceID];
                }

                if (serviceObject == null)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException("There is no service with name '" + invokeMessage.ServiceInterfaceName + "'"));
                    return;
                }

                //Invoke method
                try
                {
                    // store RequestReplyMessenger in ServiceObject to publish changes in its properties
                    /*var cacheableServiceObject = serviceObject as CacheableServiceObject;
                    if (cacheableServiceObject != null)
                        cacheableServiceObject.AddClient(client.ClientId, requestReplyMessenger.Messenger);
                    */
                    object returnValue;

                    try
                    {
                        returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);

                    }
                    finally
                    {
                        //Set CurrentClient as null since method call completed
                        //serviceObject.Service.CurrentClient = null;
                    }

                    //Send method invocation return value to the client
                    SendInvokeResponse(_messenger, invokeMessage, returnValue, null);
                }
                catch (TargetInvocationException ex)
                {
                    var innerEx = ex.InnerException;
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException(
                            innerEx.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, innerEx));
                }
                catch (Exception ex)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new AcsRemoteException(
                            ex.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex)
            {
                SendInvokeResponse(_messenger, invokeMessage, null,
                    new AcsRemoteException("An error occured during remote service method call.", ex));
            }
        }


        /// <summary>
        ///     Sends response to the remote application that invoked a service method.
        /// </summary>
        /// <param name="client">Client that sent invoke message</param>
        /// <param name="requestMessage">Request message</param>
        /// <param name="returnValue">Return value to send</param>
        /// <param name="exception">Exception to send</param>
        private static void SendInvokeResponse(IMessenger client, AscRemoteInvokeMessage requestMessage, object returnValue,
            AcsRemoteException exception)
        {
            client.SendMessage(
                new AscRemoteInvokeReturnMessage {
                    RepliedMessageId = requestMessage.MessageId,
                    ReturnValue = returnValue,
                    RemoteException = exception,
                    ServiceID = requestMessage.ServiceID
                });
        }

        #endregion

        #region ServiceObject class

        /// <summary>
        ///     Represents a user service object.
        ///     It is used to invoke methods on a AscService object.
        /// </summary>
        private class ServiceObject {
            /// <summary>
            ///     The service object that is used to invoke methods on.
            /// </summary>
            public AscService Service { get; private set; }

            /// <summary>
            ///     AscService attribute of Service object's class.
            /// </summary>
            public AscServiceAttribute ServiceAttribute { get; private set; }

            /// <summary>
            ///     This collection stores a list of all methods of service object.
            ///     Key: Method name
            ///     Value: Informations about method.
            /// </summary>
            private readonly SortedList<string, MethodInfo> _methods;

            /// <summary>
            ///     Creates a new ServiceObject.
            /// </summary>
            /// <param name="serviceInterfaceType">Type of service interface</param>
            /// <param name="service">The service object that is used to invoke methods on</param>
            public ServiceObject(Type serviceInterfaceType, AscService service) {
                Service = service;
                var classAttributes = serviceInterfaceType.GetTypeInfo().GetCustomAttributes(typeof (AscServiceAttribute), true).ToArray();
                if (classAttributes.Length <= 0) {
                    throw new Exception("Service interface (" + serviceInterfaceType.Name +
                                        ") must have AscService attribute.");
                }

                ServiceAttribute = classAttributes[0] as AscServiceAttribute;
                _methods = new SortedList<string, MethodInfo>();
                foreach (var methodInfo in serviceInterfaceType.GetTypeInfo().GetMethods())
                {
                    // store with name + param count to allow for overloaded methods
                    _methods.Add(methodInfo.Name + methodInfo.GetParameters().Length, methodInfo);
                }
            }

            /// <summary>
            ///     Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters) {
                var sig = methodName + parameters.Length;
                //Check if there is a method with name methodName
                if (!_methods.ContainsKey(sig))
                    throw new Exception("There is not a method with name '" + methodName + "' in service class.");

                //Get method
                var method = _methods[sig];

                //Invoke method and return invoke result
                return method.Invoke(Service, parameters);
            }
        }

		/// <summary>
		/// A Cacheable service object. It registers client's listenting to it.
		/// This allows to push updates of fields to the clients, when they occur.
		/// </summary>
        private sealed class CacheableServiceObject : ServiceObject {
            private static ConcurrentDictionary<long, IMessenger> _clients;
            private readonly IDictionary<string, PropertyInfo> _properties;

            public CacheableServiceObject(Type serviceInterfaceType, AscService service)
                : base(serviceInterfaceType, service) {
                _clients = new ConcurrentDictionary<long, IMessenger>();

                _properties = new Dictionary<string, PropertyInfo>();
                foreach (var propertyInfo in serviceInterfaceType.GetTypeInfo().GetProperties()) {
                    _properties.Add(propertyInfo.Name, propertyInfo);
                }

                var propChanger = service as ICacheable;
                if (propChanger != null) propChanger.PropertyChanged += PropChangerOnPropertyChanged;
            }


            /// <summary>
            ///     Send PropertyChangedMessage to all subscribed clients
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="propertyChangedEventArgs"></param>
            private void PropChangerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs) {
                // send PropertyChangedMessage to all subscribed clients
                Parallel.ForEach(_clients.Values, messenger => {
                    var newValue = _properties[propertyChangedEventArgs.PropertyName].GetGetMethod()
                        .Invoke(Service, null);


                    try {
                        messenger.SendMessage(new PropertyChangedMessage(newValue,
                            _properties[propertyChangedEventArgs.PropertyName].GetGetMethod().Name));
                    }
                    catch {
                        // suppress all exceptions on purpose, since it might happen, that clients have disconnected meanwhile
                        // The send command will fail in that case, but that's ok.
                    }
                });
            }

            /// <summary>
            ///     Catches the event of a client disconnect and removes its reference from this
            ///     CacheableServiceObject's client list.
            /// </summary>
            /// <param name="sender"></param>
            /// <param name="e"></param>
            public static void CacheableObject_OnClientDisconnected(object sender, ServerClientEventArgs e) {
                if (_clients == null) return;
                if (_clients.ContainsKey(e.Client.ClientId))
                {
                    IMessenger bla;
                    _clients.TryRemove(e.Client.ClientId, out bla);
                }
            }

            /// <summary>
            ///     Add a client to the CacheableObject's client list.
            ///     This method is thread-safe.
            /// </summary>
            /// <param name="client"></param>
            public void AddClient(long clientID, IMessenger client) {
                _clients[clientID] = client;
            }
        }

        #endregion
    }
}