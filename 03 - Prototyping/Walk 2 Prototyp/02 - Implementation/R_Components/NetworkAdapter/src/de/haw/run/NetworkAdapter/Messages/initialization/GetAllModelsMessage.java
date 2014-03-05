package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class GetAllModelsMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public GetAllModelsMessage(int clientId) {
        super(clientId);
    }
}