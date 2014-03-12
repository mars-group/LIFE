

using System;
using Mono.Addins;
using MonoAddins;

[assembly:Addin]
[assembly:AddinDependency("HelloWorld", "1.0")]

namespace Extension
{
    [Extension]
    public class HelloCommand : ICommand
    {
        public void Run()
        {
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
