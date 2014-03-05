package de.haw.run.NetworkAdapter.Messages;

public class GetLayerContainerStatusMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public GetLayerContainerStatusMessage(int clientId) {
        super(clientId);
    }
}