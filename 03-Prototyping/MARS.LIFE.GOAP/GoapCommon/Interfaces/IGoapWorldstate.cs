﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GoapCommon.Interfaces {
    /// <summary>
    ///     Represents one aspect of the world of one agent. Many of this informations configure
    ///     the whole world. Every agent got his own sight on the world from the sum of his aspects.
    /// </summary>
    public interface IGoapWorldstate /*: IEqualityComparer */{
        /// <summary>
        ///     return the enumerated symbol
        /// </summary>
        /// <returns>IWorldstateSymbol</returns>
        Enum GetWorldstateSymbol();

        /// <summary>
        ///     Get the corresponding boolean value
        /// </summary>
        /// <returns>bool</returns>
        bool IsValid();

        /// <summary>
        /// </summary>
        /// <returns></returns>
        IGoapWorldstate GetClone();
    }
}