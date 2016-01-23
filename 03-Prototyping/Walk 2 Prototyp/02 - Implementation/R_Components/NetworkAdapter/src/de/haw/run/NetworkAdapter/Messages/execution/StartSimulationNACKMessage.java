package de.haw.run.NetworkAdapter.Messages.execution;


import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class StartSimulationNACKMessage extends NetworkMessage {


    private String cause;

    public StartSimulationNACKMessage(int clientId, String cause) {
        super(clientId);
        this.cause = cause;
    }

    public String getCause() {
        return cause;
    }
}
