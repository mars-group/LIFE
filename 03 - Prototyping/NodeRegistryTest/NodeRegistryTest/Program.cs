using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using NodeRegistry.Implementation;
using NodeRegistry.Interface;

namespace NodeRegistryTest
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Enter your Nodename, now!");
            Console.Write("<< ");

            var nodeName = Console.ReadLine();

            Console.WriteLine("Good job Human. Now enter your IP, cause I am to lazy to parse it from your interface.");
            Console.Write("<< ");

            var ip = Console.ReadLine();


            INodeRegistry nodeRegistry = new NodeRegistryManager(new NodeInformationType(NodeType.LayerContainer, nodeName, new NodeEndpoint(ip, 60100)));

            Console.WriteLine("Ok. Node Registry ist starting, brace yourself!");

            var nodeList= new List<NodeInformationType>();

            Console.WriteLine("1 => to add your node to the Cluster");
            Console.WriteLine("2 => show all Nodes in the Cluster");
            Console.WriteLine("3 => leave this Cluster");
            Console.WriteLine("quit => shut down this app");
            Console.Write("<< ");

            String input = "";

            input = Console.ReadLine();

            while (!input.Equals("quit"))
            {

                var i = Int32.Parse(input);

                switch (i)
                {
                    case 1:
                        Console.WriteLine("joining cluster");
                        nodeRegistry.startDiscovery();
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
                    default:
                        Console.WriteLine(" cant read input :-(");
                        break;
                }
                Console.WriteLine("");
                Console.Write("<< ");
                input = Console.ReadLine();



            }


        }
    }
}
