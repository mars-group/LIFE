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

			//channel.QueueDeclare ("contextservice_out");

			consumer = new QueueingBasicConsumer (channel);
			channel.BasicConsume ("contextservice_out", null, consumer);

			ThreadStart ts = new ThreadStart (run);
			Thread thread = new Thread (ts);
			thread.Start ();

			Console.WriteLine (" [*] Waiting for messages.");
		}

		public void run() {
			while (true)
			{
				var ea = (BasicDeliverEventArgs)consumer.Queue.Dequeue();

				var body = ea.Body;
				string message = Encoding.UTF8.GetString(body);
				string[] result = message.Split (';');
				channel.BasicAck (ea.DeliveryTag, false);
				Console.WriteLine(" [x] Received {0}", message);

				if(contextRuleDictionary.ContainsKey(result[0]))
				{

					//Hashtable hash = new Hashtable();
					//Hashtable results = JsonConvert.DeserializeObject<Hashtable>(result[1]);
					Dictionary<string, object> results = JsonConvert.DeserializeObject<Dictionary<string, object>>(result[1]);
					//Console.WriteLine(" [x] Hashtable {0}", results);
					contextRuleDictionary[result[0]].Invoke(results);
				}
			}
		}
	}
}

