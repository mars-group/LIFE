//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 18.12.2015
//  *******************************************************/

using System;
using System.Text;
using CommonTypes;
using ConfigService;
using RabbitMQ.Client;

namespace RabbitMQClient {

  public class RabbitMqWriter {

    private readonly string _queueName;
    private readonly IModel _channel;

    public RabbitMqWriter(Guid simulationId) {
      var cfgClient = new ConfigServiceClient(MARSConfigServiceSettings.Address);
      var connection = new ConnectionFactory {
        HostName = cfgClient.Get("rabbitmq/ip"),
        UserName = cfgClient.Get("rabbitmq/user"),
        Password = cfgClient.Get("rabbitmq/pass"),
        Port = int.Parse(cfgClient.Get("rabbitmq/port"))
      }.CreateConnection();
      _channel = connection.CreateModel();

      // name queue with 'life-' prefix
      _queueName = "life-" + simulationId;
      _channel.QueueDeclare(_queueName,
        false,
        false,
        false,
        null);
    }

    /// <summary>
    ///   Sends a message via RabbitMQ
    /// </summary>
    /// <param name="message">Message.</param>
    public void SendMessage(string message) {
      var body = Encoding.UTF8.GetBytes(message);
      try {
        _channel.BasicPublish("", _queueName, null, body);
      }
      catch (Exception ex) {
        Console.Error.WriteLine("[RabbitMQWriter] Propagation of new result package failed!");
        Console.Error.WriteLine(ex);
      }
    }
  }
}