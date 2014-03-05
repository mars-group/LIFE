package de.haw.run.NetworkAdapter.Messages;

import java.net.InetAddress;

public class ClientJoinMessage extends NetworkMessage {
    private String agentName;
	private InetAddress address;

    public ClientJoinMessage(int clientId, String agentName) {
        super(clientId);

        this.agentName = agentName;
    }

	public ClientJoinMessage(int clientId, String agentName, InetAddress address) {
		super(clientId);
        this.agentName = agentName;
        this.address = address;
    }

	public String getAgentName() {
		return this.agentName;
	}

	public InetAddress getAddress() {
		return this.address;
	}
}