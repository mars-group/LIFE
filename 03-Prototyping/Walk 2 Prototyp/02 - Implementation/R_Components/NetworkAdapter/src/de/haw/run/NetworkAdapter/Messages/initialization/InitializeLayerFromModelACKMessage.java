package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.util.UUID;

public class InitializeLayerFromModelACKMessage extends NetworkMessage {
    private UUID layerID;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param layerID
     */
    public InitializeLayerFromModelACKMessage(int clientId, UUID layerID) {
        super(clientId);
        this.layerID = layerID;
    }

    public UUID getLayerID() {
        return layerID;
    }
}