using System;
using System.Collections.Generic;
using System.Linq;

namespace GoapBetaCommon.Implementation
{
    public class Worldstate{

        private readonly Dictionary<int,WorldstateSymbol> _worldstate = new Dictionary<int, WorldstateSymbol>();
        private readonly Dictionary<Enum,int> _availableEnums = new Dictionary<Enum, int>();
        private readonly Type _enumType;

        /// <summary>
        /// create a structure where every symbol with the same representing enum is on the same position
        /// for faster comparisons
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="worldstateSymbols"></param>
        public Worldstate(Type enumType , List<WorldstateSymbol> worldstateSymbols ) {

            // create a dictionary indexed by enum int values with empty symbols
            _enumType  = enumType;
            foreach (Enum en in _enumType.GetEnumValues()){
                int enumNumber = Convert.ToInt32(en);
                _availableEnums.Add(en, enumNumber);
            }

           // insert exiting symbols
            foreach (var symbol in worldstateSymbols) {
                Enum en = symbol.EnumName;
                int index = _availableEnums[en];
                _worldstate.Add(index, symbol);
            }
        }

        private int GetIndex(WorldstateSymbol sym){
            return _availableEnums[sym.EnumName];
        }

        /// <summary>
        /// create a new dict with structure dictionary mapping the int of the enumName to the symbol itself
        /// </summary>
        /// <param name="worldstateSymbols"></param>
        /// <returns></returns>
        private Dictionary<int, WorldstateSymbol> GetDictIndexByEnum(List<WorldstateSymbol> worldstateSymbols) {
            Dictionary<int,WorldstateSymbol> res = new Dictionary<int, WorldstateSymbol>();
            foreach (var symbol in worldstateSymbols) {
                int index = GetIndex(symbol);
                if (res.ContainsKey(index)) {
                    throw new ArgumentException("one symbol key is at least double used for worldstate");
                }
                res.Add(index, symbol);
                
            }
            return res;
        }

        /// <summary>
        /// get a new worldstate by the present and the added symbols 
        /// </summary>
        /// <param name="newSymbols"></param>
        /// <returns></returns>
        public Worldstate AddSymbols(List<WorldstateSymbol> newSymbols) {
            var dict = GetDictIndexByEnum(newSymbols);
            List<int> newKeys = dict.Keys.ToList();
            
            foreach (int index in newKeys){
                if (_worldstate.ContainsKey(index)) {
                    throw new ArgumentException("worldstate already contains elem to insert");
                }
            }
            newSymbols.AddRange(_worldstate.Values);
            return new Worldstate(_enumType, newSymbols);
        }

        /// <summary>
        /// get new worldstate with maybe replaced states
        /// </summary>
        /// <param name="newSymbols"></param>
        /// <returns></returns>
        public Worldstate AddOrReplaceSymbols(List<WorldstateSymbol> newSymbols){
            var dict = GetDictIndexByEnum(newSymbols);
            Dictionary<int, WorldstateSymbol> newWorld = new Dictionary<int, WorldstateSymbol>(_worldstate);

            foreach (var entry in dict) {
                newWorld.Add(entry.Key, entry.Value);
            }
            
            return new Worldstate(_enumType, newWorld.Values.ToList());
        }





    }
}
