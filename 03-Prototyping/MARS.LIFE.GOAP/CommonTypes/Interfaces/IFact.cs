using System;
using System.Collections.Generic;

namespace CommonTypes.Interfaces
{
    public interface IFact {

        /// <summary>
        /// get the identifier of the information
        /// the identifier marks the contract of saved data
        /// </summary>
        /// <returns>Enum</returns>
        Enum GetFactType();

        /// <summary>
        /// the content of informations collected in the working memory
        /// </summary>
        /// <returns>IDictionary</returns>
        IDictionary<String, String> GetFactContent();



    }
}
