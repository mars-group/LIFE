package de.haw.run.NetworkAdapter.Messages;

import de.haw.run.GlobalTypes.TransportTypes.TLayerContainerStatusType;

public class PushLayerContainerStatusMessage extends NetworkMessage {
	private TLayerContainerStatusType status;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param status
     */
    public PushLayerContainerStatusMessage(int clientId, TLayerContainerStatusType status) {
        super(clientId);
        this.status = status;
    }
}