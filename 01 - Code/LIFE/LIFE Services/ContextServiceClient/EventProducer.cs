using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Reflection;
using System.Collections;

namespace ContextServiceClient
{
	public class EventProducer
	{
		private IConnection connection;
		private IModel eventChannel;
		private string queueName;

		public EventProducer()
		{
			// Connection to message broker
			var factory = new ConnectionFactory () { 
				HostName = ContextServiceClient.Instance.Host, 
				Port = ContextServiceClient.Instance.Port 
			};
			connection = factory.CreateConnection();

			// Declare new message queue for outgoing Events
			eventChannel = connection.CreateModel();
			queueName = string.Format ("queue{0}", ContextServiceClient.Instance.RegisterNewEventProducer());
			//eventChannel.QueueDeclare(queueName);
		}

		public void SendEvent(Object obj)
		{
			int i = 1;
			string newEventJSON = "";
			Type t = obj.GetType();
			PropertyInfo[] pi = t.GetProperties();
			//Console.WriteLine("ClassName: {0} AttributesCount: {1}", t.Name, pi.Length);
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
				if((prop.PropertyType.Name.Equals("Single")) || (prop.PropertyType.Name.Equals("Double"))) {
					if (prop.PropertyType.Name.Equals ("Single")) {
						float floatValue = (float)prop.GetValue(obj, null);
						newEventJSON += floatValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
					} else {
						double doubleValue = (double)prop.GetValue(obj, null);
						newEventJSON += doubleValue.ToString(System.Globalization.CultureInfo.InvariantCulture);
					}
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
			//Console.WriteLine("NewEventJSON: {0}", message);

			var body = Encoding.UTF8.GetBytes(message);

			eventChannel.BasicPublish("", queueName, null, body);
			//Console.WriteLine(" [x] Sent {0}", message);
		}

		public void Disconnect() {
			eventChannel.Close();
			connection.Close();
		}
	}
}

