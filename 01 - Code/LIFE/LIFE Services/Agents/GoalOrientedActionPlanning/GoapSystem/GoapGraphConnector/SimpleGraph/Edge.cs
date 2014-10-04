using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.SimpleGraph {
    public class Edge : IGoapEdge {

        private readonly IGoapNode _source;
        private readonly IGoapNode _target;
        private readonly AbstractGoapAction _action;
        private readonly string _name;


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

        public int GetCost() {
            return _action.GetExecutionCosts();
        }

        public override string ToString() {
            return string.Format("Edge: |{0} -> {1}| ", _source, _target);
        }

      
    }
}