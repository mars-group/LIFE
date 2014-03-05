package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class InitializationCompletedNACKMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public InitializationCompletedNACKMessage(int clientId) {
        super(clientId);
    }
}