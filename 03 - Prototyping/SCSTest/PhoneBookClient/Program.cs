using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using PhoneBookCommonLib;

namespace PhoneBookClient
{
    class Program
    {
        private const int ResetEventCount = 1;
        private const int ItemCount = 10000;

        static void Main(string[] args)
        {

            Benchmark(ItemCount);
            //TestCacheUpdates();
        }


        private static void TestCacheUpdates()
        {
            var client1 = new Client();
            var client2 = new Client();

            client1.Title = "ErsterTitel";

            Console.WriteLine("Client1.Title = " + client1.Title);
            Console.WriteLine("Client2.Title = " + client2.Title);

            client2.Title = "ZweiterTitel";

            Console.WriteLine("Client1.Title = " + client1.Title);
            Console.WriteLine("Client2.Title = " + client2.Title);

            client1.Disconnect();
            client2.Disconnect();

            Console.ReadLine();
        }

        private static void Benchmark(int count)
        {
            var client = new Client();


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


    }
}
