using ProtoBuf;

namespace LIFEViewProtocol.Helper {
	[ProtoContract]
	public class FloatArray2D {
		[ProtoMember(1)]
		private float[] Fields { get; set; }

		[ProtoMember(2)]
		private int Rows { get; set; }

		public FloatArray2D() {}

		public FloatArray2D(float[,] source) {
			Rows = source.GetLength(0);
			Fields = new float[source.GetLength(0)*source.GetLength(1)];
			for (int i = 0; i < source.GetLength(1); i++) {
				for (int j = 0; j < source.GetLength(0); j++) {
					Fields[i*source.GetLength(1) + j] = source[j, i];
				}
			}
		}

		public FloatArray2D(float[] source, int rows) {
			Rows = rows;
			Fields = source;
		}

		public float[,] Get2DFloats() {
			var y = Fields.Length/Rows;
			var array = new float[Rows, y];
			for (var i = 0; i < Rows; i++) {
				for (var j = 0; j < y; j++) {
					array[i, j] = Fields[i*y + j];
				}
			}
			return array;
		}
	}
}