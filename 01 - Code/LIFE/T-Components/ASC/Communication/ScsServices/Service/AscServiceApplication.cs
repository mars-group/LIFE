using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ASC.Communication.Scs.Communication.Messages;
using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Server;
using ASC.Communication.ScsServices.Communication.Messages;
using CustomUtilities.Collections;
using CustomUtilities.Threading;

namespace ASC.Communication.ScsServices.Service {
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
        private readonly ThreadSafeSortedList<string, ThreadSafeSortedList<Guid, ServiceObject>> _serviceObjects;

        private readonly SequentialItemProcessor<IAscMessage> _incomingMessageProcessor;
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

            //_incomingMessageProcessor = new SequentialItemProcessor<IAscMessage>(ProcessRemoteInvokeMessage);
            //_incomingMessageProcessor.Start();

            _ascServer = ascServer;
            _messenger = _ascServer.GetMessenger();
           // _messenger.MessageReceived += SASControl_OnMessageReceived;
            //_messenger.MessageReceived += Client_MessageReceived;
            _messenger.MessageReceived += Msg_Received;

            _serviceObjects = new ThreadSafeSortedList<string, ThreadSafeSortedList<Guid, ServiceObject>>();
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
        public void AddService<TServiceInterface, TServiceClass>(TServiceClass service)
            where TServiceClass : AscService, TServiceInterface
            where TServiceInterface : class {
            if (service == null) throw new ArgumentNullException("service");

            var type = typeof (TServiceInterface);

            // check if service is cacheable
            var cacheableService = service as ICacheable;
            if (cacheableService != null) {
                if (_serviceObjects.ContainsKey(type.Name))
                    _serviceObjects[type.Name][service.ServiceID] = new CacheableServiceObject(type, service);
                else {
                    _serviceObjects[type.Name] = new ThreadSafeSortedList<Guid, ServiceObject>();
                    _serviceObjects[type.Name][service.ServiceID] = new CacheableServiceObject(type, service);
                }
            }
            else {
                if (_serviceObjects.ContainsKey(type.Name))
                    _serviceObjects[type.Name][service.ServiceID] = new ServiceObject(type, service);
                else {
                    _serviceObjects[type.Name] = new ThreadSafeSortedList<Guid, ServiceObject>();
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
            where TServiceInterface : class {
            return _serviceObjects.Remove(typeof (TServiceInterface).Name);
        }

        public bool RemoveService<TServiceInterface>(Guid serviceGuid) where TServiceInterface : class {
            return _serviceObjects[typeof (TServiceInterface).Name].Remove(serviceGuid);
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
                if (!_serviceObjects[invokeMessage.ServiceClassName].ContainsKey(invokeMessage.ServiceID))
                {
                    // we are not. So simply return. This is not an error condition, since the ServiceID field was set.
                    return;

                }

                //Get service object
                ServiceObject serviceObject;
                if (invokeMessage.ServiceID.Equals(Guid.Empty))
                {
                    // we are not looking for a specific implementation, but just for any, so use first found
                    serviceObject = _serviceObjects[invokeMessage.ServiceClassName].GetAllItems().First();
                }
                else
                {
                    serviceObject = _serviceObjects[invokeMessage.ServiceClassName][invokeMessage.ServiceID];
                }

                if (serviceObject == null)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new ScsRemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
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
                        new ScsRemoteException(
                            innerEx.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, innerEx));
                }
                catch (Exception ex)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new ScsRemoteException(
                            ex.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex)
            {
                SendInvokeResponse(_messenger, invokeMessage, null,
                    new ScsRemoteException("An error occured during remote service method call.", ex));
            }
            
        }

        /// <summary>
        ///     Handles MessageReceived events of all clients, evaluates each message,
        ///     finds appropriate service object and invokes appropriate method.
        /// </summary>
        /// <param name="sender">Source of event</param>
        /// <param name="e">Event arguments</param>
        private void Client_MessageReceived(object sender, MessageEventArgs e) {
            _incomingMessageProcessor.EnqueueMessage(e.Message);
        }

        private void Msg_Received(object sender, MessageEventArgs e)
        {

            //Cast message to AscRemoteInvokeMessage and check it
            var invokeMessage = e.Message as AscRemoteInvokeMessage;
            if (invokeMessage == null) return;

            try
            {
                // check whether we are responsible for the real object
                if (!_serviceObjects[invokeMessage.ServiceClassName].ContainsKey(invokeMessage.ServiceID))
                {
                    // we are not. So simply return. This is not an error condition, since the ServiceID field was set.
                    return;

                }

                //Get service object
                ServiceObject serviceObject;
                if (invokeMessage.ServiceID.Equals(Guid.Empty))
                {
                    // we are not looking for a specific implementation, but just for any, so use first found
                    serviceObject = _serviceObjects[invokeMessage.ServiceClassName].GetAllItems().First();
                }
                else
                {
                    serviceObject = _serviceObjects[invokeMessage.ServiceClassName][invokeMessage.ServiceID];
                }

                if (serviceObject == null)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new ScsRemoteException("There is no service with name '" + invokeMessage.ServiceClassName + "'"));
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
                        new ScsRemoteException(
                            innerEx.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, innerEx));
                }
                catch (Exception ex)
                {
                    SendInvokeResponse(_messenger, invokeMessage, null,
                        new ScsRemoteException(
                            ex.Message + Environment.NewLine + "Service Version: " +
                            serviceObject.ServiceAttribute.Version, ex));
                }
            }
            catch (Exception ex)
            {
                SendInvokeResponse(_messenger, invokeMessage, null,
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
        private static void SendInvokeResponse(IMessenger client, AscRemoteInvokeMessage requestMessage, object returnValue,
            ScsRemoteException exception)
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
                var classAttributes = serviceInterfaceType.GetCustomAttributes(typeof (AscServiceAttribute), true);
                if (classAttributes.Length <= 0) {
                    throw new Exception("Service interface (" + serviceInterfaceType.Name +
                                        ") must have AscService attribute.");
                }

                ServiceAttribute = classAttributes[0] as AscServiceAttribute;
                _methods = new SortedList<string, MethodInfo>();
                foreach (var methodInfo in serviceInterfaceType.GetMethods()) {
                    _methods.Add(methodInfo.Name, methodInfo);
                }
            }

            /// <summary>
            ///     Invokes a method of Service object.
            /// </summary>
            /// <param name="methodName">Name of the method to invoke</param>
            /// <param name="parameters">Parameters of method</param>
            /// <returns>Return value of method</returns>
            public object InvokeMethod(string methodName, params object[] parameters) {
                //Check if there is a method with name methodName
                if (!_methods.ContainsKey(methodName))
                    throw new Exception("There is not a method with name '" + methodName + "' in service class.");

                //Get method
                var method = _methods[methodName];

                //Invoke method and return invoke result
                return method.Invoke(Service, parameters);
            }
        }

        private sealed class CacheableServiceObject : ServiceObject {
            private static ThreadSafeSortedList<long, IMessenger> _clients;
            private readonly IDictionary<string, PropertyInfo> _properties;

            public CacheableServiceObject(Type serviceInterfaceType, AscService service)
                : base(serviceInterfaceType, service) {
                _clients = new ThreadSafeSortedList<long, IMessenger>();

                _properties = new Dictionary<string, PropertyInfo>();
                foreach (var propertyInfo in serviceInterfaceType.GetProperties()) {
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
                Parallel.ForEach(_clients.GetAllItems(), messenger => {
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
                if (_clients.ContainsKey(e.Client.ClientId)) _clients.Remove(e.Client.ClientId);
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