package de.haw.run.NetworkAdapter.Implementation;


import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.HostUnreachableException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Interface.IClientNetworkAdapter;
import de.haw.run.NetworkAdapter.Interface.MessageChannel;
import de.haw.run.NetworkAdapter.Messages.ClientAckMessage;
import de.haw.run.NetworkAdapter.Messages.ClientJoinMessage;
import de.haw.run.NetworkAdapter.Messages.ConnectionEndMessage;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;
import de.haw.run.GlobalTypes.Exceptions.ErrorMessages;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;

import java.io.IOException;
import java.net.InetAddress;
import java.net.Socket;
import java.net.UnknownHostException;
import java.security.InvalidParameterException;

class ClientNetworkAdapterUseCase
        extends AbstractNetworkAdapterUseCase
        implements IClientNetworkAdapter,
        NetworkChannel.NetworkMessageReceivedHandle<NetworkMessage> {

    private NetworkChannel controlChannel;
    private NetworkChannel dataChannel;

    private boolean connected;
    private int clientId;

    private ErrorMessages errorMessages;

    public ClientNetworkAdapterUseCase() {
        connected = false;
        clientId = -1;
        errorMessages = new ErrorMessages();
    }

    @Override
    public boolean isConnected() {
        return connected;
    }

    @Override
    public void connectToServer(String hostname, int port, String clientName) throws HostUnreachableException, InvalidParameterException, TechnicalException {
        InetAddress address;
        Socket socket;
        NetworkAccessProtocol protocol;

        try {
            //try to open the control port
            address = InetAddress.getByName(hostname);
            socket = new Socket(address, port);
            protocol = new NetworkAccessProtocol(socket);

            // say hello and wait for ack
            protocol.writeMessage(new ClientJoinMessage(-1, clientName));
            NetworkMessage message = protocol.readMessage();

            //if that is successful establish control channel and try to open data connection
            if (message instanceof ClientAckMessage) {

                clientId = message.getClientId();
                controlChannel = new NetworkChannel<NetworkMessage>(protocol, this);
                controlChannel.setDaemon(true);
                controlChannel.start();

                socket = new Socket(address, port + 1);
                protocol = new NetworkAccessProtocol(socket);

                // say hello and wait for ack
                protocol.writeMessage(new ClientJoinMessage(clientId, clientName));
                message = protocol.readMessage();

                //if that worked, too, we can establish the data connection and are done
                if (message instanceof ClientAckMessage) {
                    dataChannel = new NetworkChannel<NetworkMessage>(protocol, this);
                    dataChannel.setDaemon(true);
                    dataChannel.start();
                } else { // abort (we were so close..) :(
                    throw new HostUnreachableException();
                }

            } else { // else abort :(
                throw new HostUnreachableException();
            }

        } catch (UnknownHostException e) {
            throw new HostUnreachableException();
        } catch (IOException e) {
            throw new TechnicalException(errorMessages.get("networkError"));
        } catch (ConnectionLostException e) {
            throw new HostUnreachableException();
        }

        connected = true;
    }

    @Override
    public int getClientId() throws NotConnectedException {
        if (!connected) {
            throw new NotConnectedException();
        }

        return clientId;
    }

    @Override
    public synchronized void sendNetworkMessage(NetworkMessage message, MessageChannel channel) throws NotConnectedException, ConnectionLostException {
        if (channel == MessageChannel.DATA) {
            dataChannel.sendNetworkMessage(message);
        } else {
            controlChannel.sendNetworkMessage(message);
        }
    }

    @Override
    public void disconnect() {
        connected = false;
        try {
            ConnectionEndMessage bye = new ConnectionEndMessage(clientId, "clientDisconnecting");
            controlChannel.sendNetworkMessage(bye);
            dataChannel.sendNetworkMessage(bye);
        } catch (ConnectionLostException e) {
        } //closing anyway
    }

    @Override
    public void handleMessage(NetworkMessage message) throws NotConnectedException, ConnectionLostException {
        if (!(message instanceof ConnectionEndMessage)) {
            informSubscribers(message);
        } else {
            dataChannel.forceClose();
            controlChannel.forceClose();
        }
    }

    @Override
    public void handleException(Exception exception) {

    }
}