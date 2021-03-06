﻿//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 07.02.2016
//  *******************************************************/

using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Hik.Scs.Communication.Scs.Communication.Channels.Tcp
{
	// Represents a collection of reusable SocketAsyncEventArgs objects.  
    internal class SocketAsyncEventArgsPool
	{
		Stack<SocketAsyncEventArgs> m_pool;

		// Initializes the object pool to the specified size
		//
		// The "capacity" parameter is the maximum number of 
		// SocketAsyncEventArgs objects the pool can hold
		public SocketAsyncEventArgsPool(int capacity)
		{
			m_pool = new Stack<SocketAsyncEventArgs>(capacity);
		}

		// Add a SocketAsyncEventArg instance to the pool
		//
		//The "item" parameter is the SocketAsyncEventArgs instance 
		// to add to the pool
		public void Push(SocketAsyncEventArgs item)
		{
			if (item == null) { throw new ArgumentNullException("Items added to a SocketAsyncEventArgsPool cannot be null"); }
			lock (m_pool)
			{
				m_pool.Push(item);
			}
		}

		// Removes a SocketAsyncEventArgs instance from the pool
		// and returns the object removed from the pool
		public SocketAsyncEventArgs Pop()
		{
			lock (m_pool)
			{
				return m_pool.Pop();
			}
		}

		// The number of SocketAsyncEventArgs instances in the pool
		public int Count
		{
			get { return m_pool.Count; }
		}

	}
}

