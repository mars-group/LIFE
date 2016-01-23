package de.haw.run.NetworkAdapter.Messages.execution;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class StartSimulationACKMessage extends NetworkMessage {
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public StartSimulationACKMessage(int clientId) {
        super(clientId);
    }
}