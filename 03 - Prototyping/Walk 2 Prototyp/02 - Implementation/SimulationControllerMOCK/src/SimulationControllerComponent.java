import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.Exceptions.ThisNodeNotSetException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TDistributionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.NetworkAdapter.Implementation.ClientNetworkAdapterComponent;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.HostUnreachableException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Interface.IClientNetworkAdapter;
import de.haw.run.NetworkAdapter.Interface.INetworkMessageReceivedEventHandler;
import de.haw.run.NetworkAdapter.Interface.MessageChannel;
import de.haw.run.NetworkAdapter.Interface.NetworkEventType;
import de.haw.run.NetworkAdapter.Messages.execution.StartSimulationMessage;
import de.haw.run.NetworkAdapter.Messages.initialization.GetAllModelsMessage;
import de.haw.run.NetworkAdapter.Messages.initialization.InitializationCompletedACKMessage;
import de.haw.run.NetworkAdapter.Messages.initialization.InitializeSimulationMessage;
import de.haw.run.NetworkAdapter.Messages.initialization.PushAllModelsMessage;
import de.haw.run.noderegistry.Interface.INodeRegistry;

import java.util.*;
import java.util.function.Consumer;


public class SimulationControllerComponent {

    private final IClientNetworkAdapter clientNetworkAdapter;
    private int clientID;
    private INodeRegistry nodeRegistry;

    public SimulationControllerComponent(INodeRegistry nodeRegistry){
        this.nodeRegistry = nodeRegistry;
        this.clientNetworkAdapter = new ClientNetworkAdapterComponent();
        TConnectionInformation conInf = nodeRegistry.getConnectionInformationByNodeName("SimCore");

        try {
            this.clientNetworkAdapter.connectToServer(conInf.getIpAddress(), conInf.getPort(), nodeRegistry.getThisNode().getIdentifier());
            this.clientID = clientNetworkAdapter.getClientId();
        } catch (HostUnreachableException | TechnicalException | NotConnectedException | ThisNodeNotSetException e) {
            e.printStackTrace();
        }


    }


    public void getSimulationModels(Consumer<PushAllModelsMessage> func){
        clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new PushAllModelsMessageReceiver(func), PushAllModelsMessage.class);
        try {
            clientNetworkAdapter.sendNetworkMessage(new GetAllModelsMessage(clientID), MessageChannel.DATA);
        } catch (NotConnectedException | ConnectionLostException e) {
            e.printStackTrace();
        }
    }

    public void initializeSimulation(TSimulationModel model, Consumer<Object> func) {

        Map<UUID, String> distriMap = new Hashtable<>();
        List<String> layerContainer = nodeRegistry.getIdentifiersByNodeType(NodeType.LAYERCONTAINER);
        Random rand = new Random();
        rand.setSeed(6101986);

        model.getLayers().forEach(layer -> {
            distriMap.put(layer.getLayerID(), layerContainer.get(rand.nextInt(layerContainer.size())));
        });

        TDistributionInformation distributionInfo = new TDistributionInformation(distriMap);

        try {
            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new InitializationCompletedACKMessageReceiver(func), InitializationCompletedACKMessage.class);
            clientNetworkAdapter.sendNetworkMessage(new InitializeSimulationMessage(clientID, distributionInfo, model.getModelID()), MessageChannel.DATA);

            System.err.println("Send InitializeSimulationMessage.");

        } catch (NotConnectedException | ConnectionLostException e) {
            e.printStackTrace();
        }
    }

    public void startSimulation() {
        try {
            clientNetworkAdapter.sendNetworkMessage(new StartSimulationMessage(clientID, -1, -1), MessageChannel.DATA);
            System.err.println("Send StartSimulationMessage.");
        } catch (NotConnectedException | ConnectionLostException e) {
            e.printStackTrace();
        }
    }


    private class PushAllModelsMessageReceiver implements INetworkMessageReceivedEventHandler<PushAllModelsMessage> {
        private Consumer<PushAllModelsMessage> func;

        public PushAllModelsMessageReceiver(Consumer<PushAllModelsMessage> func) {
            this.func = func;
        }

        @Override
        public void onMessageReceived(PushAllModelsMessage message) {
            func.accept(message);
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    private class InitializationCompletedACKMessageReceiver implements INetworkMessageReceivedEventHandler<InitializationCompletedACKMessage> {

        private Consumer<Object> func;

        public InitializationCompletedACKMessageReceiver(Consumer<Object> func) {
            this.func = func;
        }

        @Override
        public void onMessageReceived(InitializationCompletedACKMessage message) {
            System.err.println("Received InitializationCompletedACKMessage");
            // call delegate method in starter.class
            func.accept(null);
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {
            //To change body of implemented methods use File | Settings | File Templates.
        }
    }


}
