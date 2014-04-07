using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppSettingsManager.Implementation;

namespace NiniTest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var configAdapter = new NiniAdapterImpl("Test", "../../TestConfig.xml");
            
            

            Console.WriteLine("the amazing test");

            Console.WriteLine(configAdapter.GetValue("Test"));

            Console.Read();


        }
    }
}
