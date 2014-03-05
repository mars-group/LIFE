package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.GlobalTypes.TransportTypes.TDistributionInformation;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.util.UUID;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 18:43
 */
public class InitializeSimulationMessage extends NetworkMessage {
    private TDistributionInformation distributionInformation;
    private UUID simModelID;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     */
    public InitializeSimulationMessage(int clientId, TDistributionInformation distributionInformation, UUID simModelID) {
        super(clientId);
        this.distributionInformation = distributionInformation;
        this.simModelID = simModelID;
    }

    public TDistributionInformation getDistributionInformation() {
        return distributionInformation;
    }

    public UUID getSimModelID() {
        return simModelID;
    }
}
