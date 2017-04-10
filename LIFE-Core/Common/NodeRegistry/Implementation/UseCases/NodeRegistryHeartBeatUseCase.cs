//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 19.10.2015
//  *******************************************************/

using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Interface;
using NodeRegistry.Implementation.Messages.Factory;

[assembly: InternalsVisibleTo("NodeRegistryTest")]

namespace NodeRegistry.Implementation.UseCases
{
    internal class NodeRegistryHeartBeatUseCase
    {
        private readonly string _clusterName;

        private readonly int _heartBeatInterval;

        private readonly Timer _heartBeatSenderTimer;

        private readonly int _heartBeatTimeOutMultiplier;

        /// <summary>
        ///   This dictionary maps Timers NodeInformationTypes to Timers. If no HeartBeat msg from a TNodeInformation is received
        ///   the timeEvent will fire
        ///   and delete this TNodeInformation from the active NodeList
        /// </summary>
        private readonly ConcurrentDictionary<TNodeInformation, Timer> _heartBeatTimers;

        private readonly TNodeInformation _localNodeInformation;

        private readonly IMulticastAdapter _multicastAdapter;

        private readonly NodeRegistryNodeManagerUseCase _nodeRegistryNodeManagerUseCase;


        public NodeRegistryHeartBeatUseCase(NodeRegistryNodeManagerUseCase nodeRegistryNodeManagerUseCase,
            TNodeInformation localNodeInformation, IMulticastAdapter multicastAdapter, int heartBeatInterval,
            string clusterName, int heartBeatTimeOutMultiplier = 3)
        {
            _clusterName = clusterName;
            _nodeRegistryNodeManagerUseCase = nodeRegistryNodeManagerUseCase;

            _heartBeatInterval = heartBeatInterval;
            _heartBeatTimeOutMultiplier = heartBeatTimeOutMultiplier;
            _multicastAdapter = multicastAdapter;
            _localNodeInformation = localNodeInformation;

            _heartBeatTimers = new ConcurrentDictionary<TNodeInformation, Timer>();
            var autoEvent = new AutoResetEvent(false);

            _heartBeatSenderTimer = new Timer(SendHeartBeat, autoEvent, 0, _heartBeatInterval);
        }


        public void CreateAndStartTimerForNodeEntry(TNodeInformation nodeInformation)
        {
            if (_heartBeatTimers.ContainsKey(nodeInformation) || nodeInformation.Equals(_localNodeInformation)) return;

            //_nodeRegistryNodeManagerUseCase.AddNode(nodeInformation);

            var timer = CreateNewTimerForNodeEntry(nodeInformation);
            _heartBeatTimers[nodeInformation] = timer;
        }

        private Timer CreateNewTimerForNodeEntry(TNodeInformation nodeInformation)
        {
            var timeOut = _heartBeatInterval * _heartBeatTimeOutMultiplier;
            return new Timer(delegate
            {
                _nodeRegistryNodeManagerUseCase.RemoveNode(nodeInformation);
                Timer delTimer;
                _heartBeatTimers.TryRemove(nodeInformation, out delTimer);
            }, new AutoResetEvent(false), timeOut, timeOut);
        }

        public void ResetTimer(string nodeID, NodeType nodeType)
        {
            var nodeInformationStub = new TNodeInformation(nodeType, nodeID, null);

            if (!_heartBeatTimers.ContainsKey(nodeInformationStub)) return;
            var timer = _heartBeatTimers[nodeInformationStub];
            var timeOut = _heartBeatInterval * _heartBeatTimeOutMultiplier;
            timer.Change(timeOut, timeOut);
        }

        public void Shutdow()
        {
            _heartBeatSenderTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        public void DeleteTimerForNodeInformationType(TNodeInformation nodeInformation)
        {
            if (!_heartBeatTimers.ContainsKey(nodeInformation)) return;
            _heartBeatTimers[nodeInformation].Change(Timeout.Infinite, Timeout.Infinite);
            Timer deletedTimer;
            _heartBeatTimers.TryRemove(nodeInformation, out deletedTimer);
        }


        private void SendHeartBeat(object stateInfo)
        {
            _multicastAdapter.SendMessageToMulticastGroup(
                NodeRegistryMessageFactory.GetHeartBeatMessage(_localNodeInformation, _clusterName)
            );
        }
    }
}