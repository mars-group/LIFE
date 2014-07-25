using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.Interfaces;

namespace GoapGraphConnector.CustomQuickGraph
{
    public class GoapQuickGraphConnector : IGoapGraph
    {
       
       public IGoapGraph CreateGoapGraph(List<IGoapWorldstate> rootState, List<IGoapWorldstate> targetState, int maximumGraphDept = 0) {
            throw new NotImplementedException();
        }

        public bool HasCircles() {
            throw new NotImplementedException();
        }

        public bool IsGraphEmpty() {
            throw new NotImplementedException();
        }

        public IGoapVertex GetNextVertexOnWhiteList() {
            throw new NotImplementedException();
        }

        public bool HasNextVertex() {
            throw new NotImplementedException();
        }

        public bool ExpandVertex(IGoapVertex vertex) {
            throw new NotImplementedException();
        }

        public bool ExpandNextVertex() {
            throw new NotImplementedException();
        }

        public bool IsVertexTarget(IGoapVertex vertex)
        {
            throw new NotImplementedException();
        }
      

    }
}
