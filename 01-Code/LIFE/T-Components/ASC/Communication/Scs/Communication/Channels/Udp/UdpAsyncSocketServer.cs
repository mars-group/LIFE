using System;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config.Types;
using System.Linq;

namespace ASC
{
	public class UdpAsyncSocketServer
	{

		/// <summary>
		///     This event is raised when a new message is received.
		/// </summary>
		public event EventHandler<byte[]> DatagramReceived;

		private int m_numConnections;   // the maximum number of connections the sample is designed to handle simultaneously 
		private int m_receiveBufferSize;// buffer size to use for each socket I/O operation 

		private Semaphore m_maxNumberReadClients;
		private Semaphore m_maxNumberWriteClients;

		readonly BufferManager m_readBufferManager;  // represents a large reusable set of buffers for all socket operations
		readonly BufferManager m_writeBufferManager;
		const int opsToPreAlloc = 2;    // read, write (don't alloc buffer space for accepts)
		Socket _listenSocket;            // the socket used to listen for incoming connection requests
		Socket _sendingSocket;			// the socket used to send datagrams

		// pools of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
		readonly SocketAsyncEventArgsPool m_readPool;
		readonly SocketAsyncEventArgsPool m_writePool;

		int _serverListenPort;

		IPAddress _mcastAddress;

		Thread _listenThread;

		// Create an uninitialized server instance.  
		// To start the server listening for connection requests
		// call the Init method followed by Start method 
		//
		// <param name="numConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
		// <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
		public UdpAsyncSocketServer(int numConnections=12, int receiveBufferSize=8192)
		{
			m_numConnections = numConnections;
			m_receiveBufferSize = receiveBufferSize;
			// allocate buffers such that the maximum number of sockets can have one outstanding read and 
			//write posted to the socket simultaneously  
			m_readBufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
				receiveBufferSize);
			m_writeBufferManager = new BufferManager(receiveBufferSize * numConnections * opsToPreAlloc,
				receiveBufferSize);

			m_readPool = new SocketAsyncEventArgsPool(numConnections);
			m_writePool = new SocketAsyncEventArgsPool(numConnections);
			_serverListenPort = 6666;
			m_maxNumberReadClients = new Semaphore (numConnections, numConnections);
			m_maxNumberWriteClients = new Semaphore (numConnections, numConnections);
		}

		// Initializes the server by preallocating reusable buffers and 
		// context objects.  These objects do not need to be preallocated 
		// or reused, but it is done this way to illustrate how the API can 
		// easily be used to create reusable objects to increase server performance.
		//
		public void Init()
		{
			// Allocates one large byte buffer which all I/O operations use a piece of.  This gaurds 
			// against memory fragmentation
			m_readBufferManager.InitBuffer();

			// preallocate pool of SocketAsyncEventArgs objects
			SocketAsyncEventArgs readEventArg;

			for (int i = 0; i < m_numConnections; i++)
			{
				//Pre-allocate a set of reusable SocketAsyncEventArgs
				readEventArg = new SocketAsyncEventArgs();
				readEventArg.Completed += IO_Completed;

				// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
				m_readBufferManager.SetBuffer(readEventArg);

				// add SocketAsyncEventArg to the pool
				m_readPool.Push(readEventArg);
			}

			SocketAsyncEventArgs writeEventArg;

			for (int i = 0; i < m_numConnections; i++)
			{
				//Pre-allocate a set of reusable SocketAsyncEventArgs
				writeEventArg = new SocketAsyncEventArgs();
				writeEventArg.Completed += IO_Completed;

				// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
				m_writeBufferManager.SetBuffer(writeEventArg);

				// add SocketAsyncEventArg to the pool
				m_writePool.Push(writeEventArg);
			}

		}

