using System;
using System.Text;
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
    /// <param name="host">Name or IP of host computer.</param>
    /// <param name="user">Username to log in.</param>
    /// <param name="password">Associated password.</param>
    /// <param name="port">Rabbit MQ port (default port is 5672).</param>
    /// <param name="queue">Name of queue to use. If it does not exists, it will be created!</param>
    public RabbitNotifier(string host, string user, string password, int port, string queue) {
      var connection = new ConnectionFactory {
        HostName = host,
        UserName = user,
        Password = password,
        Port = port
      }.CreateConnection();
      _queueName = queue;
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