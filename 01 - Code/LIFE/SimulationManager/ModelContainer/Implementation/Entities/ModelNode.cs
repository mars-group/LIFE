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
        /// 
        /// </summary>
        /// <param name="newNode"></param>
        public void UpdateEdges(ModelNode newNode)
        {
            if (Dependencies.Any(d => d == newNode.Layer)) {
                _edges.Add(newNode);
            }
        }

        #region Object contracts

        public bool Equals(ModelNode other) {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(LayerDescription, other.LayerDescription) && Equals(Layer, other.Layer) &&
                   Equals(Edges, other.Edges);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((ModelNode) obj);
        }

        public override int GetHashCode() {
            unchecked {
                int hashCode = (LayerDescription != null ? LayerDescription.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Layer != null ? Layer.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Edges != null ? Edges.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static bool operator ==(ModelNode left, ModelNode right) {
            return Equals(left, right);
        }

        public static bool operator !=(ModelNode left, ModelNode right) {
            return !Equals(left, right);
        }

        #endregion
    }
}