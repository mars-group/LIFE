package de.haw.run.NetworkAdapter.Messages.execution;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class AdvanceOneTickMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public AdvanceOneTickMessage(int clientId) {
        super(clientId);
    }
}