package de.haw.run.NetworkAdapter.Messages;

/**
 * this message signals that an established connection is about to be closed by the sending side.
 */
public class ConnectionEndMessage extends NetworkMessage {

    private String messageCode;

    public ConnectionEndMessage(int clientId, String messageCode) {
        super(clientId);

        this.messageCode = messageCode;
    }

    public String getMessageCode() {
        return messageCode;
    }
}
