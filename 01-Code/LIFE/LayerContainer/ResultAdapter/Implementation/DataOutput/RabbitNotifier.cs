using System;
using System.Text;
using ConfigService;
using RabbitMQ.Client;

namespace ResultAdapter.Implementation.DataOutput {
  
  /// <summary>
  ///   RabbitMQ notifier for new result packets. 
  /// </summary>
  internal class RabbitNotifier {
    
    private readonly IModel _channel;    // AMQP (Advanced-Message-Queue-Protocol) channel.
    private readonly string _queueName;  // Name of the used queue.


    /// <summary>
    ///   Initialize a new Rabbit-MQ notifier.
    /// </summary>
    /// <param name="cfgClient">MARS KV client for connection properties.</param>
    public RabbitNotifier(IConfigServiceClient cfgClient) {   
      var connection = new ConnectionFactory {
        HostName = cfgClient.Get("rabbitmq/ip"),
        UserName = cfgClient.Get("rabbitmq/user"),
        Password = cfgClient.Get("rabbitmq/pass"),
        Port = int.Parse(cfgClient.Get("rabbitmq/port"))
      }.CreateConnection();
      _queueName = "NewResults";
      _channel = connection.CreateModel();
      _channel.QueueDeclare(_queueName, false, false, false, null);     
    }


    /// <summary>
    ///   Announce a new result dataset.
    /// </summary>
    /// <param name="simId">Simulation ID.</param>
    /// <param name="tick">Simulation time step.</param>
    public void AnnounceNewPackage(string simId, int tick) {
      var msg = string.Format("{{\"SimID\":\"{0}\", \"Tick\":{1}}}", simId, tick);
      var bytes = Encoding.UTF8.GetBytes(msg);      
      try {
        _channel.BasicPublish("", _queueName, null, bytes);
      }
      catch (Exception ex) {
        Console.Error.WriteLine("[RabbitNotifier] Propagation of new result package failed!");
        Console.Error.WriteLine(ex);
      }
    }
  }
}