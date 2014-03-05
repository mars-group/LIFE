package de.haw.run.NetworkAdapter.Messages;

public class GetLayerContainerPerformanceMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public GetLayerContainerPerformanceMessage(int clientId) {
        super(clientId);
    }
}