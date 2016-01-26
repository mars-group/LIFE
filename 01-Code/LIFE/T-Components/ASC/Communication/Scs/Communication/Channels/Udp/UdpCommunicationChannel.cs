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

		/// <summary>
		/// The UDP receiver client
		/// </summary>
		private UdpClient _udpReceivingClient;

		/// <summary>
		/// All UDP sending clients. One client per interface is created
		/// </summary>
		private readonly List<UdpClient> _udpSendingClients;

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

        /// <summary>
        /// Listening Thread
        /// </summary>
	    private Thread _listenThread;
		#endregion

		/// <summary>
		/// At the moment we don't need a remote endpoint to call methods on the other side.
		/// </summary>
		public override AscEndPoint RemoteEndPoint {
			get { throw new UdpCommunicationHasNoRemoteEndpointException(); }
		}


		public UdpCommunicationChannel(AscUdpEndPoint endPoint) {
			_endPoint = endPoint;
			// running is false, not yet
			_running = false;
			// create multicast IPAddress
			_mcastGroupIpAddress = IPAddress.Parse(_endPoint.McastGroup);
			// sending port starts with listenport +1, will be increased if port is not availabel
			_sendingStartPort = endPoint.UdpPort+1;
			// get a receiving UDP client which listens on all interfaces and specified port
			_udpReceivingClient = GetReceivingClient();
			// Join multicast groups with all receiving clients
			JoinMulticastGroup();
			// get all sending udpClients. One per active and multicast enabled interface
			_udpSendingClients = GetSendingClients();

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
			_listenThread = new Thread(Receive);
			_listenThread.Start();
		}

		private void Receive() {
			var remoteEP = new IPEndPoint(IPAddress.Any, 0);
		    while (_running) {
				try
				{
				    var data = _udpReceivingClient.Receive(ref remoteEP);
				    var stream = new MemoryStream(data);
				    var msg = (IAscMessage)_binaryFormatter.Deserialize(stream);
				    // inform all listeners about the new message
				    OnMessageReceived(msg);
				}
				catch(ObjectDisposedException){
					// catch broken sockets and re-create them
					_udpReceivingClient = GetReceivingClient();
					JoinMulticastGroup();
				}
				catch (SocketException sex)
				{
					if (sex.ErrorCode != 10004 && sex.ErrorCode != 10060) throw;
				}
			}
		}

		protected override void SendMessageInternal(IAscMessage message)
		{
			//Create a byte array from message according to current protocol
			var memoryStream = new MemoryStream();

			new BinaryFormatter().Serialize(memoryStream, message);

			var messageBytes = memoryStream.ToArray();
			var endpoint = new IPEndPoint(_mcastGroupIpAddress, _endPoint.UdpPort);


			//Send all bytes to the remote application asynchronously
			_udpSendingClients.ForEach(client =>
				client.BeginSend (messageBytes, messageBytes.Length, endpoint, SendCallback, client));
			
			// store last time a message was sent
			LastSentMessageTime = DateTime.Now;
			OnMessageSent(message);

		}

		private void SendCallback(IAsyncResult ar)
		{
            
			// nothing to be done here
		}

		#endregion

		#region private Methods

		/// <summary>
		/// Joins the multicast group via all available interfaces
		/// </summary>
		private void JoinMulticastGroup()
		{
			foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
			{
				foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
				{
					if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4))
						_udpReceivingClient.JoinMulticastGroup(_mcastGroupIpAddress, unicastAddr.Address);
				}
			}
		}

		private UdpClient GetReceivingClient()
		{
			var udpClient = new UdpClient();
			udpClient.ExclusiveAddressUse = false;

			// allow another client to bind to this port
			udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
			udpClient.Client.Bind(new IPEndPoint(IPAddress.Any, _endPoint.UdpPort));
			return udpClient;
		}

		private List<UdpClient> GetSendingClients()
		{
			var resultList = new List<UdpClient>();
			//foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
			//{
                /*
				foreach (var unicastAddress in networkInterface.GetIPProperties().UnicastAddresses)
				{
					if (unicastAddress.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4))
					{
						var updClient = SetupSocket(unicastAddress.Address);
						updClient.MulticastLoopback = true;
						resultList.Add(updClient);
					}
				}
                */

                var address = MulticastNetworkUtils.GetAllMulticastInterfaces().First().GetIPProperties().UnicastAddresses
			        .First(elem => elem.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4));
                var updClient = SetupSocket(address.Address);
                updClient.MulticastLoopback = true;
                resultList.Add(updClient);
            //}

			return resultList;
		}

		/// <summary>
		/// Sets up the sending socket.
		/// Will increase the sending port, if it is already in use
		/// </summary>
		/// <param name="unicastAddress"></param>
		/// <returns></returns>
		private UdpClient SetupSocket(IPAddress unicastAddress)
		{
			try
			{
			    return new UdpClient(new IPEndPoint(unicastAddress, _sendingStartPort));

			}
			catch (SocketException socketException)
			{
				//if sending port is already in use increment port and try again.
				if (socketException.ErrorCode != 10048) throw;
				_sendingStartPort = _sendingStartPort + 1;
				return SetupSocket(unicastAddress);
			}
		}

		#endregion
	}

	[Serializable]
	internal class UdpCommunicationHasNoRemoteEndpointException : Exception {}
}
