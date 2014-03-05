package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.util.List;

public class PushAllModelsMessage extends NetworkMessage {
    private List<TSimulationModel> allModels;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param allModels
     */
    public PushAllModelsMessage(int clientId, List<TSimulationModel> allModels) {
        super(clientId);
        this.allModels = allModels;
    }

    public List<TSimulationModel> getAllModels() {
        return allModels;
    }
}