using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Communication.EndPoints.Udp;
using ASC.Communication.Scs.Communication.Messages;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config.Types;
using System.Threading.Tasks;
using System.Threading;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
	class UdpCommunicationChannel : CommunicationChannelBase
	{
		#region private fields

		private readonly AscUdpEndPoint _endPoint;

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

		/// <summary>
		/// The Formatter used to serialize and de-serialize
		/// </summary>
		private readonly BinaryFormatter _binaryFormatter;

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

			// get all sending udpClients. One per active and multicast enabled interface
			//_udpSendingClients = GetSendingClients();

			_binaryFormatter = new BinaryFormatter();
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
			//Create a byte array from message according to current protocol
			var memoryStream = new MemoryStream();

			new BinaryFormatter().Serialize(memoryStream, message);

			var messageBytes = memoryStream.ToArray();

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
		    var msg = (IAscMessage) new BinaryFormatter().Deserialize(new MemoryStream(datagram));
	        OnMessageReceived(msg);
		}
	}

	[Serializable]
	internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
