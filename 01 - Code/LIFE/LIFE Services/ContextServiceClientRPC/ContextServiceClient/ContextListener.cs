using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;


namespace ContextServiceClient
{
	public delegate void ListenerDelegate();
	public class ContextListener
	{
		private IConnection connection;
		private IModel channel;
		private QueueingBasicConsumer consumer;
		public Hashtable contextDelegates = new Hashtable();
		public Dictionary<string, MethodDelegate> contextRuleDictionary;

		public ContextListener ()
		{
			contextRuleDictionary = new Dictionary<string, MethodDelegate>();

			var factory = new ConnectionFactory () { 
				HostName = ContextServiceClient.Instance.Host, 
				Port = ContextServiceClient.Instance.Port 
			};
			connection = factory.CreateConnection ();
			channel = connection.CreateModel ();

			consumer = new QueueingBasicConsumer (channel);
			channel.BasicConsume ("contextservice_out", null, consumer);

			ThreadStart ts = new ThreadStart (run);
			Thread thread = new Thread (ts);
			thread.Start ();

			Console.WriteLine (" [*] Context Listener is waiting for messages.");
		}

		public void run() {
			while (true)
			{
				var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

				var body = ea.Body;
				string message = Encoding.UTF8.GetString(body);
				string[] result = message.Split (';');
				channel.BasicAck (ea.DeliveryTag, false);

				if(contextRuleDictionary.ContainsKey(result[0]))
				{
					Dictionary<string, object> results = JsonConvert.DeserializeObject<Dictionary<string, object>>(result[1]);
				
					contextRuleDictionary[result[0]].Invoke(results);
				}
			}
		}
	}
}

