using System;
using Hik.Communication.ScsServices.Service;
using PhoneBookCommonLib;

namespace PhoneBookServer
{
    internal class YetAnotherService : ScsService, IYetAnotherService
    {
        public YetAnotherService(Guid id) : base(id.ToByteArray())
        {
            
        }

        public string GetInformation()
        {
            return "Loads of information!";
        }
    }
}
