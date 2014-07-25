using System;
using CommonTypes.Interfaces;
using GoapActionSystem.Implementation;

namespace GoapActionSystemFactory.Implementation {

    /// <summary>
    /// main access to create an instance of the goap component
    /// </summary>
    public static class GoapComponent {
        
        /// <summary>
        /// singleton goap manager
        /// </summary>
        private static IActionSystem _goapManager;

        /// <summary>
        /// singleton goap manager per process
        /// </summary>
        /// <param name="owningAgent"></param>
        /// <returns>IActionSytem</returns>
        public static IActionSystem GetGoap(Object owningAgent) {
            if (_goapManager != null) return _goapManager;
            return _goapManager = new GoapManager();
        }
    }
}
