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
            for (int i = 0; i < 10000; i++) { spaceProxy.Write(new Person { Ssn = i, FirstName = "Vincent", LastName = "Chase" }); }

            spaceProxy.Write(new Person { Ssn = 2, FirstName = "Johnny", LastName = "Drama" });

            Console.WriteLine("Read (retrieve) an entry from the grid by its id:");
            var result1 = spaceProxy.ReadById<Person>(1);

            Console.WriteLine("Read an entry from the grid using LINQ:");
            var query = from p in spaceProxy.Query<Person>()
                        where p.FirstName == "Johnny"
                        select p;

            var result2 = spaceProxy.Read<Person>(query.ToSpaceQuery());

            Console.WriteLine("Read all entries of type Person from the grid:");
            var now = DateTime.Now;
            Person[] results = spaceProxy.ReadMultiple(new Person());
            var then = DateTime.Now;
            Console.WriteLine("time spent: " + (then-now));
            Console.ReadLine();
        }
    }
}
