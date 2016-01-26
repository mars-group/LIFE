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
		private byte[] _buffer;                // the underlying byte array maintained by the Buffer Manager
	    private readonly Stack<int> _freeIndexPool;     // 
		private int _currentIndex;
	    private readonly int _bufferSize;

	    private object _synclock;

		public BufferManager(int totalBytes, int bufferSize)
		{
			_numBytes = totalBytes;
			_currentIndex = 0;
			_bufferSize = bufferSize;
			_freeIndexPool = new Stack<int>();
		    _synclock = new object();
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
            lock (_synclock) { 
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
        }

		// Removes the buffer from a SocketAsyncEventArg object.  
		// This frees the buffer back to the buffer pool
		public void FreeBuffer(SocketAsyncEventArgs args)
		{
		    lock (_synclock)
		    {
		        _freeIndexPool.Push(args.Offset);
		        args.SetBuffer(null, 0, 0);
		    }
		}

	}
}

