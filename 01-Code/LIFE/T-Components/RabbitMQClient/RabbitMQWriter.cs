//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/
using System;
using RabbitMQ.Client;
using System.Text;
using ConfigService;
using System.Threading.Tasks;

namespace RabbitMQClient
{
	public class RabbitMQWriter
	{
		private IConnection connection;
		private IModel channel;
		private string _queueName;

		public RabbitMQWriter(Guid simulationId)
		{
			
			var cfgClient = new ConfigServiceClient("http://marsconfig:8080/");
			var ip   = cfgClient.Get("rabbitmq/ip");
			var port = cfgClient.Get("rabbitmq/port");
			var user = cfgClient.Get("rabbitmq/user");
			var pass = cfgClient.Get("rabbitmq/pass");

			var factory = new ConnectionFactory() { 
				HostName = ip, 
				Port = Int32.Parse(port),
				UserName = user,
				Password = pass
			
			};

			connection = factory.CreateConnection();
			channel = connection.CreateModel ();

			// name queue with 'life-' prefix
			_queueName = "life-" + simulationId;
			channel.QueueDeclare(queue: _queueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);
		}

		/// <summary>
		/// Sends a message via RabbitMQ
		/// </summary>
		/// <param name="message">Message.</param>
		public void SendMessage(string message){
			var body = Encoding.UTF8.GetBytes(message);

			channel.BasicPublish(exchange: "",
				routingKey: _queueName,
				basicProperties: null,
				body: body);
		}
	}
}

