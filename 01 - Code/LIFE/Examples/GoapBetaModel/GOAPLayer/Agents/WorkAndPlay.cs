using System;
using GoapBetaActionSystem.Implementation;
using GoapBetaCommon.Abstract;
using LayerAPI.Interfaces;
using TypeSafeBlackboard;

namespace GOAPBetaLayer.Agents
{
    class WorkAndPlay : IAgent {

        private readonly Blackboard _blackboard;
        private readonly AbstractGoapSystem _goapActionSystem;

        public WorkAndPlay(string agentConfigFileName, string namespaceOfModelDefinition)
        {
            _blackboard = new Blackboard();
            _goapActionSystem = GoapComponent.LoadGoapConfiguration(agentConfigFileName, namespaceOfModelDefinition, _blackboard);
        }

        public void Tick() {
            var a = _goapActionSystem.GetNextAction();
            ExecuteAction(a);
        }

        private void ExecuteAction(AbstractGoapAction action)
        {
            Console.WriteLine(action.GetType() + " is now executed");
            var curr = _blackboard.Get(AbstractGoapSystem.Worldstate);
            var result = action.GetResultingWorldstate(curr);
            _blackboard.Set(AbstractGoapSystem.Worldstate, result);
        }
    }
}
