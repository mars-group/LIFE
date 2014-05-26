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

            // update all dependencies
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

            while (_nodes.Any()) {
                // get all satisfied nodes from _nodes
                var dependentNodes = _nodes.Where(
                    n => n.Edges.All(
                        e => setList.Any(s => s.Contains(e))
                        )
                    ).ToArray();

                // remove these from the _nodes collection
                foreach (var dependentNode in dependentNodes) {
                    _nodes.Remove(dependentNode);
                }

                // add nodes to final list
                setList.Add(new HashSet<ModelNode>(dependentNodes));
            }

            return setList.Aggregate(new List<TLayerDescription>(), (list, set) => {
                list.AddRange(set.Select(n => n.LayerDescription).ToList());
                return list;
            });
        }
    }
}