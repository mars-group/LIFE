using System;

using GigaSpaces.Core.Metadata;

namespace TupleSpaceStarter
{
    public class Person
    {
        [SpaceID]
        public int? Ssn { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
    }
}
