package de.haw.run.NetworkAdapter.Messages;

import de.haw.run.GlobalTypes.NodeType;

import java.net.InetAddress;

public class DiscoverMeMessage extends NetworkMessage {
	/**
	 * Description and/or name of the node
	 */
	private String identifier;
	/**
	 * Address of the node
	 */
	private InetAddress address;
	/**
	 * The port in which this node listens to protocol messages
	 */
	private int port;
	/**
	 * An enum describing the type of this node
	 */
	private NodeType nodeType;

    /**
     * creates a basic message.
     *
     * @param clientId see getClientID()
     * @param identifier
     * @param address
     * @param port
     * @param nodeType
     */
    public DiscoverMeMessage(int clientId, String identifier, InetAddress address, int port, NodeType nodeType) {
        super(clientId);
        this.identifier = identifier;
        this.address = address;
        this.port = port;
        this.nodeType = nodeType;
    }
}