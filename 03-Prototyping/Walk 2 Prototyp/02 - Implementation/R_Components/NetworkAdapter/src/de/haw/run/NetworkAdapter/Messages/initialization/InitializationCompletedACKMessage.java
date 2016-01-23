package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.util.UUID;

public class InitializationCompletedACKMessage extends NetworkMessage {


    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public InitializationCompletedACKMessage(int clientId) {
        super(clientId);
    }

}