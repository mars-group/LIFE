using ProtoBuf;

namespace MessageWrappers {
	[ProtoContract]
	public class RegisterMessage : BasicVisualizationMessage {
		/// <summary>
		///     Determines, if the message is used to unregister the client.
		///     The default value is false.
		/// </summary>
		[ProtoMember(1)]
		public bool Unregister { get; set; }

		/// <summary>
		///     The GUID of the client.
		/// </summary>
		[ProtoMember(2)]
		public string Guid { get; set; }

		[ProtoMember(3)]
		public bool Transactional { get; private set; }

		protected RegisterMessage() {
/*			Unregister = false;
			Guid = null;*/
			GetInheritancePath();
		}

		/// <summary>
		///     Constructor for RegisterMessage. The message has to contain a GUID.
		/// </summary>
		/// <param name="unregister">True if you want to unregister the client</param>
		/// <param name="guid">The GUID of the client</param>
		/// <param name="transactional">Determines wether the simulation is updated transactionaly</param>
		public RegisterMessage(bool unregister, string guid, bool transactional = false) {
			Unregister = unregister;
			Guid = guid;
			Transactional = transactional;
			GetInheritancePath();
		}
	}
}