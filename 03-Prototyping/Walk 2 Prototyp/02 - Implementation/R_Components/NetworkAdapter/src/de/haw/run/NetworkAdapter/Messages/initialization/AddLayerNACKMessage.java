package de.haw.run.NetworkAdapter.Messages.initialization;


import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

public class AddLayerNACKMessage extends NetworkMessage {

    private String reason;
    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param reason
     */
    public AddLayerNACKMessage(int clientId, String reason) {
        super(clientId);
        this.reason = reason;
    }

    public String getReason() {
        return reason;
    }
}
