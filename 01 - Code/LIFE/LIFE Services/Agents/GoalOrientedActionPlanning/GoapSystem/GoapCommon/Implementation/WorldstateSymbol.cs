using System;

namespace GoapCommon.Implementation {

    /// <summary>
    ///     struct offers call by value.
    ///     represents one world state in the goap action system.
    /// </summary>
    public struct WorldstateSymbol {
        public readonly Enum EnumName;
        public readonly ValueType Value;

        /// <summary>
        ///     the expected type is needed because of available integral types for numbers
        /// </summary>
        public readonly Type TypeOfValue;

        public WorldstateSymbol(Enum enKey, ValueType value, Type typeOfValue) {
            EnumName = enKey;
            Value = value;
            TypeOfValue = typeOfValue;
        }

        public override string ToString() {
            return string.Format("Key: {0}, Value: {1}, Type: {2}", EnumName, Value, TypeOfValue);
        }
    }

}