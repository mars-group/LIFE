using System;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     Indicates that the return type of a Method shall be cached by its stub.
    ///     Works only on Getter Methods.
    ///     Is not valid for void return types.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : Attribute {}
}