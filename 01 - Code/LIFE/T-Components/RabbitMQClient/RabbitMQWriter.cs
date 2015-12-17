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
			var queueName = "life-" + simulationId;
			channel.QueueDeclare(queue: queueName,
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
				routingKey: "",
				basicProperties: null,
				body: body);
		}

		/// <summary>
		/// Sends a message via RabbitMQ asynchronously
		/// </summary>
		/// <param name="message">Message.</param>
		public async Task SendMessageAsync(string message){
			await Task.Run (() => {
				var body = Encoding.UTF8.GetBytes (message);

				channel.BasicPublish (exchange: "",
					routingKey: "",
					basicProperties: null,
					body: body);
			});
		}
	}
}

