using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Client;
using PhoneBookCommonLib;

namespace PhoneBookClient
{
    class Program
    {
        private const int ResetEventCount = 10;
        private const int ItemCount = 10000;

        static void Main(string[] args)
        {
            Console.WriteLine("BenchmarkWriteAndRead...");
            BenchmarkWriteAndRead();
            Console.WriteLine("BenchmarkWriteAndReadWithChanges....");
           // BenchmarkWriteAndReadWithChanges();
        }


        private static void BenchmarkWriteAndRead()
        {
            var yac = ScsServiceClientBuilder.CreateClient<IYetAnotherService>(
                //new ScsTcpEndPoint(IPAddress.Loopback.ToString(), BasePort), serviceID);
                new ScsTcpEndPoint("192.168.178.31", 10048));
            var client = new Client(yac.ServiceProxy,GuidProvider.GetIdenticalGuid());


            client.AddPerson(new PhoneBookRecord
            {
                Name = "Christian",
                Phone = "1235"
            });
            client.Title = "Titel0";


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

                        client.AddPerson(new PhoneBookRecord
                        {
                            Name = g.ToString(),
                            Phone = nr.ToString()
                        });
                        Console.WriteLine(client.GetInformation());
                        
                        client.Title = "Titel";
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
                        var person = client.FindPerson("Christian");
                        var titel = client.Title;
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

            Console.ReadLine();

            //Disconnect from server
            client.Disconnect();    
        }
        /*
        private static void BenchmarkWriteAndReadWithChanges()
        {



            var resetEventsLock = new object();
            var resetEvents = new Dictionary<int, ManualResetEvent>();


            for (var o = 0; o < ResetEventCount; o++)
            {
                int rootid;
                if (o == 3 || o == 7)
                {
                    rootid = resetEvents.Count + 1;

                    resetEvents.Add(rootid, new ManualResetEvent(false));

                    var clientWrite = new Client(GuidProvider.GetIdenticalGuid());
                    int rootid2 = rootid;
                    ThreadPool.QueueUserWorkItem(delegate
                    {

                        for (var i = 0; i <= ItemCount; i++)
                        {
                            var title = "Titel" + i;

                            clientWrite.Title = title;
                            //Console.WriteLine("WRITTEN: " + title);

                        }
                        clientWrite.Disconnect();
                        lock (resetEventsLock)
                        {
                            resetEvents[rootid2].Set();
                        }
                    }, rootid);

                }

                rootid = resetEvents.Count + 1;

                resetEvents.Add(rootid, new ManualResetEvent(false));

                var clientRead = new Client(GuidProvider.GetIdenticalGuid());
                int rootid1 = rootid;
                ThreadPool.QueueUserWorkItem(delegate
                {

                    for (var i = 0; i <= ItemCount; i++)
                    {
                        var title = clientRead.Title;
                        //Console.WriteLine("READ: " + title);
                    }
                    clientRead.Disconnect();
                    lock (resetEventsLock)
                    {
                        resetEvents[rootid1].Set();
                    }

                }, rootid);

            }

            var now = DateTime.Now;

            WaitHandle.WaitAll(resetEvents.Values.ToArray());

            var then = DateTime.Now;
            Console.WriteLine("Time:" + (then - now).TotalMilliseconds);

            Console.ReadLine();


        }
         */
    }
}
