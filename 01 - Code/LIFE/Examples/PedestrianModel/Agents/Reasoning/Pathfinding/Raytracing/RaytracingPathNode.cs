using System;
using GenericAgentArchitectureCommon.Datatypes;

namespace PedestrianModel.Agents.Reasoning.Pathfinding.Raytracing {

    public class RaytracingPathNode : IPathNode<Vector> {
        /// <summary>
        ///     The underlying position.
        /// </summary>
        private readonly Vector _obj;

        private readonly float _x;
        private readonly float _y;
        private readonly float _z;

        /// <summary>
        ///     The predecessor of this node in the path. If null, this node has no predecessor.
        /// </summary>
        private RaytracingPathNode _predecessor;

        /// <summary>
        ///     Creates a new LoDNode with the specified Vector3D as source.
        /// </summary>
        /// <param name="obj"> the underlying vector </param>
        public RaytracingPathNode(Vector obj) {
            _obj = obj;
            _x = _obj.X;
            _y = _obj.Y;
            _z = _obj.Z;
        }

        #region IPathNode<Vector> Members

        public Vector AdaptedObject { get { return _obj; } }

        public IPathNode<Vector> Predecessor {
            get { return _predecessor; }
            set { _predecessor = (RaytracingPathNode) value; }
        }


        public override sealed int GetHashCode() {
            long x = (long)Math.Round(_x * 100);
            long y = (long)Math.Round(_y * 100);
            long z = (long)Math.Round(_z * 100);

            const int prime = 31;
            int result = 1;
            result = prime*result + (int) (x ^ ((long) ((ulong) x >> 32)));
            result = prime*result + (int) (y ^ ((long) ((ulong) y >> 32)));
            result = prime*result + (int) (z ^ ((long) ((ulong) z >> 32)));
            return result;
        }

        public override sealed bool Equals(object obj) {
            if (this == obj) return true;
            if (obj == null) return false;
            if (GetType() != obj.GetType()) return false;

            var raytracingPathNode = obj as RaytracingPathNode;
            return raytracingPathNode != null && AdaptedObject.Equals(raytracingPathNode.AdaptedObject);
        }

        #endregion

        public override sealed string ToString() {
            return _obj.ToString();
        }
    }

}