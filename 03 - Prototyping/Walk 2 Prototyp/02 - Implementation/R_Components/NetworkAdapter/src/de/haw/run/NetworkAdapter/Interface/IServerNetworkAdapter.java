package de.haw.run.NetworkAdapter.Interface;

import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.TransportTypes.TNetworkClient;

import java.util.List;

/**
 * This interface defines the access and control possibilities for TCP connections between the GameServer and its
 * AIClients.
 */
public interface IServerNetworkAdapter {

    /**
     * This method can be used to subscribe for events (in the form of INetworkMessages) from the network.
     * <br/>TODO: Killing an EventHandler without removing its subscription may result in runtime errors
     * @param eventHandler - A class implementing the INetworkMessageReceivedEventHandler interface
     * @param messageType the class object of the message type, != null
     */
	public <T extends NetworkMessage> void subscribeForNetworkMessageReceivedEvent(INetworkMessageReceivedEventHandler eventHandler, Class messageType);

    /**
     * @throws de.haw.run.GlobalTypes.Exceptions.TechnicalException if any severe technical exceptions occur, i.e. the port is blocked.
     */
    public void startHosting() throws TechnicalException, ConnectionLostException;

    /**
     * This method sends network messages to the client specified by the clientId in the network message
     * @param message the message to send
     * @param channel the channel on which to send the message
     * @throws de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException if no client with a clientId matching to the message's is connected to the server
     * @throws ConnectionLostException if sending the message failed, because the connection to the client was lost
     */
	public void sendNetworkMessage(NetworkMessage message, MessageChannel channel) throws NotConnectedException, ConnectionLostException;

	/**
	 * Gets a list of players containing all players that are currently connected to the server.
	 * @return All currently connected players.
	 */
	public List<TNetworkClient> getConnectedClients();

    /**
     * Stops all listening for new connections and closes all current connections.
     */
    public void stopHosting();
}