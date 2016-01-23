using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Addins;

namespace MonoAddins
{
    [TypeExtensionPoint]
    public interface ICommand
    {
        void Run();
    }
}
