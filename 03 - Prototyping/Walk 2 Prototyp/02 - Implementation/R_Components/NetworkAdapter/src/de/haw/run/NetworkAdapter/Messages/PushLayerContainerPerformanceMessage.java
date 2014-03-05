package de.haw.run.NetworkAdapter.Messages;

import de.haw.run.GlobalTypes.TransportTypes.TLayerContainerPerformance;


public class PushLayerContainerPerformanceMessage extends NetworkMessage {
	private TLayerContainerPerformance performance;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param performance
     */
    public PushLayerContainerPerformanceMessage(int clientId, TLayerContainerPerformance performance) {
        super(clientId);
        this.performance = performance;
    }
}