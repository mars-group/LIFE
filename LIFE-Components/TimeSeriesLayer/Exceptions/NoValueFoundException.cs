using System;
using System.Runtime.Serialization;

namespace LIFE.Components.TimeSeriesLayer.Exceptions {

	[Serializable]
	public class NoValueFoundException : Exception {

		public NoValueFoundException(String msg) : base(msg) { }

		public NoValueFoundException(SerializationInfo serializationInfo, StreamingContext context) { }
	}
}

