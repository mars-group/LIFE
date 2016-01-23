/*
 * Created by SharpDevelop.
 * User: anovak
 * Date: 6/28/2010
 * Time: 10:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using Daylight;

namespace DummyNode
{
	/// <summary>
	/// This is a "dummy" node that listens on local port 8810.
	/// Use it if you want one Daylight application to be able to join a network easily, for testing.
	/// </summary>
	class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine("Initializing Dummy Node...");
			
			KademliaNode node = new KademliaNode(8810);
			node.EnableDebug();
		    node.Bootstrap(8810);
		    Console.ReadLine();
            //System.Threading.Thread.Sleep(50);
			node.JoinNetwork(); // Try to join network. Fail.
			
            node.Put(ID.Hash("A"), "AwesomeValue");

			// Sleep until we're killed.
			while(true) {
				System.Threading.Thread.Sleep(1000);
			}
		}
	}
}