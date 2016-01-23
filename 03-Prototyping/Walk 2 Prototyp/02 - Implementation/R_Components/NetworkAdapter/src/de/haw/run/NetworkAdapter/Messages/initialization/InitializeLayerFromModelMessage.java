package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.layerAPI.TLayerInitializationDataType;

import java.util.UUID;

public class InitializeLayerFromModelMessage extends NetworkMessage {
	private UUID layerID;
	private TLayerInitializationDataType layerInitData;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param layerID
     * @param layerInitData
     */
    public InitializeLayerFromModelMessage(int clientId, UUID layerID, TLayerInitializationDataType layerInitData) {
        super(clientId);
        this.layerID = layerID;
        this.layerInitData = layerInitData;
    }

    public TLayerInitializationDataType getLayerInitData() {
        return layerInitData;
    }

    public UUID getLayerID() {
        return layerID;
    }
}