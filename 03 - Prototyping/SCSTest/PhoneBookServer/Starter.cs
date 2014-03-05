using System;
using Hik.Communication.Scs.Communication.EndPoints.Tcp;
using Hik.Communication.ScsServices.Service;
using PhoneBookCommonLib;

namespace PhoneBookServer
{
    class Starter
    {

        static void Main(string[] args)
        {
            //Create a Scs Service application that runs on 10048 TCP port.
            //var server = ScsServiceBuilder.CreateService(new ScsTcpEndPoint(10048));

            //Add Phone Book Service to service application

            //server.AddService<IPhoneBookService, PhoneBookService>(new PhoneBookService());

            //Start server
            //server.Start();

            var pbS = new PhoneBookService();
            //Wait user to stop server by pressing Enter
            Console.WriteLine(
                "Phone Book Server started successfully. Press enter to stop...");
            Console.ReadLine();

            //Stop server
            pbS.Stop();
        }

    }
}
