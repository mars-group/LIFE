using System;
using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapGraphConnector;
using GoapGraphConnector.CustomQuickGraph;

namespace GoapActionSystem.Implementation
{
    /// <summary>
    /// concrete class offering the neccessary methods
    /// </summary>
    public class GoapManager : IActionSystem
    {
        public GoapManager() {}


        public IAction GetNextAction() {
            throw new NotImplementedException();
        }

        public bool PushIActionToBlackboard() {
            throw new NotImplementedException();
        }

        


    }
}
