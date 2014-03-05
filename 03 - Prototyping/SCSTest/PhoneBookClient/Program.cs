using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Threading;
using System.Threading.Tasks;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using PhoneBookCommonLib;

namespace PhoneBookClient
{
    class Program
    {
        private const int ResetEventCount = 1;
        private const int ItemCount = 100;

        static void Main(string[] args)
        {
            //Create a client to connect to phone book service on local server and
            //10048 TCP port.
            var client = ScsServiceClientBuilder.CreateClient<IPhoneBookService>(
                new ScsTcpEndPoint("127.0.0.1", 10048));

            //Console.WriteLine("Press enter to connect to phone book service...");
            //Console.ReadLine();

            //Connect to the server
            client.Connect();

            client.ServiceProxy.AddPerson(new PhoneBookRecord
            {
                Name = "Christian",
                Phone = "123"
            });


            var resetEventsLock = new object();
            var resetEvents = new Dictionary<int, ManualResetEvent>();

            var now = DateTime.Now;

            for (var o = 0; o < ResetEventCount; o++)
            {
                var rootid = resetEvents.Count + 1;

                resetEvents.Add(rootid, new ManualResetEvent(false));

                
                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    for (var i = 0; i <= ItemCount; i++)
                    {
                        var g = new Guid();
                        var nr = new Guid();
                        client.ServiceProxy.AddPerson(new PhoneBookRecord
                        {
                            Name = g.ToString(),
                            Phone = nr.ToString()
                        });
                    }

                    lock (resetEventsLock)
                    {
                        resetEvents[rootid].Set();
                    }
                }), rootid);
    
            }

            
            WaitHandle.WaitAll(resetEvents.Values.ToArray());

            var then = DateTime.Now;
            Console.WriteLine("WRITE:" + (then - now).TotalMilliseconds);
            
            //////////////////////

            resetEventsLock = new object();
            resetEvents = new Dictionary<int, ManualResetEvent>();

            now = DateTime.Now;

            for (var o = 0; o < ResetEventCount; o++)
            {
                var rootid = resetEvents.Count + 1;

                resetEvents.Add(rootid, new ManualResetEvent(false));


                ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                {
                    for (var i = 0; i <= ItemCount; i++)
                    {
                        //Search for a person
                        var person = client.ServiceProxy.FindPerson("Christian");

                    }

                    lock (resetEventsLock)
                    {
                        resetEvents[rootid].Set();
                    }
                }), rootid);

            }


            WaitHandle.WaitAll(resetEvents.Values.ToArray());

            then = DateTime.Now;
            Console.WriteLine("READ:" + (then - now).TotalMilliseconds);
            





            Console.WriteLine();
            Console.WriteLine("Press enter to disconnect from phone book service...");
            Console.ReadLine();

            //Disconnect from server
            client.Disconnect();
        }


    }
}
