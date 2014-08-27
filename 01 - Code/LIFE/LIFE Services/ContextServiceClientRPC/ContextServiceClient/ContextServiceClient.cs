using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ContextServiceClient
{
	public delegate void MethodDelegate();
	class ContextServiceClient
	{
		private IConnection connection;
		private IModel rpcChannel;
		private IModel eventChannel;
		private string replyQueueName;
		private QueueingBasicConsumer consumer;
		private ContextListener contextListener;
		//public Hashtable contextDelegates = new Hashtable();

		public ContextServiceClient()
		{
			// Connection for RPC
			var factory = new ConnectionFactory() { HostName = "localhost" };
			connection = factory.CreateConnection();
			rpcChannel = connection.CreateModel();
			replyQueueName = rpcChannel.QueueDeclare();
			consumer = new QueueingBasicConsumer(rpcChannel);
			rpcChannel.BasicConsume(replyQueueName, null, consumer);

			// Connection for outgoing Events
			eventChannel = connection.CreateModel();
			eventChannel.QueueDeclare("contextservice_in");

			// Start Listener for incoming Events
			contextListener = new ContextListener();
		}

		public string Call(string message)
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
				if (ea.BasicProperties.CorrelationId == corrId)
				{
					return Encoding.UTF8.GetString(ea.Body);
				}
			}
		}

		public void Close()
		{
			connection.Close();
		}

		public void RegisterNewEventType(Object obj)
		{
			int i = 1;
			string newEventTypeJSON = "";
			Type t = obj.GetType();
			PropertyInfo[] pi = t.GetProperties();
			Console.WriteLine("ClassName: {0} AttributesCount: {1}", t.Name, pi.Length);
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
			Console.WriteLine("{0}", message);
			this.Call (message);
		}

		public void RegisterNewContextRule(string contextRule, MethodDelegate method)
		{
			string message = string.Format ("1;{0}", contextRule);
			Console.WriteLine("{0}", message);
			string ruleID = this.Call(message);
			Console.WriteLine("Received ruleID: {0}", ruleID);
			//contextListener.contextDelegates.Add (ruleID, method);
			contextListener.contextRuleDictionary.Add (ruleID, method);
		}

		public void SendEvent(Object obj)
		{
			int i = 1;
			string newEventJSON = "";
			Type t = obj.GetType();
			PropertyInfo[] pi = t.GetProperties();
			Console.WriteLine("ClassName: {0} AttributesCount: {1}", t.Name, pi.Length);
			newEventJSON = "{\"EventType\":\"";
			newEventJSON += t.Name;
			newEventJSON += "\",\"AttributesCount\":\"";
			newEventJSON += pi.Length;
			newEventJSON += "\",\"Attributes\":[";
			foreach (PropertyInfo prop in pi) {
				newEventJSON += "{\"AttributeName\":\"";
				newEventJSON += prop.Name;
				newEventJSON += "\",\"AttributeType\":\"";
				newEventJSON += prop.PropertyType.Name;
				newEventJSON += "\",\"AttributeValue\":\"";
				if(prop.PropertyType.Name.Equals("Double")) {
					double doubleValue = (double)prop.GetValue(obj, null);
					newEventJSON += doubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
				}
				else {
					newEventJSON += prop.GetValue(obj, null);
				}
				newEventJSON += "\"}";
				if ((pi.Length > 1) && (i < pi.Length)) {
					newEventJSON += ",";
				}
				i++;
			}
			newEventJSON += "]}";
			string message = string.Format ("0;{0}", newEventJSON);
			Console.WriteLine("NewEventJSON: {0}", message);
			//this.Call (message);

			var body = Encoding.UTF8.GetBytes(message);

			eventChannel.BasicPublish("", "contextservice_in", null, body);
			Console.WriteLine(" [x] Sent {0}", message);
		}
	}
}

