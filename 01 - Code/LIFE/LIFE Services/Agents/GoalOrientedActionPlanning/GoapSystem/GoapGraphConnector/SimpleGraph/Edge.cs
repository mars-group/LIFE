using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class Edge : IGoapEdge {

        private readonly IGoapNode _source;
        private readonly IGoapNode _target;
        private readonly AbstractGoapAction _action;

        public Edge(AbstractGoapAction action,  IGoapNode source, IGoapNode target){
            _action = action;
            _source = source;
            _target = target;
        }

        public IGoapNode GetSource(){
            return _source;
        }

        public IGoapNode GetTarget(){
            return _target;
        }

        public AbstractGoapAction GetAction() {
            return _action;
        }

        public int GetCost() {
            return _action.GetExecutionCosts();
        }

   

      
    }
}