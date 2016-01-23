package de.haw.run.NetworkAdapter.Interface;



import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.HostUnreachableException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;

import java.security.InvalidParameterException;

/**
 * This interface defines the possibilities for AI Clients to control their connection to the GameServer.
 */
public interface IClientNetworkAdapter {
    /**
     * True, if an active connection to a GameServer is established.
     * @return
     */
    public boolean isConnected();

    /**
     * Tries to connect to the given IP or hostname.
     * @param hostname the host to connect to (IPv4, IPv6 address or hostname), != null
     * @param port the server's main port to connect to, != null
     * @param clientName not empty, != null
     * @throws java.security.InvalidParameterException if the hostname or the port weren't valid
     */
    public void connectToServer(String hostname, int port, String clientName) throws HostUnreachableException, InvalidParameterException, TechnicalException;

    /**
     * Returns the id that was assigned by the server.
     * @return >= 0
     * @throws de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException if no server connection was established.
     */
    public int getClientId() throws NotConnectedException;

    /**
     * This method can be used to subscribe for events (in the form of INetworkMessages) from the network.
     * @param eventHandler - A class implementing the INetworkMessageReceivedEventHandler interface
     * @param messageType the class object of the message type
     */
    public void subscribeForNetworkMessageReceivedEvent(INetworkMessageReceivedEventHandler eventHandler, Class messageType);

    /**
     * Sends a message via the network.
     * @param message != null
     * @param channel the network channel on which to send the message
     * @throws NotConnectedException if no active connection was in place
     * @throws de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException if the connection to the server was aborted unexpectedly
     */
    public void sendNetworkMessage(NetworkMessage message, MessageChannel channel) throws NotConnectedException, ConnectionLostException;

    /**
     *  Returns the hostname, with which the client is known to the server.
     *  @throws NotConnectedException if not connected to a server.
     * @return see summary, not null
     */
    // public String getHostname() throws NotConnectedException;

    /**
     * Disconnects from the server, if a connection is established. Does nothing else.
     */
    public void disconnect();
}
