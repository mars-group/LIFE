package de.haw.run.NetworkAdapter.Messages.execution;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class StartSimulationMessage extends NetworkMessage {
	private long startTime;
	private long tickLength;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param startTime
     * @param tickLength
     */
    public StartSimulationMessage(int clientId, long startTime, long tickLength) {
        super(clientId);
        this.startTime = startTime;
        this.tickLength = tickLength;
    }

    public long getTickLength() {
        return tickLength;
    }

    public long getStartTime() {
        return startTime;
    }
}