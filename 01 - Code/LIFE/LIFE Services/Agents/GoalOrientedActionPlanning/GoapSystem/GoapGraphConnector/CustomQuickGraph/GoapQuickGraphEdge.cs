using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;
using QuickGraph;

namespace GoapGraphConnector.CustomQuickGraph
{

    internal class GoapQuickGraphEdge<TVertex> : Edge<TVertex>, IGoapEdge
    {

        public GoapQuickGraphEdge(TVertex source, TVertex target, IAction action, int cost)
            : base(source, target)
        {
            Action = action;
            Cost = cost;
        }

        private IAction Action { get; set; }
        private int Cost { get; set; }


        public IGoapVertex GetSource() {
            throw new System.NotImplementedException();
        }

        public IGoapVertex GetTarget() {
            throw new System.NotImplementedException();
        }
    }
}
