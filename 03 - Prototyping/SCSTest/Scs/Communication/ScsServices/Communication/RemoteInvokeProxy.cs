using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Proxies;
using Hik.Communication.Scs.Communication.Messages;
using Hik.Communication.Scs.Communication.Messengers;
using Hik.Communication.ScsServices.Communication.Messages;
using Hik.Communication.ScsServices.Service;

namespace Hik.Communication.ScsServices.Communication
{
    /// <summary>
    /// This class is used to generate a dynamic proxy to invoke remote methods.
    /// It translates method invocations to messaging.
    /// </summary>
    /// <typeparam name="TProxy">Type of the proxy class/interface</typeparam>
    /// <typeparam name="TMessenger">Type of the messenger object that is used to send/receive messages</typeparam>
    internal class RemoteInvokeProxy<TProxy, TMessenger> : RealProxy where TMessenger : IMessenger
    {
        /// <summary>
        /// Messenger object that is used to send/receive messages.
        /// </summary>
        private readonly RequestReplyMessenger<TMessenger> _clientMessenger;

        private List<MethodInfo> _cacheableMethods;
        private readonly Type _typeOfTProxy;

        private IDictionary<string,object> _cache;


        /// <summary>
        /// Creates a new RemoteInvokeProxy object.
        /// </summary>
        /// <param name="clientMessenger">Messenger object that is used to send/receive messages</param>
        public RemoteInvokeProxy(RequestReplyMessenger<TMessenger> clientMessenger)
            : base(typeof(TProxy))
        {
            _clientMessenger = clientMessenger;

            // subscribe for new PropertyChangedMessages. Will work since
            // SendAndWaitForReply() does not raise MessageReceived Event
            _clientMessenger.MessageReceived += ClientMessengerOnMessageReceived;

            _cache = new Dictionary<string, object>();

            _typeOfTProxy = typeof (TProxy);

            //retreive all methods which are marked as cacheable
            var methodsOfTProxy = _typeOfTProxy.GetMethods();
            _cacheableMethods = new List<MethodInfo>();

            foreach (var method in methodsOfTProxy)
            {
                var cacheable = Attribute.GetCustomAttribute(method,
                    typeof(CacheableAttribute), false) as CacheableAttribute;
                if (cacheable == null)
                    continue;
                // store methodInfos
                _cacheableMethods.Add(method);
            }

            //retreive all properties of TProxy
            var properties = _typeOfTProxy.GetProperties();
            foreach (var propertyInfo in properties)
            {
                _cacheableMethods.Add(propertyInfo.GetGetMethod());
            }
        }

        private void ClientMessengerOnMessageReceived(object sender, MessageEventArgs messageEventArgs)
        {
            var msg = messageEventArgs.Message as PropertyChangedMessage;
            if (msg != null)
            {
                _cache[msg.PropertyGetMethodName] = msg.NewValue;
            }
        }

        /// <summary>
        /// Overrides message calls and translates them to messages to remote application.
        /// </summary>
        /// <param name="msg">Method invoke message (from RealProxy base class)</param>
        /// <returns>Method invoke return message (to RealProxy base class)</returns>
        public override IMessage Invoke(IMessage msg)
        {
            var message = msg as IMethodCallMessage;
            if (message == null)
            {
                return null;
            }
            // TODO Extend to support additional parameter for target agent.

            // Answer request from cache if available
            if (_cache.ContainsKey(message.MethodName))
            {
                return new ReturnMessage(_cache[message.MethodName], null, 0, message.LogicalCallContext, message);
            }

            var requestMessage = new ScsRemoteInvokeMessage
            {
                ServiceClassName = _typeOfTProxy.Name,
                MethodName = message.MethodName,
                Parameters = message.InArgs
            };

            var responseMessage = _clientMessenger.SendMessageAndWaitForResponse(requestMessage) as ScsRemoteInvokeReturnMessage;
            if (responseMessage == null)
            {
                return null;
            }

            // Store result in cache if no exception was thrown and the method has been marked as cacheable
            if (responseMessage.RemoteException == null && _cacheableMethods.Exists(m => m.Name.Equals(message.MethodName)))
            {
                if (!_cache.ContainsKey(message.MethodName))
                {
                    _cache.Add(message.MethodName, responseMessage.ReturnValue);
                }
                else
                {
                    _cache[message.MethodName] = responseMessage.ReturnValue;
                }

            }

            return responseMessage.RemoteException != null
                       ? new ReturnMessage(responseMessage.RemoteException, message)
                       : new ReturnMessage(responseMessage.ReturnValue, null, 0, message.LogicalCallContext, message);
        }
    }
}