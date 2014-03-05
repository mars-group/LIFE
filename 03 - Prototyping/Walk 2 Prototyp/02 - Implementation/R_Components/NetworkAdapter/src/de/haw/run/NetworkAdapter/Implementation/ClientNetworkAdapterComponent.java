package de.haw.run.NetworkAdapter.Implementation;

import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.HostUnreachableException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Interface.IClientNetworkAdapter;
import de.haw.run.NetworkAdapter.Interface.INetworkMessageReceivedEventHandler;
import de.haw.run.NetworkAdapter.Interface.MessageChannel;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;

import java.security.InvalidParameterException;

/**
 * Represents the implementation of the IClientNetworkAdapterInterface.
 */
public class ClientNetworkAdapterComponent implements IClientNetworkAdapter {

    private ClientNetworkAdapterUseCase clientNetworkAdapterUseCase;

    public ClientNetworkAdapterComponent() {
        clientNetworkAdapterUseCase = new ClientNetworkAdapterUseCase();
    }

    @Override
    public boolean isConnected() {
        return clientNetworkAdapterUseCase.isConnected();
    }

    @Override
    public void subscribeForNetworkMessageReceivedEvent(INetworkMessageReceivedEventHandler eventHandler, Class messageType) {
        clientNetworkAdapterUseCase.subscribeForNetworkMessageReceivedEvent(eventHandler, messageType);
    }

    @Override
    public void sendNetworkMessage(NetworkMessage message, MessageChannel channel) throws NotConnectedException, ConnectionLostException {
        clientNetworkAdapterUseCase.sendNetworkMessage(message, channel);
    }

    @Override
    public void connectToServer(String address, int port, String clientName) throws HostUnreachableException, InvalidParameterException, TechnicalException {
        clientNetworkAdapterUseCase.connectToServer(address, port, clientName);
    }

    @Override
    public int getClientId() throws NotConnectedException {
        return clientNetworkAdapterUseCase.getClientId();
    }

    @Override
    public void disconnect() {
        clientNetworkAdapterUseCase.disconnect();
    }
}
