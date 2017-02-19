﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.02.2016
//  *******************************************************/
using System.Collections.Generic;
using System.Net.Sockets;

namespace ASC.Communication.Scs.Communication.Channels.Udp
{
	// This class creates a single large buffer which can be divided up 
	// and assigned to SocketAsyncEventArgs objects for use with each 
	// socket I/O operation.  
	// This enables bufffers to be easily reused and guards against 
	// fragmenting heap memory.
	// 
	// The operations exposed on the BufferManager class are not thread safe.
	internal class BufferManager
	{
	    private readonly int _numBytes;                 // the total number of bytes controlled by the buffer pool
		private byte[] _buffer;                			// the underlying byte array maintained by the Buffer Manager
	    private readonly Stack<int> _freeIndexPool;     // 
		private int _currentIndex;
	    private readonly int _bufferSize;

		public BufferManager(int totalBytes, int bufferSize)
		{
			_numBytes = totalBytes;
			_currentIndex = 0;
			_bufferSize = bufferSize;
			_freeIndexPool = new Stack<int>();
		}

		// Allocates buffer space used by the buffer pool
		public void InitBuffer()
		{
			// create one big large buffer and divide that 
			// out to each SocketAsyncEventArg object
			_buffer = new byte[_numBytes];
		}

		// Assigns a buffer from the buffer pool to the 
		// specified SocketAsyncEventArgs object
		//
		// <returns>true if the buffer was successfully set, else false</returns>
		public bool SetBuffer(SocketAsyncEventArgs args)
		{

			    if (_freeIndexPool.Count > 0)
			    {
				    args.SetBuffer(_buffer, _freeIndexPool.Pop(), _bufferSize);
			    }
			    else
			    {
				    if ((_numBytes - _bufferSize) < _currentIndex)
				    {
					    return false;
				    }
				    args.SetBuffer(_buffer, _currentIndex, _bufferSize);
				    _currentIndex += _bufferSize;
			    }
			    return true;
            
        }

		// Removes the buffer from a SocketAsyncEventArg object.  
		// This frees the buffer back to the buffer pool
		public void FreeBuffer(SocketAsyncEventArgs args)
		{

		        _freeIndexPool.Push(args.Offset);
		        args.SetBuffer(null, 0, 0);
		    
		}

	}
}

