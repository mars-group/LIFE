using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Edge : IGoapEdge {
        private readonly IGoapNode _source;
        private readonly IGoapNode _target;
        private readonly int _cost;
        private readonly string _name;

       
        public Edge(int cost, IGoapNode source, IGoapNode target, string name = "NotNamedEdge") {
            _cost = cost;
            _source = source;
            _target = target;
            _name = name;
        }

        public IGoapNode GetSource() {
            return _source;
        }

        public IGoapNode GetTarget() {
            return _target;
        }

        public int GetCost() {
            return _cost;
        }

        public string Name
        {
            get { return _name; }
        }    

        public override string ToString() {
            return string.Format("Edge: |{0} -> {1}| ", _source, _target);
        }

      
    }
}