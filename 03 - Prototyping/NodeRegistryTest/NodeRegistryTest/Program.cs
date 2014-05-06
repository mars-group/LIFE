using System.Runtime.InteropServices;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;
using System;
using System.Collections.Generic;

namespace NodeRegistryTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter your Nodename, now!");
            Console.WriteLine("wait chris is made, so am i ... i parse it.");



            INodeRegistry nodeRegistry = new NodeRegistryManager();

            nodeRegistry.SubscribeForNewNodeConnected(handleJoinEvent);

            Console.WriteLine("Ok. Node Registry ist starting, brace yourself!");

            var nodeList = new List<NodeInformationType>();

            Console.WriteLine("1 => to add your node to the Cluster");
            Console.WriteLine("2 => show all Nodes in the Cluster");
            Console.WriteLine("3 => leave this Cluster");
            Console.WriteLine("4 => subscribe for newNodeEvent");
            Console.WriteLine("quit => shut down this app");
            Console.Write("<< ");

            String input = "";

            input = Console.ReadLine();

            while (!input.ToLower().Equals("quit"))
            {

                var i = int.Parse(input);

                switch (i)
                {
                    case 1:
                        Console.WriteLine("joining cluster");
                        nodeRegistry.JoinCluster();
                        Console.WriteLine("done.");
                        break;
                    case 2:

                        nodeList = nodeRegistry.GetAllNodes();
                        Console.WriteLine("printing nodes:");
                        foreach (var nodeInformationType in nodeList)
                        {
                            Console.WriteLine(nodeInformationType);
                        }
                        Console.WriteLine("done.");
                        break;
                    case 3:
                        Console.WriteLine("leaving cluster");
                        nodeRegistry.LeaveCluster();
                        Console.WriteLine("done");
                        break;
                    case 4:
                        nodeRegistry.SubscribeForNewNodeConnected(handleJoinEvent);
                        break;
                    default:
                        Console.WriteLine(" cant read input :-(");
                        break;
                }
                Console.WriteLine("");
                Console.Write("<< ");


                input = Console.ReadLine();

            }

        }

        private static void handleJoinEvent(NodeInformationType informationType)
        {
            Console.WriteLine("New Node from Type " + informationType.NodeType + " id is " + informationType.NodeIdentifier);
        }

    }



}
