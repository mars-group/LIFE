using System;

namespace GoapCommon.Interfaces {

    /// <summary>
    ///     Represents one aspect of the world of one agent. Many of this informations configure
    ///     the whole world. Every agent got his own sight on the world from the sum of his aspects.
    /// </summary>
    public interface IGoapWorldProperty /*: IEqualityComparer */ {
        /// <summary>
        ///     return the enumerated symbol
        /// </summary>
        /// <returns>IWorldstateSymbol</returns>
        Enum GetPropertyKey();

        /// <summary>
        ///     Get the corresponding boolean value
        /// </summary>
        /// <returns>bool</returns>
        bool IsValid();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IGoapWorldProperty GetClone();

        IGoapWorldProperty GetNegative();
    }

}