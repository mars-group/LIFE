using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using LCConnector.TransportTypes;

[assembly: InternalsVisibleTo("SimulationManagerTest")]

namespace ModelContainer.Implementation.Entities {
    /// <summary>
    ///     This class is an internal representation of a model structure.
    /// </summary>
    /// <remarks>It is essentially a graph and used to examine possible instantiation orders for the model's</remarks>
    internal class ModelStructure {
        private readonly ISet<ModelNode> _nodes;

        public ModelStructure() {
            _nodes = new HashSet<ModelNode>();
        }

        public void AddLayer(TLayerDescription layerDescription, Type nodeType, params Type[] dependencies) {
            ModelNode newNode = new ModelNode(layerDescription, nodeType, dependencies);
            _nodes.Add(newNode);

            foreach (var modelNode in _nodes) {
                modelNode.UpdateEdges(newNode);
                newNode.UpdateEdges(modelNode);
            }
        }

        public IList<TLayerDescription> CalculateInstantiationOrder() {
            /* This algorithm works backwards from nodes without any dependencies.
             * It works iteratively outwards on the graph, parallely tracking all possibilities.
             * For each iteration, we check if there is a node that depends on at least one of the previous ones.
             * If all of these node's dependencies are satisfied from the nodes within the result list, we can add them,
             * since they can now be instantiated. The algorithm ends, if there are no new dependent nodes found.
             */

            IList<HashSet<ModelNode>> setList = new List<HashSet<ModelNode>>();

            int i = 0;
            HashSet<ModelNode> nodes = new HashSet<ModelNode>(_nodes.Where(n => n.Edges.Count < 1).ToArray());
            while (nodes.Any()) {
                var dependentNodes = _nodes.Where(
                    n => n.Edges.All(
                        e => setList.Any(s => s.Contains(e))
                        )
                    ).ToArray();

                nodes = new HashSet<ModelNode>(dependentNodes);
                i++;
                setList[i] = nodes;

            }

            return setList.Aggregate(new List<TLayerDescription>(), (list, set) => {
                return set.Select(n => n.LayerDescription).ToList();
            });
        }
    }
}