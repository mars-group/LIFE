
using Mono.Addins;

[assembly:AddinRoot("HelloWorld" ,"1.0")]

namespace MonoAddins
{
    class Program
    {
        static void Main(string[] args)
        {
            AddinManager.Initialize ("./addinRegistry");
		    AddinManager.Registry.Update ();
		
		    foreach (var cmd in AddinManager.GetExtensionObjects<ICommand> ()){
			    cmd.Run ();
	        }
        }
    }


}
