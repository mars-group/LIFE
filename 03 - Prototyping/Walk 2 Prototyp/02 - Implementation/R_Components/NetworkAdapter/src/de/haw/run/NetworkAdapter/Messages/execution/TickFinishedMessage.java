package de.haw.run.NetworkAdapter.Messages.execution;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class TickFinishedMessage extends NetworkMessage {
	private long tickDuration;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param tickDuration
     */
    public TickFinishedMessage(int clientId, long tickDuration) {
        super(clientId);
        this.tickDuration = tickDuration;
    }

    public long getTickDuration() {
        return tickDuration;
    }
}