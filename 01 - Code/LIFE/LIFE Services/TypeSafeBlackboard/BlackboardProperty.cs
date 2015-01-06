using System;

namespace TypeSafeBlackboard {
    
    /// <summary>
    ///     Licence: CPOL 1.02
    ///     auhor Antonio Nakić Alfirević 
    ///     published at http://www.codeproject.com/Articles/451326/Type-safe-blackboard-property-bag
    ///     Strongly typed property identifier for properties on a blackboard
    /// </summary>
    /// <typeparam name="T">The type of the property value it identifies</typeparam>
    public class BlackboardProperty<T> {
        /// <summary>
        ///     The name of the property.
        ///     <remarks>
        ///         Properties on the blackboard are stored by name, use caution NOT to have different
        ///         properties using the same name, as they will overwrite each others values if used on
        ///         the same blackboard.
        ///     </remarks>
        /// </summary>
        public string Name { get; set; }

        //factory method used to provide a default value when a blackboard 
        //does not contain data for this property
        private readonly Func<T> _createDefaultValueFunc;

        public BlackboardProperty(string name)
            : this(name, default(T)) {}

        /// <summary>
        /// </summary>
        /// <param name="name"></param>
        /// <param name="defaultValue">
        ///     The value which will be returned if the blackboard does not
        ///     contain an entry for this property.
        ///     <remarks>
        ///         Use this constructor if the default value is a constant or a value type.
        ///     </remarks>
        /// </param>
        public BlackboardProperty(string name, T defaultValue) {
            Name = name;
            _createDefaultValueFunc = () => defaultValue;
        }

        /// <summary>
        /// </summary>
        /// <remarks>
        ///     Use this constructor if the default value is a reference type, and you
        ///     do not want to share the same instance across multiple blackboards.
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="createDefaultValueFunc"></param>
        public BlackboardProperty(string name, Func<T> createDefaultValueFunc) {
            Name = name;
            _createDefaultValueFunc = createDefaultValueFunc;
        }

        public BlackboardProperty() {
            Name = Guid.NewGuid().ToString();
        }

        public T GetDefault() {
            return _createDefaultValueFunc();
        }
    }
}