using ASC.Communication.Scs.Communication.Messengers;

namespace ASC.Communication.Scs.Client {
    /// <summary>
    ///     Represents a client to connect to server.
    /// </summary>
    public interface IScsClient : IMessenger, IConnectableClient {
        //Does not define any additional member
    }
}