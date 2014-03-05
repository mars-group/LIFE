package de.haw.run.NetworkAdapter.Messages;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 11:33
 */
public class AbortSimulationMessage extends NetworkMessage {

    private String message;

    public AbortSimulationMessage(int clientId, String s) {
        super(clientId);
        message = s;
    }

    public String getMessage() {
        return message;
    }
}
