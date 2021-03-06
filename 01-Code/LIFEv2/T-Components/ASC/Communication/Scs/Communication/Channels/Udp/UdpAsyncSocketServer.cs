﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.02.2016
//  *******************************************************/
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config.Types;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
	public class UdpAsyncSocketServer
	{

		/// <summary>
		///     This event is raised when a new message is received.
		/// </summary>
		public event EventHandler<byte[]> DatagramReceived;

		private readonly int _numReadConnections;   // the maximum number of connections the sample is designed to handle simultaneously 
        private readonly int _numWriteConnections;
        private readonly Semaphore _maxNumberReadClients;
		private readonly Semaphore _maxNumberWriteClients;

        private readonly BufferManager _readBufferManager;  // represents a large reusable set of buffers for all socket operations
        private readonly BufferManager _writeBufferManager;
        private Socket _listenSocket;            // the socket used to listen for incoming connection requests
        private Socket _sendingSocket;			// the socket used to send datagrams

		// pools of reusable SocketAsyncEventArgs objects for write, read and accept socket operations
	    private readonly SocketAsyncEventArgsPool _readPool;
	    private readonly SocketAsyncEventArgsPool _writePool;

		private int _serverListenPort;

		private IPAddress _mcastAddress;

		private Thread _listenThread;
	    private readonly int _receiveBufferSize;


	    // Create an uninitialized server instance.  
		// To start the server listening for connection requests
		// call the Init method followed by Start method 
		//
		// <param name="numReadConnections">the maximum number of connections the sample is designed to handle simultaneously</param>
		// <param name="receiveBufferSize">buffer size to use for each socket I/O operation</param>
		public UdpAsyncSocketServer(int numReadConnections=10, int numWriteConnections=10, int receiveBufferSize=8192)
		{
			_numReadConnections = numReadConnections;
		    _numWriteConnections = numWriteConnections;

            _receiveBufferSize = receiveBufferSize;
			// allocate buffers such that the maximum number of sockets can have one outstanding read and 
			//write posted to the socket simultaneously  
			_readBufferManager = new BufferManager(receiveBufferSize * numReadConnections,
				receiveBufferSize);
			_writeBufferManager = new BufferManager(receiveBufferSize * numWriteConnections,
				receiveBufferSize);

			_readPool = new SocketAsyncEventArgsPool(numReadConnections);
			_writePool = new SocketAsyncEventArgsPool(numReadConnections);
			_serverListenPort = 6666;
			_maxNumberReadClients = new Semaphore (numReadConnections, numReadConnections);
			_maxNumberWriteClients = new Semaphore (numWriteConnections, numWriteConnections);
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
			_readBufferManager.InitBuffer();
            _writeBufferManager.InitBuffer();
			// preallocate pool of SocketAsyncEventArgs objects

		    for (var i = 0; i < _numReadConnections; i++)
			{
				//Pre-allocate a set of reusable SocketAsyncEventArgs
				var readEventArg = new SocketAsyncEventArgs();
				readEventArg.Completed += IO_Completed;
                // receive from every address and port
                readEventArg.RemoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                // assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
                _readBufferManager.SetBuffer(readEventArg);

				// add SocketAsyncEventArg to the pool
				_readPool.Push(readEventArg);
			}
            
		    for (var i = 0; i < _numWriteConnections; i++)
			{
				//Pre-allocate a set of reusable SocketAsyncEventArgs
				var writeEventArg = new SocketAsyncEventArgs();
				writeEventArg.Completed += IO_Completed;

				// assign a byte buffer from the buffer pool to the SocketAsyncEventArg object
				_writeBufferManager.SetBuffer(writeEventArg);

				// add SocketAsyncEventArg to the pool
				_writePool.Push(writeEventArg);
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
		    _listenSocket.ReceiveBufferSize = _receiveBufferSize;
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

		    _sendingSocket = new Socket(localEndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp)
		    {
		        SendBufferSize = _receiveBufferSize
		    };
		    var sendingStartPort = _serverListenPort + 1;

			BindSendingSocket(localAddress.Address, sendingStartPort);


            // begin to receive in Thread to not block the sending side
		    _listenThread = new Thread(Receive) {IsBackground = true};
		    _listenThread.Start();
		}

		private void BindSendingSocket(IPAddress localAddress, int sendingStartPort){
			try{
				_sendingSocket.Bind(new IPEndPoint(localAddress, sendingStartPort));
			} catch (SocketException socketException){
				//if sending port is already in use increment port and try again.
				if (socketException.SocketErrorCode != SocketError.AddressAlreadyInUse) throw;
				sendingStartPort = sendingStartPort + 1;
				BindSendingSocket(localAddress, sendingStartPort);
			}
		}

		public void Send(byte[] messageToSend){
			_maxNumberWriteClients.WaitOne ();
			// Pop a SocketAsyncEventArgs object from the stack
			var writeEventArgs = _writePool.Pop();
			// send to provided multicastaddress and serverListenPort
			writeEventArgs.RemoteEndPoint = new IPEndPoint (_mcastAddress, _serverListenPort);
            // set Buffer to messageToSend byte[]
		    try
		    {
				if(messageToSend.Length > _receiveBufferSize) {
					throw new ArgumentOutOfRangeException();
				}
				// set buffer to new length
               	writeEventArgs.SetBuffer(writeEventArgs.Offset, messageToSend.Length);
				// copy message into buffer
		      	Buffer.BlockCopy(messageToSend, 0, writeEventArgs.Buffer, writeEventArgs.Offset, messageToSend.Length);
		    }
		    catch (ArgumentOutOfRangeException)
		    {
                Console.WriteLine("Offset + Count were larger than buffersize");
            }

            // Start actual send operation
			var willRaiseEvent = _sendingSocket.SendToAsync (writeEventArgs);
			if (!willRaiseEvent) {
				// operation completed synchronously
				ProcessSend(writeEventArgs);
			}
		}
			

		private void Receive()
		{
			_maxNumberReadClients.WaitOne ();
			// Pop a SocketAsyncEventArgs object from the stack
			var readEventArgs = _readPool.Pop();

			// As soon as the client is connected, post a receive to the connection
			var willRaiseEvent = _listenSocket.ReceiveFromAsync(readEventArgs);
			if(!willRaiseEvent){
				// operation completed synchronously
				ProcessReceive(readEventArgs);
			}
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
			case SocketAsyncOperation.ReceiveFrom:
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
			Buffer.BlockCopy (e.Buffer, e.Offset, data, 0, dgramLength);

			// fire event
			OnDatagramReceived (data);

			// put the SocketAsyncEventArg object back onto the stack for later usage
			_readPool.Push(e);
			// release the semaphore
			_maxNumberReadClients.Release();

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
			_writePool.Push(e);
			// release the semaphore
			_maxNumberWriteClients.Release();
		}


	}    
}

