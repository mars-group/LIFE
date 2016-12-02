//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.02.2016
//  *******************************************************/
using System;
using System.IO;
using System.Net;
using System.Text;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Communication.Messages;
using Newtonsoft.Json;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
	class UdpCommunicationChannel : CommunicationChannelBase
	{
		#region private fields

		private readonly AscUdpEndPoint _endPoint;
        private static readonly JsonSerializerSettings Jset = new JsonSerializerSettings() { TypeNameHandling = TypeNameHandling.All };

        /// <summary>
        ///     A flag to control thread's running
        /// </summary>
        private volatile bool _running;

		private UdpAsyncSocketServer _udpAsyncSockerServer;

		/// <summary>
		/// The multicast address being used for this communicationchannel
		/// </summary>
		private readonly IPAddress _mcastGroupIpAddress;

		/// <summary>
		/// The port to start sending on. Will automatically be increased if already in use
		/// </summary>
		private int _sendingStartPort;

		#endregion

		/// <summary>
		/// At the moment we don't need a remote endpoint to call methods on the other side.
		/// </summary>
		public override AscEndPoint RemoteEndPoint {
			get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
		}


		public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
			_udpAsyncSockerServer = new UdpAsyncSocketServer ();
			_udpAsyncSockerServer.Init();
			_endPoint = endPoint;
			// running is false, not yet
			_running = false;
			// create multicast IPAddress
			_mcastGroupIpAddress = IPAddress.Parse(_endPoint.McastGroup);
			// sending port starts with listenport +1, will be increased if port is not availabel
			_sendingStartPort = endPoint.UdpPort+1;
		}



		public override void Disconnect() {
			// do nothing atm...
		}

		#region protected Methods

		protected override void StartInternal()
		{
			// check if running so as to not call this multiple times from different client objects
			if (_running)
			{
				// nothing to be done, so return
				return;
			}

			_running = true;

			// start receiving in an asynchronous way
			_udpAsyncSockerServer.DatagramReceived += On_DatagramReceived;
			_udpAsyncSockerServer.Start(new IPEndPoint(IPAddress.Any, _endPoint.UdpPort), _mcastGroupIpAddress);
		}

		protected override void SendMessageInternal(IAscMessage message)
		{
			var json = JsonConvert.SerializeObject(message, Jset);

		    var messageBytes = Encoding.UTF8.GetBytes(json);

			_udpAsyncSockerServer.Send (messageBytes);
			
			// store last time a message was sent
			LastSentMessageTime = DateTime.Now;
			OnMessageSent(message);

		}

		private void SendCallback(IAsyncResult ar)
		{
			// nothing to be done here
		}

		#endregion


		/// <summary>
		/// Reacts to a received datagram by deserializing it and raising our own message received event
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="datagram">Datagram.</param>
		void On_DatagramReceived (object sender, byte[] datagram)
		{
		    var msg = JsonConvert.DeserializeObject<IAscMessage>(Encoding.UTF8.GetString(datagram), Jset);//.DeserializeObject(Encoding.UTF8.GetString(datagram));
	        OnMessageReceived(msg);
		}
	}

	[Serializable]
	internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
