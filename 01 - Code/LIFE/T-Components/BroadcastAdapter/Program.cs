﻿
using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net.NetworkInformation;

namespace SimpleMCastTestProject
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			//DisplayTypeAndAddress ();
			var ifs = NetworkInterface.GetAllNetworkInterfaces ();
			Parallel.ForEach (ifs, (inf) => {

				if (inf.SupportsMulticast) {
					Console.WriteLine("Interface supports MCAST:" + inf.Name);
				}
			});

			Task.Run (() => Receiver ());
			Task.Run (() => {
				while(true)
				{
					Sender ();
					Thread.Sleep(1000);
				}
			});



			Console.ReadLine ();
		}

		static void Sender()
		{
			Console.WriteLine ("Sending...");

			var recipient = new IPEndPoint (IPAddress.Parse("224.100.10.10"), 60100);
			var udpClient = new UdpClient ();
			udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.10.10"));
			var data = Encoding.UTF8.GetBytes("Hallo world!");
			int bytesSent = udpClient.Send (data, data.Length, recipient);
			udpClient.Close ();
			Console.WriteLine ("{0} bytes sent", bytesSent);
		}

		static void Receiver()
		{
			Console.WriteLine ("Receiving...");
			var udpClient = new UdpClient (60100);
			udpClient.JoinMulticastGroup(IPAddress.Parse("224.100.10.10"));
			var endPoint = new IPEndPoint(IPAddress.Any, 0);
			byte[] data = udpClient.Receive (ref endPoint);
			Console.WriteLine ("Received '{0}'.", Encoding.UTF8.GetString (data));
			udpClient.Close ();
		}

		public static void DisplayTypeAndAddress()
		{
			//IPGlobalProperties computerProperties = IPGlobalProperties.GetIPGlobalProperties();
			NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
			//Console.WriteLine("Interface information for {0}.{1}     ",
			//	computerProperties.HostName, computerProperties.DomainName);
			foreach (NetworkInterface adapter in nics)
			{
				IPInterfaceProperties properties = adapter.GetIPProperties();
				Console.WriteLine(adapter.Description);
				Console.WriteLine(String.Empty.PadLeft(adapter.Description.Length,'='));
				Console.WriteLine("  Interface type .......................... : {0}", adapter.NetworkInterfaceType);
				Console.WriteLine("  Physical Address ........................ : {0}", 
					adapter.GetPhysicalAddress().ToString());
				Console.WriteLine("  Is receive only.......................... : {0}", adapter.IsReceiveOnly);
				Console.WriteLine("  Multicast................................ : {0}", adapter.SupportsMulticast);
				Console.WriteLine();
			}
		}
	}
}