using System;
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
