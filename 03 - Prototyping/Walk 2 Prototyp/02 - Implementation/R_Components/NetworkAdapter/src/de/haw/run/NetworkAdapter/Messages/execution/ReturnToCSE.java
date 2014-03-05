package de.haw.run.NetworkAdapter.Messages.execution;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class ReturnToCSE extends NetworkMessage {
	private long currentTickCount;
	private long currentSAPDTime;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param currentTickCount
     * @param currentSAPDTime
     */
    public ReturnToCSE(int clientId, long currentTickCount, long currentSAPDTime) {
        super(clientId);
        this.currentTickCount = currentTickCount;
        this.currentSAPDTime = currentSAPDTime;
    }
}