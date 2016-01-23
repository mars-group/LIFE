
using System;
using LayerAPI.AddinLoader;
using Mono.Addins;

[assembly:AddinRoot("HelloWorld" ,"1.0")]

namespace MonoAddins
{
    class Program
    {
        static void Main(string[] args)
        {
            var addinLoader = new AddinLoader();
            var exLayer = addinLoader.LoadLayer(typeof(ExampleLayer.ExampleLayer));
            var result = addinLoader.LoadLayer(typeof(AwesomeExampleLayer.AwesomeExampleLayer));
            var awesomeLayer = result.Type.GetConstructors()[0].Invoke(new []{exLayer.CreateInstance()});
            Console.WriteLine("result=" + awesomeLayer.GetType());
            Console.ReadLine();
        }
    }
}
