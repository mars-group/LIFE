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
            var pbS = new PhoneBookService(GuidProvider.GetIdenticalGuid());
            //Wait user to stop server by pressing Enter
            Console.WriteLine(
                "Phone Book Server started successfully. Press enter to stop...");
            Console.ReadLine();

            //Stop server
            pbS.Stop();
        }

    }
}
