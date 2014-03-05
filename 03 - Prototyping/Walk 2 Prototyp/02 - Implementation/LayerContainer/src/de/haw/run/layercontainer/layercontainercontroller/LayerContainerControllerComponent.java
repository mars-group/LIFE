package de.haw.run.layercontainer.layercontainercontroller;

import de.haw.run.GlobalTypes.Exceptions.LayerExecutionError;
import de.haw.run.GlobalTypes.Exceptions.LayerNotAddableException;
import de.haw.run.GlobalTypes.Exceptions.ThisNodeNotSetException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Interface.INetworkMessageReceivedEventHandler;
import de.haw.run.NetworkAdapter.Interface.IServerNetworkAdapter;
import de.haw.run.NetworkAdapter.Interface.MessageChannel;
import de.haw.run.NetworkAdapter.Interface.NetworkEventType;
import de.haw.run.NetworkAdapter.Messages.*;
import de.haw.run.NetworkAdapter.Messages.execution.*;
import de.haw.run.NetworkAdapter.Messages.initialization.*;
import de.haw.run.layercontainer.ISwitchExecutionMode;
import de.haw.run.layercontainer.layercontainercontroller.layermanagement.LayerController;
import de.haw.run.layercontainer.layercontainercontroller.layermanagement.LayerRepository;
import de.haw.run.noderegistry.Interface.INodeRegistry;
import de.haw.run.services.IGlobalClock;
import de.haw.run.services.globalclock.SAPDTimer;
import sun.reflect.generics.reflectiveObjects.NotImplementedException;

import java.util.UUID;


public class LayerContainerControllerComponent {

	private LayerRepository layerRepository;
    private LayerController layerController;
    private IServerNetworkAdapter serverNetworkAdapter;
    private INodeRegistry nodeRegistry;
    private IGlobalClock globalClock;

    /**
     * Creates a LayerContainerController, which will listen to all incoming messages from the SimulationCore.
     * According to those messages this Controller will then initialize, start and advance the layers living in this LayerContainer
     * @param serverNetworkAdapter the NetworkAdapter. Must be already started!
     * @param nodeRegistry the NodeRegistry
     */
    public LayerContainerControllerComponent(IServerNetworkAdapter serverNetworkAdapter, INodeRegistry nodeRegistry) {
        this.serverNetworkAdapter = serverNetworkAdapter;
        this.nodeRegistry = nodeRegistry;
        this.layerRepository = new LayerRepository();

        try {
            this.globalClock = new SAPDTimer(nodeRegistry.getThisNode().getIdentifier());
        } catch (ThisNodeNotSetException e) {
            e.printStackTrace();
        }

        this.layerController = new LayerController(layerRepository, globalClock, new ExecutionModeSwitcher());

        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new AddLayerMessageReceiver(), AddLayerMessage.class);
        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new InitializeLayerFromModelReceiver(), InitializeLayerFromModelMessage.class);
        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new AdvanceOneTickMessageReceiver(), AdvanceOneTickMessage.class);
        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new StartSimulationMessageReceiver(), StartSimulationMessage.class);
    }




    /**
     * Receives and handles AddLayerMessages from the SimulationCore.
     * Will send an ACK Message containing the LayerID back to SimCore if successful, and an NACK Message if not.
     */
    private class AddLayerMessageReceiver implements INetworkMessageReceivedEventHandler<AddLayerMessage>{

        @Override
        public void onMessageReceived(AddLayerMessage message) {

                System.err.println("Received AddLayerMessage");

                try {
                    UUID layerID = layerRepository.addLayer(message.getLayerURI(), message.getLayerID());
                    serverNetworkAdapter.sendNetworkMessage(new AddLayerACKMessage(message.getClientId(), layerID), MessageChannel.DATA);
                    System.err.println("Send AddLayerACKMessage");

                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                } catch (LayerNotAddableException e) {
                    try {
                    serverNetworkAdapter.sendNetworkMessage(new AddLayerNACKMessage(message.getClientId(), "ERROR: Provided LayerURI was wrong!"), MessageChannel.DATA);
                    } catch (NotConnectedException | ConnectionLostException notConnectedException) {
                        notConnectedException.printStackTrace();
                    }
                }
        }


        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Receives and handles Initialization Messages from the SimulationCore for certain Layers
     */
    private class InitializeLayerFromModelReceiver implements INetworkMessageReceivedEventHandler<InitializeLayerFromModelMessage>{

        @Override
        public void onMessageReceived(InitializeLayerFromModelMessage message) {

            System.err.println("Received InitializeLayerFromModelMessage");

            if(layerController.initializeLayerFromModel(message.getLayerID(), message.getLayerInitData())){
                try {
                    globalClock.startTimer();
                    serverNetworkAdapter.sendNetworkMessage(new InitializeLayerFromModelACKMessage(message.getClientId(), message.getLayerID()), MessageChannel.DATA);
                    System.err.println("Send InitializeLayerFromModelACKMessage");
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }
            } else {
                try {
                    serverNetworkAdapter.sendNetworkMessage(new InitializeLayerFromModelNACKMessage(message.getClientId(), message.getLayerID()), MessageChannel.DATA);
                    System.err.println("ERROR: Send InitializeLayerFromModelNACKMessage");
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Receives and handles AdvanceOneTickMessages. Tells the LayerController to advance all Layers one Tick,
     * then sends the longest duration back to SimulationCore.
     */
    private class AdvanceOneTickMessageReceiver implements INetworkMessageReceivedEventHandler<AdvanceOneTickMessage>{

        @Override
        public void onMessageReceived(AdvanceOneTickMessage msg) {

            try {
                long duration = layerController.advanceAllLayersOneTick();

                try {
                    serverNetworkAdapter.sendNetworkMessage(new TickFinishedMessage(msg.getClientId(), duration), MessageChannel.DATA);
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }

            } catch (LayerExecutionError layerExecutionError) {

                layerExecutionError.printStackTrace();
                // a layer threw an exception, stop execution
                try {
                    serverNetworkAdapter.sendNetworkMessage(new AbortSimulationMessage(msg.getClientId(), "A Layer threw an Error, aborting..."), MessageChannel.DATA);
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }

            }

        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Receives and handles StartSimulationMessages. Tells the LayerController to start all Layers at
     * given startTime and with given Timeinterval
     */
    private class StartSimulationMessageReceiver implements INetworkMessageReceivedEventHandler<StartSimulationMessage>{

        @Override
        public void onMessageReceived(StartSimulationMessage msg) {

            System.err.println("Received StartSimulationMessage.");

            if(layerController.startAllLayers(msg.getStartTime(), msg.getTickLength())){
                try {
                    serverNetworkAdapter.sendNetworkMessage(new StartSimulationACKMessage(msg.getClientId()), MessageChannel.DATA);
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }
            } else {
                try {
                    serverNetworkAdapter.sendNetworkMessage(new StartSimulationNACKMessage(msg.getClientId(), "Simulation couldn't get started, please check LayerContainer status."), MessageChannel.DATA);
                } catch (NotConnectedException | ConnectionLostException e) {
                    e.printStackTrace();
                }
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Gets called from LayerController if a layer took longer to execute than ExecutionInterval was set to
     * Will then stop execution and delegate ExecutionControl back to SimulationCore.
     */
    private class ExecutionModeSwitcher implements ISwitchExecutionMode {

        @Override
        public void switchMode(long tick, long longestDuration) {
            // TODO: Implement mode switch.
            throw new NotImplementedException();
        }
    }

}