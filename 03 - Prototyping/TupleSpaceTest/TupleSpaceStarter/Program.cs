using System;
using System.Linq;
using GigaSpaces.Core;
using GigaSpaces.Core.Linq;

namespace TupleSpaceStarter
{
    class Program
    {
        static void Main(string[] args)
        {
            var spaceProxy = GigaSpacesFactory.FindSpace("jini://*/*/myGrid?groups=$(XapNet.Groups)");


            Console.WriteLine("Write (store) a couple of entries in the data grid:");
            spaceProxy.Write(new Person { Ssn = 1, FirstName = "Vincent", LastName = "Chase" });
            spaceProxy.Write(new Person { Ssn = 2, FirstName = "Johnny", LastName = "Drama" });

            Console.WriteLine("Read (retrieve) an entry from the grid by its id:");
            var result1 = spaceProxy.ReadById<Person>(1);

            Console.WriteLine("Read an entry from the grid using LINQ:");
            var query = from p in spaceProxy.Query<Person>()
                        where p.FirstName == "Johnny"
                        select p;
            var result2 = spaceProxy.Read<Person>(query.ToSpaceQuery());

            Console.WriteLine("Read all entries of type Person from the grid:");
            Person[] results = spaceProxy.ReadMultiple(new Person());
        }
    }
}
