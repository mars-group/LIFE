﻿using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using ConfigurationAdapter;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using Newtonsoft.Json;
using NodeRegistry.Implementation.Messages;
using NUnit.Framework;

namespace MulticastAdapterTests {

  public class MultiCastAdapterBasicTests {

    private TNodeInformation _information;
    private int _listenStartPortSeed = 50000;
    private readonly int _sendingStartPortSeed = 52500;

    #region Setup/Teardown

    [SetUp]
    public void Setup() {
      _information = new TNodeInformation
      (
        NodeType.SimulationManager,
        "UnitTestNode0",
        new NodeEndpoint("127.0.0.1", 55500)
      );
    }

    #endregion

    [Test]
    public void TestMultiCastAdapter() {
      var localListenPort = _listenStartPortSeed;
      _listenStartPortSeed++;

      var mcastAdapterA = new MulticastAdapterComponent
      (
        new GlobalConfig("239.0.0.6", localListenPort, _sendingStartPortSeed, 4),
        new MulticastSenderConfig()
      );

      var mcastAdapterB = new MulticastAdapterComponent
      (
        new GlobalConfig("239.0.0.6", localListenPort, _sendingStartPortSeed, 4),
        new MulticastSenderConfig()
      );

      var tokenSource = new CancellationTokenSource();
      var ct = tokenSource.Token;

      var msgRcvdA = 0;
      var msgRcvdB = 0;

      Task.Factory.StartNew(() => {
        // Were we already canceled?
        //ct.ThrowIfCancellationRequested();

        while (!ct.IsCancellationRequested) {
          var msg = mcastAdapterA.ReadMulticastGroupMessage();

          if (msg.Length <= 0) continue;

          var msgString = Encoding.UTF8.GetString(msg);

          JsonConvert.DeserializeObject<AbstractNodeRegistryMessage>(msgString,
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
          msgRcvdA++;
        }
      }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

      Task.Factory.StartNew(() => {
        while (!ct.IsCancellationRequested) {
          var msg = mcastAdapterB.ReadMulticastGroupMessage();

          if (msg.Length <= 0) continue;

          var msgString = Encoding.UTF8.GetString(msg);
          Console.WriteLine($"Msg was: {msgString}");
          JsonConvert.DeserializeObject<AbstractNodeRegistryMessage>(msgString,
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All});
          msgRcvdB++;
          //Console.WriteLine($"Msg is: {regMsg.MessageType}, RCVD BY: B");
        }
      }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

      var sendTaskA = Task.Factory.StartNew(() => {
        Parallel.For(0, 10,
          i => mcastAdapterA.SendMessageToMulticastGroup(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
            new NodeRegistryConnectionInfoMessage(
              NodeRegistryMessageType.Join, _information, "192.168.178.31", null),
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All}))));
      }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

      var sendTaskB = Task.Factory.StartNew(() => {
        Parallel.For(0, 10,
          i => mcastAdapterA.SendMessageToMulticastGroup(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(
            new NodeRegistryConnectionInfoMessage(
              NodeRegistryMessageType.Join, _information, "192.168.178.31", null),
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All}))));
      }, tokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);

      sendTaskB.Wait();
      sendTaskA.Wait();
      Thread.Sleep(2000);
      Assert.True((msgRcvdA > 0) && (msgRcvdB > 0));
      Console.WriteLine($"A rcvd: {msgRcvdA} msgs");
      Console.WriteLine($"B rcvd: {msgRcvdB} msgs");
    }
  }
}