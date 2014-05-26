using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using LCConnector.TransportTypes;

namespace ModelContainer.Implementation.Entities {
    internal class ModelNode : IEquatable<ModelNode> {
        private IList<ModelNode> _edges;
        public TLayerDescription LayerDescription { get; protected set; }

        public Type Layer { get; protected set; }

        protected IReadOnlyCollection<Type> Dependencies { get; set; }

        public IReadOnlyCollection<ModelNode> Edges {
            get { return new ReadOnlyCollection<ModelNode>(_edges); }
        }

        public ModelNode(TLayerDescription layerDescription, Type layer, Type[] dependencies) {
            LayerDescription = layerDescription;
            Layer = layer;
            Dependencies = dependencies;

            _edges = new List<ModelNode>();
        }

        /// <summary>
        /// Checks whether this node depends on newNode
        /// and adds an edge if true
        /// </summary>
        /// <param name="newNode"></param>
        public void UpdateEdges(ModelNode newNode)
        {
            if (Dependencies.Any(d => d == newNode.Layer)) {
                _edges.Add(newNode);
            }
        }

        public bool Equals(ModelNode other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(this.LayerDescription, other.LayerDescription) && Equals(this.Layer, other.Layer) && Equals(this._edges, other._edges);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ModelNode)obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (this.LayerDescription != null ? this.LayerDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.Layer != null ? this.Layer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this._edges != null ? this._edges.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ModelNode left, ModelNode right) {
            return Equals(left, right);
        }

        public static bool operator !=(ModelNode left, ModelNode right) {
            return !Equals(left, right);
        }
    }
}