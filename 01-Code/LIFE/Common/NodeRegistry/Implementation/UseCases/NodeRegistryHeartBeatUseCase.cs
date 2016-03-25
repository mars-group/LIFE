//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using log4net;
using LoggerFactory.Interface;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages.Factory;
using Timer = System.Timers.Timer;

[assembly: InternalsVisibleTo("NodeRegistryTest")]

namespace NodeRegistry.Implementation.UseCases
{
   

    internal class NodeRegistryHeartBeatUseCase
    {

        private readonly NodeRegistryNodeManagerUseCase _nodeRegistryNodeManagerUseCase;

        /// <summary>
		/// This dictionary maps Timers NodeInformationTypes to Timers. If no HeartBeat msg from a TNodeInformation is received the timeEvent will fire
		///  and delete this TNodeInformation from the active NodeList
        /// </summary>
        private readonly ConcurrentDictionary<TNodeInformation, Timer> _heartBeatTimers;

        private readonly int _heartBeatInterval;

        private readonly int _heartBeatTimeOutMultiplier;

        private readonly IMulticastAdapter _multicastAdapter;

        private readonly TNodeInformation _localNodeInformation;

        private Thread _heartBeatSenderThread;
        private readonly Timer _heartBeatSenderTimer;

        private readonly ILog Logger;



        public NodeRegistryHeartBeatUseCase(NodeRegistryNodeManagerUseCase nodeRegistryNodeManagerUseCase, TNodeInformation localNodeInformation, IMulticastAdapter multicastAdapter,  int heartBeatInterval, int heartBeatTimeOutMultiplier = 3)
        {

            Logger = LoggerInstanceFactory.GetLoggerInstance<NodeRegistryHeartBeatUseCase>();
            _nodeRegistryNodeManagerUseCase = nodeRegistryNodeManagerUseCase;
            
            _heartBeatInterval = heartBeatInterval;
            _heartBeatTimeOutMultiplier = heartBeatTimeOutMultiplier;
            _multicastAdapter = multicastAdapter;
            _localNodeInformation = localNodeInformation;

            _heartBeatTimers = new ConcurrentDictionary<TNodeInformation, Timer>();
            _heartBeatSenderTimer = new Timer(_heartBeatInterval);
            StartSendingHeartBeats();
        }

        private void StartSendingHeartBeats()
        {
            _heartBeatSenderTimer.Elapsed += SendHeartBeat;
            _heartBeatSenderTimer.Enabled = true;
            _heartBeatSenderTimer.Start();
        }


        public void CreateAndStartTimerForNodeEntry(TNodeInformation nodeInformation)
        {
            if (_heartBeatTimers.ContainsKey(nodeInformation) || nodeInformation.Equals(_localNodeInformation)) return;

            _nodeRegistryNodeManagerUseCase.AddNode(nodeInformation);

            var timer = CreateNewTimerForNodeEntry(nodeInformation);
            _heartBeatTimers[nodeInformation] = timer;
            timer.Start();
        }


        public void ResetTimer(String nodeID, NodeType nodeType)
        {
            var nodeInformationStub = new TNodeInformation(nodeType, nodeID, null);

            if (!_heartBeatTimers.ContainsKey(nodeInformationStub)) return;
            var timer = _heartBeatTimers[nodeInformationStub];
            timer.Stop();
            timer.Start();
        }

        public void Shutdow()
        {
            _heartBeatSenderTimer.Stop();
        }

        public void DeleteTimerForNodeInformationType(TNodeInformation nodeInformation) {
            if (!_heartBeatTimers.ContainsKey(nodeInformation)) return;
            _heartBeatTimers[nodeInformation].Stop();
            Timer deletedTimer;
            _heartBeatTimers.TryRemove(nodeInformation, out deletedTimer);
        }


        private Timer CreateNewTimerForNodeEntry(TNodeInformation nodeInformation)
        {
            var timer = new Timer(_heartBeatInterval * _heartBeatTimeOutMultiplier) { AutoReset = false };
			// add event to timer
            timer.Elapsed += delegate {
                Logger.Debug("Timer for " + nodeInformation + " expired. Deleting node.");
                _nodeRegistryNodeManagerUseCase.RemoveNode(nodeInformation);
                Timer delTimer;
                _heartBeatTimers.TryRemove(nodeInformation, out delTimer);
            };

            return timer;
        }

        private void SendHeartBeat(object sender, EventArgs eventArgs)
        {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetHeartBeatMessage(_localNodeInformation)
                );
        }

    }
}
