using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

namespace ContextServiceClient
{
	public delegate void MethodDelegate(Dictionary<string, object> results);
	class ContextServiceClient
	{
		private static ContextServiceClient instance;
		private bool connected = false;

		private IConnection connection;
		private IModel rpcChannel;
		private string replyQueueName;
		private QueueingBasicConsumer consumer;
		private ContextListener contextListener;

		private ContextServiceClient()
		{
			Host = "127.0.0.1";
			Port = 5672;
		}

		public string Host {get;set;}
		public int Port {get;set;}

		public static ContextServiceClient Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new ContextServiceClient();
				}
				return instance;
			}
		}

		public void ConnectTo(string host, int port)
		{
			if (!connected)
			{
				// Connection for RPC
				var factory = new ConnectionFactory() { HostName = host, Port = port };
				connection = factory.CreateConnection();
				rpcChannel = connection.CreateModel();
				replyQueueName = rpcChannel.QueueDeclare("rpc_reply_queue", true, false, false, null);
				consumer = new QueueingBasicConsumer(rpcChannel);
				rpcChannel.BasicConsume(replyQueueName, false, consumer);

				// Start Listener for incoming Events
				contextListener = new ContextListener();

				// Initialize Context Server
				this.Call ("4");

				connected = true;
			}
		}

		private string Call(string message)
		{
			var corrId = Guid.NewGuid().ToString();
			var props = rpcChannel.CreateBasicProperties();
			props.ReplyTo = replyQueueName;
			props.CorrelationId = corrId;

			var messageBytes = Encoding.UTF8.GetBytes(message);
			rpcChannel.BasicPublish("", "rpc_queue", props, messageBytes);

			while (true)
			{
				var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();
				rpcChannel.BasicAck (ea.DeliveryTag, false);
				if (ea.BasicProperties.CorrelationId == corrId)
				{
					return Encoding.UTF8.GetString(ea.Body);
				}
			}
		}

		public void Disconnect()
		{
			rpcChannel.Close();
			connection.Close();
		}

		public int RegisterNewEventProducer()
		{
			int registeredId = 0;

			registeredId = Convert.ToInt32(this.Call ("3"));

			return registeredId;
		}

		public void RegisterNewEventType(Object obj)
		{
			int i = 1;
			string newEventTypeJSON = "";
			Type t = obj.GetType();
			PropertyInfo[] pi = t.GetProperties();
			newEventTypeJSON = "{\"EventTypeName\":\"";
			newEventTypeJSON += t.Name;
			newEventTypeJSON += "\",\"AtributesCount\":\"";
			newEventTypeJSON += pi.Length;
			newEventTypeJSON += "\",\"Atributes\":[";
			foreach (PropertyInfo prop in pi) {
				newEventTypeJSON += "{\"AtributeName\":\"";
				newEventTypeJSON += prop.Name;
				newEventTypeJSON += "\",\"AtributeType\":\"";
				newEventTypeJSON += prop.PropertyType.Name;
				newEventTypeJSON += "\"}";
				if ((pi.Length > 1) && (i < pi.Length)) {
					newEventTypeJSON += ",";
				}
				i++;
			}
			newEventTypeJSON += "]}";
			string message = string.Format ("0;{0}", newEventTypeJSON);
			this.Call (message);
		}

		public void RegisterNewContextRule(string contextRule, MethodDelegate method)
		{
			string message = string.Format ("1;{0}", contextRule);
			string ruleID = this.Call(message);
			if(!ruleID.Equals("-1")){
				contextListener.contextRuleDictionary.Add (ruleID, method);
			}
		}
	}
}

