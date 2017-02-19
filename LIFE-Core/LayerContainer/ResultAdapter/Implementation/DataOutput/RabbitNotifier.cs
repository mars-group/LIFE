using System;
using System.Collections.Generic;
using System.Text;
using ConfigService;
using RabbitMQ.Client;

namespace ResultAdapter.Implementation.DataOutput {
  
  /// <summary>
  ///   RabbitMQ notifier for new result packets. 
  /// </summary>
  internal class RabbitNotifier {
    
    private IModel _channel;             // AMQP (Advanced-Message-Queue-Protocol) channel.
    private readonly IConfigServiceClient _csc; // The config service client for key retrieval.
    private readonly string _queueName;         // Name of the used queue.


    /// <summary>
    ///   Initialize a new Rabbit-MQ notifier.
    /// </summary>
    /// <param name="cfgClient">MARS KV client for connection properties.</param>
    public RabbitNotifier(IConfigServiceClient cfgClient) {   
      _queueName = "ResultAdapterEvents";
      _csc = cfgClient;
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

        // Check if the connection is available. Otherwise (re-)initialize it.
        if (_channel == null || _channel.IsClosed) {
          var connection = new ConnectionFactory {
            HostName = "rabbitmq",
            UserName = _csc.Get("rabbitmq/user"),
            Password = _csc.Get("rabbitmq/pass"),
            Port = 5672
          }.CreateConnection();
          _channel = connection.CreateModel();
          _channel.QueueDeclare(_queueName, false, false, false, new Dictionary<string, object> {
            {"x-message-ttl", 5000}
          });           
        }

        // Send propagation message to the queue. 
        _channel.BasicPublish("", _queueName, null, bytes);
      }
      catch (Exception ex) {
        Console.Error.WriteLine("[RabbitNotifier] Propagation of new result package failed!");
        Console.Error.WriteLine(ex);
      }
    }
  }
}