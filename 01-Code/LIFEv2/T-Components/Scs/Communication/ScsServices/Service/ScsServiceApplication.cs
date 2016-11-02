//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.Scs.Server;
using Hik.Communication.ScsServices.Communication.Messages;

namespace Hik.Communication.ScsServices.Service {
    /// <summary>
    ///     Implements IScsServiceApplication and provides all functionallity.
    /// </summary>
    internal class ScsServiceApplication : IScsServiceApplication {
        #region Public events

        /// <summary>
        ///     This event is raised when a new client connected to the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientConnected;

        /// <summary>
        ///     This event is raised when a client disconnected from the service.
        /// </summary>
        public event EventHandler<ServiceClientEventArgs> ClientDisconnected;

        #endregion

        #region Private fields

        /// <summary>
        ///     Underlying IScsServer object to accept and manage client connections.
        /// </summary>
        private readonly IScsServer _scsServer;

        /// <summary>
        ///     User service objects that is used to invoke incoming method invocation requests.
        ///     Key1: Service interface type's name.
        ///     Key2: ID of the ServiceObjects Instance, encoded as a GUID
        ///     Value: Service object.
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, ServiceObject>> _serviceObjects;

        /// <summary>
        ///     All connected clients to service.
        ///     Key: Client's unique Id.
        ///     Value: Reference to the client.
        /// </summary>
        private readonly ConcurrentDictionary<long, IScsServiceClient> _serviceClients;

        #endregion

        #region Constructors

        /// <summary>
        ///     Creates a new ScsServiceApplication object.
        /// </summary>
        /// <param name="scsServer">Underlying IScsServer object to accept and manage client connections</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if scsServer argument is null</exception>
        public ScsServiceApplication(IScsServer scsServer) {
            if (scsServer == null) throw new ArgumentNullException("scsServer");

            _scsServer = scsServer;
            _scsServer.ClientConnected += ScsServer_ClientConnected;
            _scsServer.ClientDisconnected += ScsServer_ClientDisconnected;
            _scsServer.ClientDisconnected += CacheableServiceObject.CacheableObject_OnClientDisconnected;
            _serviceObjects = new ConcurrentDictionary<string, ConcurrentDictionary<Guid, ServiceObject>>();
            _serviceClients = new ConcurrentDictionary<long, IScsServiceClient>();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Starts service application.
        /// </summary>
        public void Start() {
            _scsServer.Start();
        }

        /// <summary>
        ///     Stops service application.
        /// </summary>
        public void Stop() {
            _scsServer.Stop();
        }

        /// <summary>
        ///     Adds a service object to this service application.
        ///     Only single service object can be added for a service interface type.
        /// </summary>
        /// <typeparam name="TServiceInterface">Service interface type</typeparam>
        /// <typeparam name="TServiceClass">
        ///     Service class type. Must be delivered from ScsService and must implement
        ///     TServiceInterface.
        /// </typeparam>
        /// <param name="service">An instance of TServiceClass.</param>
        /// <exception cref="ArgumentNullException">Throws ArgumentNullException if service argument is null</exception>
        /// <exception cref="Exception">Throws Exception if service is already added before</exception>
        public void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : ScsService, TServiceInterface
            where TServiceInterface : class {
            if (service == null) throw new ArgumentNullException("service");

            var type = typeof (TServiceInterface);

            // check if service is cacheable
            var cacheableService = service as ICacheable;
            if (cacheableService != null) {
                if (_serviceObjects.ContainsKey(type.Name))
                    _serviceObjects[type.Name][service.ServiceID] = new CacheableServiceObject(type, service);
                else {
                    _serviceObjects[type.Name] = new ConcurrentDictionary<Guid, ServiceObject>();
                    _serviceObjects[type.Name][service.ServiceID] = new CacheableServiceObject(type, service);
                }
            }
            else {
                if (_serviceObjects.ContainsKey(type.Name))
                    _serviceObjects[type.Name][service.ServiceID] = new ServiceObject(type, service);
                else {
                    _serviceObjects[type.Name] = new ConcurrentDictionary<Guid, ServiceObject>();
                    _serviceObjects[type.Name][service.ServiceID] = new ServiceObject(type, service);
                }
            }
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

        #endregion

        #region Private methods

        /// <summary>
        ///     Handles ClientConnected event of _scsServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ScsServer_ClientConnected(object sender, ServerClientEventArgs e) {
            var requestReplyMessenger = new RequestReplyMessenger<IScsServerClient>(e.Client);
            requestReplyMessenger.MessageReceived += Client_MessageReceived;
            requestReplyMessenger.Start();


            var serviceClient = ScsServiceClientFactory.CreateServiceClient(e.Client, requestReplyMessenger);
            _serviceClients[serviceClient.ClientId] = serviceClient;
            OnClientConnected(serviceClient);
        }

        /// <summary>
        ///     Handles ClientDisconnected event of _scsServer object.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void ScsServer_ClientDisconnected(object sender, ServerClientEventArgs e) {
            var serviceClient = _serviceClients[e.Client.ClientId];
            if (serviceClient == null) return;

            IScsServiceClient bla;
            _serviceClients.TryRemove(e.Client.ClientId, out bla);
            OnClientDisconnected(serviceClient);
        }

