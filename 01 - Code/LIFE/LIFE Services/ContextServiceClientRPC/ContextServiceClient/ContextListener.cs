using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading;
using System.Collections;
using System.Collections.Generic;


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

			var factory = new ConnectionFactory () { HostName = "localhost" };
			connection = factory.CreateConnection ();
			channel = connection.CreateModel ();

			channel.QueueDeclare ("contextservice_out");

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
				var methodId = Encoding.UTF8.GetString(body);
				Console.WriteLine(" [x] Received {0}", methodId);
				//ListenerDelegate md = new ListenerDelegate(contextDelegates[message]);
				if(contextRuleDictionary.ContainsKey(methodId))
				{
					contextRuleDictionary[methodId].Invoke();
				}
			}
		}
	}
}

