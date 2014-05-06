using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhoneBookCommonLib
{
    public static class GuidProvider
    {

        private static readonly Guid Guid = Guid.NewGuid();

        public static Guid GetIdenticalGuid()
        {
            return Guid.Parse("1134a0c0-9d9a-436e-b7a5-3e49fa267eb4");
        }
    }
}