        /// <summary>
        ///     Handles MessageReceived events of all clients, evaluates each message,
        ///     finds appropriate service object and invokes appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_MessageReceived(object sender, MessageEventArgs e) {
            //Get RequestReplyMessenger object (sender of event) to get client
            var requestReplyMessenger = (RequestReplyMessenger<IScsServerClient>) sender;

            //Cast message to ScsRemoteInvokeMessage and check it
            var invokeMessage = e.Message as ScsRemoteInvokeMessage;
            if (invokeMessage == null) return;

            try {
                //Get client object
                var client = _serviceClients[requestReplyMessenger.Messenger.ClientId];
                if (client == null) {
                    requestReplyMessenger.Messenger.Disconnect();
                    return;
                }

                //Get service object
                ServiceObject serviceObject;
                if (invokeMessage.ServiceID.Equals(Guid.Empty)) {
                    // we are not looking for a specific implementation, but just for any, so use first found
                    serviceObject = _serviceObjects[invokeMessage.ServiceClassName].First().Value;
                }
                else serviceObject = _serviceObjects[invokeMessage.ServiceClassName][invokeMessage.ServiceID];

                if (serviceObject == null) {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                        new ScsRemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
                    return;
                }

                //Invoke method
                try {
                    // store RequestReplyMessenger in ServiceObject to publish changes in its properties
                    var cacheableServiceObject = serviceObject as CacheableServiceObject;
                    if (cacheableServiceObject != null)
                        cacheableServiceObject.AddClient(client.ClientId, requestReplyMessenger.Messenger);

                    object returnValue;
                    //Set client to service, so user service can get client
                    //in service method using CurrentClient property.
                    serviceObject.Service.CurrentClient = client;
                    try {
                        Console.Error.WriteLine($"Calling method : {invokeMessage.MethodName}");
                        returnValue = serviceObject.InvokeMethod(invokeMessage.MethodName, invokeMessage.Parameters);
                    }
                    finally {
                        //Set CurrentClient as null since method call completed
                        serviceObject.Service.CurrentClient = null;
                    }

                    //Send method invocation return value to the client
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, returnValue, null);
                }
                catch (TargetInvocationException ex)
                {
                    var innerEx = ex.InnerException;
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,

                        new ScsRemoteException(
                            innerEx.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, innerEx));
                }
                catch (Exception ex) {
                    SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                        new ScsRemoteException(
                            ex.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex) {
                SendInvokeResponse(requestReplyMessenger, invokeMessage, null,
                    new ScsRemoteException("An error occured during remote service method call.", ex));
            }
        }


