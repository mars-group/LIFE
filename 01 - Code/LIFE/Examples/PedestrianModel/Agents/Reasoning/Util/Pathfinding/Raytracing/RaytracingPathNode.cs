using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace PedestrianModel.Agents.Reasoning.Util.Pathfinding.Raytracing
{

	/// <summary>
	/// @author Christian Thiel
	/// 
	/// </summary>
	public class RaytracingPathNode : IPathNode<Vector3D>
	{

		/// <summary>
		/// The underlying position.
		/// </summary>
		private readonly Vector3D obj;

		/// <summary>
		/// The predecessor of this node in the path. If null, this node has no predecessor.
		/// </summary>
		private RaytracingPathNode predecessor = null;

		/// <summary>
		/// Creates a new LoDNode with the specified Vector3D as source.
		/// </summary>
		/// <param name="obj"> the underlying vector </param>
		public RaytracingPathNode(Vector3D obj)
		{
			this.obj = obj;
		}

		public Vector3D AdaptedObject
		{
			get
			{
				return this.obj;
			}
		}

		public IPathNode<Vector3D> Predecessor
		{
			get
			{
				return this.predecessor;
			}
			set
			{
				this.predecessor = (RaytracingPathNode) value;
			}
		}


		public sealed override int GetHashCode()
		{
			//long x = Math.Round(obj.X * 100);
			//long y = Math.Round(obj.Y * 100);
			//long z = Math.Round(obj.Z * 100);
            long x = (long)Math.Round(obj.X * 100);
            long y = (long)Math.Round(obj.Y * 100);
            long z = (long)Math.Round(obj.Z * 100);

			const int prime = 31;
			int result = 1;
			result = prime * result + (int)(x ^ ((long)((ulong)x >> 32)));
			result = prime * result + (int)(y ^ ((long)((ulong)y >> 32)));
			result = prime * result + (int)(z ^ ((long)((ulong)z >> 32)));
			return result;
		}

		public sealed override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}

			return this.GetHashCode() == obj.GetHashCode();
		}

		public override sealed string ToString()
		{
			return obj.ToString();
		}
	}

}