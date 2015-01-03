using ASC.Communication.Scs.Communication.Messengers;
using ASC.Communication.Scs.Server;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     This class is used to create service client objects that is used in server-side.
    /// </summary>
    internal static class AscServiceClientFactory {
        /// <summary>
        ///     Creates a new service client object that is used in server-side.
        /// </summary>
        /// <param name="serverClient">Underlying server client object</param>
        /// <param name="requestReplyMessenger">RequestReplyMessenger object to send/receive messages over serverClient</param>
        /// <returns></returns>
        public static IAscServiceClient CreateServiceClient(IAscServerClient serverClient,
            RequestReplyMessenger<IAscServerClient> requestReplyMessenger) {
            return new AscServiceClient(serverClient, requestReplyMessenger);
        }
    }
}