        /// <summary>
        ///     Sends response to the remote application that invoked a service method.
        /// </summary>
        /// <param name="client">Client that sent invoke message</param>
        /// <param name="requestMessage">Request message</param>
        /// <param name="returnValue">Return value to send</param>
        /// <param name="exception">Exception to send</param>
        private static void SendInvokeResponse(IMessenger client, IScsMessage requestMessage, object returnValue,
            ScsRemoteException exception)
        {
            client.SendMessage(
                new ScsRemoteInvokeReturnMessage {
                    RepliedMessageId = requestMessage.MessageId,
                    ReturnValue = returnValue,
                    RemoteException = exception
                });
        }

        /// <summary>
        ///     Raises ClientConnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientConnected(IScsServiceClient client) {
            var handler = ClientConnected;
            if (handler != null) handler(this, new ServiceClientEventArgs(client));
        }

        /// <summary>
        ///     Raises ClientDisconnected event.
        /// </summary>
        /// <param name="client"></param>
        private void OnClientDisconnected(IScsServiceClient client) {
            var handler = ClientDisconnected;
            if (handler != null) handler(this, new ServiceClientEventArgs(client));
        }

        #endregion

        #region ServiceObject class

        /// <summary>
        ///     Represents a user service object.
        ///     It is used to invoke methods on a ScsService object.
        /// </summary>
        private class ServiceObject {
            /// <summary>
            ///     The service object that is used to invoke methods on.
            /// </summary>
            public ScsService Service { get; private set; }

            /// <summary>
            ///     ScsService attribute of Service object's class.
            /// </summary>
            public ScsServiceAttribute ServiceAttribute { get; private set; }

            /// <summary>
            ///     This collection stores a list of all methods of service object.
            ///     Key: Method name
            ///     Value: Informations about method.
            /// </summary>
            private readonly SortedList<string, MethodInfo> _methods;
            private readonly SortedList<string, MethodInfo> _internalMethods;

            /// <summary>
            ///     Creates a new ServiceObject.
            /// </summary>
            /// <param name="serviceInterfaceType">Type of service interface</param>
            /// <param name="service">The service object that is used to invoke methods on</param>
            public ServiceObject(Type serviceInterfaceType, ScsService service) {
                Service = service;
                var classAttributes = serviceInterfaceType.GetTypeInfo().GetCustomAttributes(typeof (ScsServiceAttribute), true).ToArray();
                if (!classAttributes.Any()) {
                    throw new Exception("Service interface (" + serviceInterfaceType.Name +
                                        ") must have ScsService attribute.");
                }

                ServiceAttribute = classAttributes[0] as ScsServiceAttribute;
                _methods = new SortedList<string, MethodInfo>();
                foreach (var methodInfo in serviceInterfaceType.GetTypeInfo().GetMethods()) {
                    // store with name + param count to allow for overloaded methods
                    _methods.Add(methodInfo.Name+methodInfo.GetParameters().Length, methodInfo);
                }
                _internalMethods = new SortedList<string, MethodInfo>();
                foreach (var methodInfo in service.GetType().GetTypeInfo().GetMethods())
                {
                    var sig = methodInfo.Name + methodInfo.GetParameters().Length;
                    if (_internalMethods.ContainsKey(sig))
                    {
                        continue;
                    }
                    _internalMethods.Add(sig, methodInfo);
                }
            }


            /// <summary>
            ///     Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters)
            {
                MethodInfo method = null;
                var sig = methodName + parameters.Length;
                //Check if there is a method with name methodName
                if (!_methods.ContainsKey(sig))
                {
                    if (!_internalMethods.ContainsKey(sig))
                    {
                        throw new Exception("There is not a method with name '" + methodName + "' in service class.");
                    }
                    method = _internalMethods[sig];
                }
                else
                {
                    method = _methods[sig];
                }
                
                //Invoke method and return invoke result
                return method.Invoke(Service, parameters);
            }
        }

        private sealed class CacheableServiceObject : ServiceObject {
            private static ConcurrentDictionary<long, IMessenger> _clients;
            private readonly IDictionary<string, PropertyInfo> _properties;

            public CacheableServiceObject(Type serviceInterfaceType, ScsService service)
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
                // TODO: change or adapt this for Multicast Messaging (maybe not possible...)
                Parallel.ForEach<IMessenger>(_clients.Values, messenger => {
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