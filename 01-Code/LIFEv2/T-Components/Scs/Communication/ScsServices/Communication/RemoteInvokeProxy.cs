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
using System.Reflection;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.ScsServices.Communication.Messages;
using Hik.Communication.ScsServices.Service;

namespace Hik.Communication.ScsServices.Communication
{

#if HAS_REAL_PROXY
    using System.Runtime.Remoting.Messaging;
    using System.Runtime.Remoting.Proxies;

    /// <summary>
    ///     This class is used to generate a dynamic proxy to invoke remote methods.
    ///     It translates method invocations to messaging.
    /// </summary>
    /// <typeparam name="TProxy">Type of the proxy class/interface</typeparam>
    /// <typeparam name="TMessenger">Type of the messenger object that is used to send/receive messages</typeparam>
    internal class RemoteInvokeProxy<TProxy, TMessenger> : RealProxy where TMessenger : IMessenger {
        /// <summary>
        ///     Messenger object that is used to send/receive messages.
        /// </summary>
        private readonly RequestReplyMessenger<TMessenger> _clientMessenger;

        private readonly Guid _serviceId;

        private readonly List<MethodInfo> _cacheableMethods;
        private readonly Type _typeOfTProxy;

        private readonly IDictionary<string, object> _cache;


        protected RemoteInvokeProxy(RequestReplyMessenger<TMessenger> clientMessenger)
            : base(typeof (TProxy)) {
            _clientMessenger = clientMessenger;
            // subscribe for new PropertyChangedMessages. Will work since
            // SendAndWaitForReply() does not raise MessageReceived Event
            _clientMessenger.MessageReceived += ClientMessengerOnMessageReceived;

            _cache = new Dictionary<string, object>();

            _typeOfTProxy = typeof (TProxy);

            //retreive all methods which are marked as cacheable
            var methodsOfTProxy = _typeOfTProxy.GetMethods();
            _cacheableMethods = new List<MethodInfo>();

            foreach (var method in methodsOfTProxy) {
                var cacheable = Attribute.GetCustomAttribute(method,
                    typeof (CacheableAttribute), false) as CacheableAttribute;
                if (cacheable == null)
                    continue;
                // store methodInfos
                _cacheableMethods.Add(method);
            }

            //retreive all properties of TProxy
            var properties = _typeOfTProxy.GetProperties();
            foreach (var propertyInfo in properties) {
                _cacheableMethods.Add(propertyInfo.GetGetMethod());
            }
        }

        /// <summary>
        ///     Creates a new RemoteInvokeProxy object.
        /// </summary>
        /// <param name="clientMessenger">Messenger object that is used to send/receive messages</param>
        /// <param name="serviceID"></param>
        public RemoteInvokeProxy(RequestReplyMessenger<TMessenger> clientMessenger, Guid serviceID)
            : this(clientMessenger) {
            _serviceId = serviceID;
        }

        private void ClientMessengerOnMessageReceived(object sender, MessageEventArgs messageEventArgs) {
            var msg = messageEventArgs.Message as PropertyChangedMessage;
            if (msg != null) _cache[msg.PropertyGetMethodName] = msg.NewValue;
        }

        /// <summary>
        ///     Overrides message calls and translates them to messages to remote application.
        /// </summary>
        /// <param name="msg">Method invoke message (from RealProxy base class)</param>
        /// <returns>Method invoke return message (to RealProxy base class)</returns>
        public override IMessage Invoke(IMessage msg) {
            var message = msg as IMethodCallMessage;
            if (message == null) return null;

            // TODO Extend to support additional parameter for target agent. Was soll das ???

            // Answer request from cache if available
            if (_cache.ContainsKey(message.MethodName))
                return new ReturnMessage(_cache[message.MethodName], null, 0, message.LogicalCallContext, message);

            var requestMessage = new ScsRemoteInvokeMessage {
                ServiceClassName = _typeOfTProxy.Name,
                MethodName = message.MethodName,
                Parameters = message.InArgs,
                ServiceID = _serviceId
            };

            var responseMessage =
                _clientMessenger.SendMessageAndWaitForResponse(requestMessage) as ScsRemoteInvokeReturnMessage;
            if (responseMessage == null) return null;

            // Store result in cache if no exception was thrown and the method has been marked as cacheable
            if (responseMessage.RemoteException == null &&
                _cacheableMethods.Exists(m => m.Name.Equals(message.MethodName))) {
                if (!_cache.ContainsKey(message.MethodName))
                    _cache.Add(message.MethodName, responseMessage.ReturnValue);
                else _cache[message.MethodName] = responseMessage.ReturnValue;
            }

            return responseMessage.RemoteException != null
                ? new ReturnMessage(responseMessage.RemoteException, message)
                : new ReturnMessage(responseMessage.ReturnValue, null, 0, message.LogicalCallContext, message);
        }
    }


#else
    /// <summary>
    ///     This class is used to generate a dynamic proxy to invoke remote methods.
    ///     It translates method invocations to messaging.
    /// </summary>
    /// <typeparam name="TProxy">Type of the proxy class/interface</typeparam>
    /// <typeparam name="TMessenger">Type of the messenger object that is used to send/receive messages</typeparam>
    public class RemoteInvokeProxy<TProxy, TMessenger> : DispatchProxy where TMessenger : IMessenger
    {
        /// <summary>
        ///     Messenger object that is used to send/receive messages.
        /// </summary>
        private RequestReplyMessenger<TMessenger> _clientMessenger;

