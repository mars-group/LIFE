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
using CommonTypes;

namespace RabbitMQClient
{
	public class RabbitMQWriter
	{
		private IConnection connection;
		private IModel channel;
		private string _queueName;

		public RabbitMQWriter(Guid simulationId)
		{
			
			var cfgClient = new ConfigServiceClient(MARSConfigServiceSettings.Address);

			var connection = new ConnectionFactory {
				HostName = cfgClient.Get("rabbitmq/ip"),
				UserName = cfgClient.Get("rabbitmq/user"),
				Password = cfgClient.Get("rabbitmq/pass"),
				Port = int.Parse(cfgClient.Get("rabbitmq/port"))
			}.CreateConnection();
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
            try
            {
                channel.BasicPublish("", _queueName, null, body);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("[RabbitMQWriter] Propagation of new result package failed!");
                Console.Error.WriteLine(ex);
            }
		}
	}
}

