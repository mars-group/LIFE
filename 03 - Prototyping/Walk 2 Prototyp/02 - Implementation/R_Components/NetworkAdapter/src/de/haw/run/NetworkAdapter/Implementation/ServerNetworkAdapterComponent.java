package de.haw.run.NetworkAdapter.Implementation;

import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Interface.INetworkMessageReceivedEventHandler;
import de.haw.run.NetworkAdapter.Interface.IServerNetworkAdapter;
import de.haw.run.NetworkAdapter.Interface.MessageChannel;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TNetworkClient;

import java.util.List;

public class ServerNetworkAdapterComponent implements IServerNetworkAdapter {

    ServerNetworkAdapterUseCase serverNetworkAdapterUseCase;

    public ServerNetworkAdapterComponent() throws TechnicalException, SettingException {
        serverNetworkAdapterUseCase = new ServerNetworkAdapterUseCase();
    }

    /**
     * <b>For unit testing ONLY!!!</b>
     * @param port
     */
    public ServerNetworkAdapterComponent(int port) {
        serverNetworkAdapterUseCase = new ServerNetworkAdapterUseCase(port);
    }

    @Override
    public void subscribeForNetworkMessageReceivedEvent(INetworkMessageReceivedEventHandler eventHandler, Class messageType) {
        serverNetworkAdapterUseCase.subscribeForNetworkMessageReceivedEvent(eventHandler, messageType);
    }

    @Override
    public void startHosting() throws TechnicalException, ConnectionLostException {
        serverNetworkAdapterUseCase.startHosting();
    }

    @Override
    public synchronized void sendNetworkMessage(NetworkMessage message, MessageChannel channel) throws NotConnectedException, ConnectionLostException {
        serverNetworkAdapterUseCase.sendNetworkMessage(message, channel);
    }

    @Override
    public List<TNetworkClient> getConnectedClients() {
        return serverNetworkAdapterUseCase.getConnectedClients();
    }

    @Override
    public void stopHosting() {
        serverNetworkAdapterUseCase.stopHosting();
    }
}