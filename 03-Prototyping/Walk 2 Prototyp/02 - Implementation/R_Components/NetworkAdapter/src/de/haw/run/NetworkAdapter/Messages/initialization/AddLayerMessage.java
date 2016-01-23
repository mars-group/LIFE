package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.net.URI;
import java.util.UUID;

public class AddLayerMessage extends NetworkMessage {
	private URI layerURI;
    private UUID layerID;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param layerURI
     * @param layerID
     */
    public AddLayerMessage(int clientId, URI layerURI, UUID layerID) {
        super(clientId);
        this.layerURI = layerURI;
        this.layerID = layerID;
    }

    public URI getLayerURI() {
        return layerURI;
    }

    public UUID getLayerID() {
        return layerID;
    }
}