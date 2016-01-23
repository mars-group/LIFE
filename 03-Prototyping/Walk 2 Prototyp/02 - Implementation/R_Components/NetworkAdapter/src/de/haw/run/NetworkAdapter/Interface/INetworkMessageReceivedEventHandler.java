package de.haw.run.NetworkAdapter.Interface;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

/**
 * This interface defines the signature of eventListeners for network messages.
 */
public interface INetworkMessageReceivedEventHandler<M extends NetworkMessage> {

    /**
     * this method will be called on event listeners, when a network message arrives
     *
     * @param message
     */
    public void onMessageReceived(M message);

    public void onNetworkEvent(NetworkEventType networkEventType, int clientID);
}