		// Starts the server such that it is listening for 
		// incoming connection requests.    
		//
		// <param name="localEndPoint">The endpoint which the server will listening 
		// for connection requests on</param>
		public void Start(IPEndPoint localEndPoint, IPAddress mcastAddress)
		{
			_serverListenPort = localEndPoint.Port;
			_mcastAddress = mcastAddress;
			// create the socket which listens for incoming connections
			_listenSocket = new Socket(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
			_listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

			_listenSocket.Bind(localEndPoint);

			// join Mcast Groups on all relevant interfaces
			foreach (var networkInterface in MulticastNetworkUtils.GetAllMulticastInterfaces())
			{
				foreach (var unicastAddr in networkInterface.GetIPProperties().UnicastAddresses)
				{
					if (unicastAddr.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily (IPVersionType.IPv4)) {
						var mcastOption = new MulticastOption(mcastAddress, unicastAddr.Address);

						_listenSocket.SetSocketOption(SocketOptionLevel.IP, 
							SocketOptionName.AddMembership, 
							mcastOption);
					}
				}
			}

			// create the socket(s) we will use to send
			var localAddress = MulticastNetworkUtils.GetAllMulticastInterfaces().First().GetIPProperties().UnicastAddresses
				.First(elem => elem.Address.AddressFamily == MulticastNetworkUtils.GetAddressFamily(IPVersionType.IPv4));
			
			_sendingSocket = new Socket(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

			var sendingStartPort = _serverListenPort + 1;

			BindSendingSocket(localAddress.Address, sendingStartPort);


            // begin to receive in Thread to not block the sending side
			_listenThread = new Thread(Receive);
			_listenThread.IsBackground = true;
			_listenThread.Start();
		}

		private void BindSendingSocket(IPAddress localAddress, int sendingStartPort){
			try{
				_sendingSocket.Bind(new IPEndPoint(localAddress, sendingStartPort));
			} catch (SocketException socketException){
				//if sending port is already in use increment port and try again.
				if (socketException.ErrorCode != 10048) throw;
				sendingStartPort = sendingStartPort + 1;
				BindSendingSocket(localAddress, sendingStartPort);
			}
		}

		public void Send(byte[] messageToSend){
			m_maxNumberWriteClients.WaitOne ();
			// Pop a SocketAsyncEventArgs object from the stack
			SocketAsyncEventArgs writeEventArgs = m_writePool.Pop();
			// send to provided multicastaddress and serverListenPort
			writeEventArgs.RemoteEndPoint = new IPEndPoint (_mcastAddress, _serverListenPort);
			// free the buffer of this socketAsyncEventArg
			m_writeBufferManager.FreeBuffer (writeEventArgs);
			// set Buffer to messageToSend byte[]
			writeEventArgs.SetBuffer(messageToSend, 0, messageToSend.Length);
			bool willRaiseEvent = _sendingSocket.SendToAsync (writeEventArgs);
			if (!willRaiseEvent) {
				// operation completed synchronously
				ProcessSend(writeEventArgs);
			}
		}
			

		private void Receive()
		{
			m_maxNumberReadClients.WaitOne ();
			// Pop a SocketAsyncEventArgs object from the stack
			SocketAsyncEventArgs readEventArgs = m_readPool.Pop();

			// receive from every address and port
			readEventArgs.RemoteEndPoint = new IPEndPoint (IPAddress.Any, 0);

			// As soon as the client is connected, post a receive to the connection
			bool willRaiseEvent = _listenSocket.ReceiveAsync(readEventArgs);
			if(!willRaiseEvent){
				// operation completed synchronously
				ProcessReceive(readEventArgs);
			}

			/*
			 *  catch (SocketException sex)
				{
					if (sex.ErrorCode != 10004 && sex.ErrorCode != 10060) throw;
				}
			 */

			// Accept the next connection request
			Receive();
		}

		// This method is called whenever a receive or send operation is completed on a socket 
		//
		// <param name="e">SocketAsyncEventArg associated with the completed receive operation</param>
		void IO_Completed(object sender, SocketAsyncEventArgs e)
		{
			// determine which type of operation just completed and call the associated handler
			switch (e.LastOperation)
			{
			case SocketAsyncOperation.Receive:
				ProcessReceive(e);
				break;
			case SocketAsyncOperation.SendTo:
				ProcessSend(e);
				break;
			default:
				throw new ArgumentException("The last operation completed on the socket was not a receive or send");
			}       

		}

		// This method is invoked when an asynchronous receive operation completes.  
		private void ProcessReceive(SocketAsyncEventArgs e)
		{
			var dgramLength = e.BytesTransferred;
			var data = new byte[dgramLength];
			// copy datagram into data bytearray
			Buffer.BlockCopy (e.Buffer, 0, data, 0, dgramLength);

			// fire event
			OnDatagramReceived (data);

			// put the SocketAsyncEventArg object back onto the stack for later usage
			m_readPool.Push(e);
			// release the semaphore
			m_maxNumberReadClients.Release();

			// currently nothing, later on: Handle messages too large for the buffer,
			// this will need a 4 digit prefix to indicate the message size
		}

		protected virtual void OnDatagramReceived(byte[] datagram) {
			var handler = DatagramReceived;
			if (handler != null) handler(this, datagram);
		}

		// This method is invoked when an asynchronous send operation completes.  
		// The method issues another receive on the socket to read any additional 
		// data sent from the client
		//
		// <param name="e"></param>
		private void ProcessSend(SocketAsyncEventArgs e)
		{
			// sending done, reset buffer and free socketAsyncEventArg
			m_writeBufferManager.SetBuffer (e);
			m_writePool.Push(e);
			// release the semaphore
			m_maxNumberWriteClients.Release();
		}


	}    
}

