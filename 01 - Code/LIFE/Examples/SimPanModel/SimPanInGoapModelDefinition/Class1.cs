using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Text;
using System.Threading.Tasks;
using GoapCommon.Implementation;
using SimPanInGoapModelDefinition.Worldstates;

namespace SimPanInGoapModelDefinition
{
    

    class Class1
    {
        public static void Main() {
            
            var sym = new WorldstateSymbol(Worldstates.Properties.HasPath, false ,typeof(bool));

            
            var enumType = sym.EnumName.GetType();
            
            foreach (var i in enumType.GetEnumValues()) {
                Console.WriteLine(i);
                Console.WriteLine((int)i);
            }

            Console.WriteLine((int)Properties.HasPath);

            
            /*
            Console.WriteLine(enumType.GetEnumValues());
            
            Console.WriteLine(Enum.GetValues(sym.Key.GetType()));
            
            Console.WriteLine( sym.Value.Equals(true));
            Console.WriteLine(sym.TypeOfValue);

            var s = new WorldstateSymbol();
            var t = new WorldstateSymbol();



            Console.WriteLine(s);
            
            Console.WriteLine(s.Equals(t));
            

            
            Console.WriteLine(s.Key);
            Console.WriteLine(s.TypeOfValue);
            Console.WriteLine(s.Value);
             */

            Console.ReadKey();

        }
    }
}
