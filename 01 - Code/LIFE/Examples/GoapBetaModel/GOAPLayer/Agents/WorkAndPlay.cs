using System;
using System.Collections.Generic;
using GoapBetaActionSystem.Implementation;
using GoapBetaCommon.Abstract;
using GoapBetaCommon.Implementation;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace GOAPBetaLayer.Agents {

    internal class WorkAndPlay : IAgent {
        private readonly Blackboard _blackboard;
        private readonly AbstractGoapSystem _goapActionSystem;

        public WorkAndPlay(string agentConfigFileName, string namespaceOfModelDefinition) {
            _blackboard = new Blackboard();
            _goapActionSystem = GoapComponent.LoadGoapConfiguration
                (agentConfigFileName, namespaceOfModelDefinition, _blackboard);
        }

        #region IAgent Members

        public void Tick() {
            AbstractGoapAction a = _goapActionSystem.GetNextAction();
            ExecuteAction(a);
        }

        #endregion

        private void ExecuteAction(AbstractGoapAction action) {
            Console.WriteLine(action.GetType() + " is now executed");
            List<WorldstateSymbol> curr = _blackboard.Get(AbstractGoapSystem.Worldstate);
            List<WorldstateSymbol> result = action.GetResultingWorldstate(curr);
            _blackboard.Set(AbstractGoapSystem.Worldstate, result);
        }
    }

}