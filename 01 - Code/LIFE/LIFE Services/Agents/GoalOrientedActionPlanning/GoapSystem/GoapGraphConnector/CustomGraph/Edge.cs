using System;
using System.Collections.Generic;
using GoapCommon.Abstract;
using GoapCommon.Interfaces;

namespace GoapGraphConnector.CustomGraph {
    public class Edge : IGoapEdge {
        private readonly IGoapVertex _source;
        private readonly IGoapVertex _target;
        private readonly int _cost;
        private readonly string _name;

       
        public Edge(int cost, IGoapVertex source, IGoapVertex target, string name = "NotNamedEdge") {
            _cost = cost;
            _source = source;
            _target = target;
            _name = name;
        }

        public IGoapVertex GetSource() {
            return _source;
        }

        public IGoapVertex GetTarget() {
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