        private Guid _serviceId;

        private List<MethodInfo> _cacheableMethods;
        private Type _typeOfTProxy;

        private IDictionary<string, object> _cache;

        internal bool _configured = false;

        public void Configure(RequestReplyMessenger<TMessenger> clientMessenger)
        {
            _clientMessenger = clientMessenger;
            // subscribe for new PropertyChangedMessages. Will work since
            // SendAndWaitForReply() does not raise MessageReceived Event
            _clientMessenger.MessageReceived += ClientMessengerOnMessageReceived;

            _cache = new Dictionary<string, object>();

            _typeOfTProxy = typeof(TProxy);

            //retreive all methods which are marked as cacheable
            var methodsOfTProxy = _typeOfTProxy.GetTypeInfo().GetMethods();
            _cacheableMethods = new List<MethodInfo>();

            foreach (var method in methodsOfTProxy)
            {
                var cacheable = method.GetCustomAttribute(typeof(CacheableAttribute), false) as CacheableAttribute;

                if (cacheable == null)
                    continue;
                // store methodInfos
                _cacheableMethods.Add(method);
            }

            //retreive all properties of TProxy
            var properties = _typeOfTProxy.GetTypeInfo().GetProperties();
            foreach (var propertyInfo in properties)
            {
                _cacheableMethods.Add(propertyInfo.GetGetMethod());
            }
            _configured = true;
        }

        public void Configure(RequestReplyMessenger<TMessenger> clientMessenger, Guid serviceID)
        {
            _serviceId = serviceID;
            Configure(clientMessenger);
            _configured = true;
        }

        private void ClientMessengerOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var msg = messageEventArgs.Message as PropertyChangedMessage;
            if (msg != null) _cache[msg.PropertyGetMethodName] = msg.NewValue;
        }

        /// <summary>
        ///     Overrides message calls and translates them to messages to remote application.
        /// </summary>
        /// <param name="targetMethod"></param>
        /// <param name="args"></param>
        /// <returns>Method invoke return message (to RealProxy base class)</returns>
        protected override object Invoke(MethodInfo targetMethod, object[] args)
        {
            if (targetMethod == null) return null;
            if(!_configured) { throw new Exception("Proxy not configured"); }

            // Answer request from cache if available
            if (_cache.ContainsKey(targetMethod.Name))
                return _cache[targetMethod.Name];

            var requestMessage = new ScsRemoteInvokeMessage
            {
                ServiceClassName = _typeOfTProxy.Name,
                MethodName = targetMethod.Name,
                Parameters = args,
                ServiceID = _serviceId
            };

            var responseMessage =
                _clientMessenger.SendMessageAndWaitForResponse(requestMessage) as ScsRemoteInvokeReturnMessage;
            if (responseMessage == null) return null;

            // Store result in cache if no exception was thrown and the method has been marked as cacheable
            if (responseMessage.RemoteException == null &&
                _cacheableMethods.Exists(m => m.Name.Equals(targetMethod.Name)))
            {
                if (!_cache.ContainsKey(targetMethod.Name))
                    _cache.Add(targetMethod.Name, responseMessage.ReturnValue);
                else _cache[targetMethod.Name] = responseMessage.ReturnValue;
            }

            if (responseMessage.RemoteException != null){
                throw responseMessage.RemoteException;
            } else {
                return responseMessage.ReturnValue; 
            }
        }
    }


#endif
}