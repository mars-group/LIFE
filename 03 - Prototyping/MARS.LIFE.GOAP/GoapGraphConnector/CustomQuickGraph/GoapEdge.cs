using System.Collections.Generic;
using CommonTypes.Interfaces;
using GoapCommon.Interfaces;
using QuickGraph;

namespace GoapGraphConnector.CustomQuickGraph {

    internal class GoapEdge<TVertex> : Edge<TVertex> {

        public GoapEdge(TVertex source, TVertex target, IAction action, int heuristic)
            : base(source, target) {
            Action = action;
            Heuristic = heuristic;
        }

        private IAction Action { get; set; }
        private int Heuristic { get; set; }

        public int GetHeuristic(List<IGoapWorldstate> targetWorldstates = null) {
            return Heuristic;
        }
    }
}
