package de.haw.run.simulationcore.simulationmanager;

import de.haw.run.GlobalTypes.Exceptions.BaseURLNotAvailableException;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.*;
import de.haw.run.NetworkAdapter.Implementation.ClientNetworkAdapterComponent;
import de.haw.run.NetworkAdapter.Interface.*;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.HostUnreachableException;
import de.haw.run.NetworkAdapter.Interface.Exceptions.NotConnectedException;
import de.haw.run.NetworkAdapter.Messages.execution.*;
import de.haw.run.NetworkAdapter.Messages.initialization.*;
import de.haw.run.noderegistry.Interface.INodeRegistry;
import de.haw.run.services.IGlobalClock;
import de.haw.run.simulationcore.modellcontainer.IModelContainer;

import java.util.*;
import java.util.concurrent.Executors;
import java.util.concurrent.ScheduledExecutorService;
import java.util.concurrent.TimeUnit;
import java.util.stream.Collectors;

/**
 *  The SimulationManager implements the initialization and centralized simulation excecution
 *  protocols by providing listeners and appropriate actions to the incoming messages.
 */
public class SimulationManager {

    // ##### global stuff

    private AppSettings appSettings;
    private final IModelContainer modelContainer;
    private IServerNetworkAdapter serverNetworkAdapter;
    private INodeRegistry nodeRegistry;
    private IGlobalClock sapdTimer;
    private final UUID clientname;
    private List<TLayer> layersOrderedForInitialization;
    private Map<String, IClientNetworkAdapter> containerToNetworkAdapterMap;
    private TDistributionInformation distributionInformation;
    private TSimulationModel chosenModel;
    private Map<UUID, String> layerToContainerMap;

    // ##### Init stuff

    private List<UUID> addedLayersWaitingForAcknowledgement;
    private List<UUID> initializedLayersWaitingForAcknowledgement;

    // ##### Running stuff

    private static long MIN_TICK_DIFFERENCE;
    private long START_TIME_DELAY;
    private int MAX_TICK_DURATION_NOT_CHANGED_THRESHOLD;
    private long currentMaxTickDuration;
    private int maxTickDurationNotChangedSince;
    private List<Integer> advancedLayerContainerAwaitingAcknowledgement;
    private List<Long> tickDurationList;


    public SimulationManager(IServerNetworkAdapter serverNetworkAdapter, INodeRegistry nodeRegistry, IGlobalClock sapdTimer, IModelContainer modelContainer) {
        appSettings = new AppSettings();
        try {
            MIN_TICK_DIFFERENCE = Long.parseLong(appSettings.getString("MinTickDifference"));
            MAX_TICK_DURATION_NOT_CHANGED_THRESHOLD = appSettings.getInt("MaxTickDurationNotChangedThreshold");
            START_TIME_DELAY = Long.parseLong(appSettings.getString("StartTimeDelay"));
        } catch (SettingException e) {
            e.printStackTrace();
        }

        this.serverNetworkAdapter = serverNetworkAdapter;
        this.nodeRegistry = nodeRegistry;
        this.sapdTimer = sapdTimer;
        this.modelContainer = modelContainer;
        this.containerToNetworkAdapterMap = new Hashtable<>();
        this.layerToContainerMap = new Hashtable<>();
        this.addedLayersWaitingForAcknowledgement = new LinkedList<>();
        this.initializedLayersWaitingForAcknowledgement = new LinkedList<>();
        this.advancedLayerContainerAwaitingAcknowledgement = new LinkedList<>();
        this.tickDurationList = new LinkedList<>();
        this.currentMaxTickDuration = -1;
        this.maxTickDurationNotChangedSince = 0;

        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new GetAllModelsMessageReceiver(), GetAllModelsMessage.class);
        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new InitializeSimulationMessageReceiver(), InitializeSimulationMessage.class);
        this.serverNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new StartSimulationMessageReceiver(), StartSimulationMessage.class);
        this.clientname = UUID.randomUUID();
    }


    /*
    ################ Initialization Process ##################
     */

    /**
     * Receives and handles GetAllModelsMessages from SimulationControllers. Will send back a list of models to the sender.
     */
    private class GetAllModelsMessageReceiver implements INetworkMessageReceivedEventHandler<GetAllModelsMessage>{

        @Override
        public void onMessageReceived(GetAllModelsMessage message) {
            try {
                serverNetworkAdapter.sendNetworkMessage(
                        new PushAllModelsMessage(
                                message.getClientId(),
                                modelContainer.getAllModels()
                        ),
                        MessageChannel.DATA
                );
            } catch (NotConnectedException | ConnectionLostException e) {
                e.printStackTrace();
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Receives and handles InitializeSimulationMessages from SimulationControllers. Will calculate LoadOrder,
     * add Layers from the chosenModel in that order to the appropriate layers according to the provided
     * distribution information.
     */
    private class InitializeSimulationMessageReceiver implements INetworkMessageReceivedEventHandler<InitializeSimulationMessage>{

        @Override
        public void onMessageReceived(InitializeSimulationMessage message) {

            System.err.println("Received InitializeSimulationMessage.");

            // retrieve SimulationModel for ModelID
            chosenModel = modelContainer.getModelByID(message.getSimModelID());
            // calculate LoadOrder for SimulationModel
            layersOrderedForInitialization = LoadOrderCalculator.calculateLoadOrder(chosenModel);
            // save distribution information, that is, which layer should be executed on which layer container
            distributionInformation = message.getDistributionInformation();

            // add layerIDs to the list of added Layers waiting for acknowledgment
            addedLayersWaitingForAcknowledgement.addAll(layersOrderedForInitialization.parallelStream().map(l -> l.getLayerID()).collect(Collectors.toList()));

            // add Layers to LayerContainers
            layersOrderedForInitialization.forEach(

                    (layer) -> {

                        IClientNetworkAdapter clientNetworkAdapter = null;

                        String layerContainerName = distributionInformation.getNodeNameByLayerID(layer.getLayerID());

                        // check if a connection to that LayerContainer already exists
                        if(containerToNetworkAdapterMap.containsKey(layerContainerName)){
                            clientNetworkAdapter = containerToNetworkAdapterMap.get(layerContainerName);
                        } else {
                            clientNetworkAdapter = new ClientNetworkAdapterComponent();

                            // get connection information for the layers chosen node
                            TConnectionInformation connectionInformation = nodeRegistry.getConnectionInformationByNodeName(layerContainerName);


                            try {
                                clientNetworkAdapter.connectToServer(connectionInformation.getIpAddress(), connectionInformation.getPort(), clientname.toString());
                            } catch (HostUnreachableException | TechnicalException e) {
                                e.printStackTrace();
                            }

                            // subscribe for ACK Messages
                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new AddLayerACKMessageReceiver(), AddLayerACKMessage.class);
                            // subscribe for NACK Messages
                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new AddLayerNACKMessageReceiver(), AddLayerNACKMessage.class);


                            // subscribe for ACK Messages
                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new InitializeLayerFromModelACKMessageReceiver(), InitializeLayerFromModelACKMessage.class);
                            // subscribe for NACK Messages
                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new InitializeLayerFromModelNACKMessageReceiver(), InitializeLayerFromModelNACKMessage.class);

                            // subscribe for TickFinishedMessages
                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new TickFinishedMessageReceiver(), TickFinishedMessage.class);


                            clientNetworkAdapter.subscribeForNetworkMessageReceivedEvent(new StartSimulationACKMessageReceiver(), StartSimulationACKMessage.class);
                        }


                        // try to send AddLayerMessage to the layers LayerContainer
                        try {
                            clientNetworkAdapter.sendNetworkMessage(
                                    new AddLayerMessage(
                                            clientNetworkAdapter.getClientId(),
                                            modelContainer.getPluginURIForLayerID(layer),
                                            layer.getLayerID()
                                    ),
                                    MessageChannel.DATA
                            );

                            System.err.println("Send AddLayerMessage.");

                            // Success:
                            // save layer - layerContainer mapping
                            layerToContainerMap.put(layer.getLayerID(), layerContainerName);
                            // save layerContainer - NetworkAdapter mapping
                            containerToNetworkAdapterMap.put(layerContainerName, clientNetworkAdapter);


                        } catch (NotConnectedException | ConnectionLostException | BaseURLNotAvailableException e) {
                            e.printStackTrace();
                        }
                    }

            );

        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {
        }
    }

    /**
     * Receives and handles AddLayerACKMessages from LayerContainers. when all ACK Messages are received, layer initialization will be started.
     */
    private class AddLayerACKMessageReceiver implements INetworkMessageReceivedEventHandler<AddLayerACKMessage> {
        @Override
        public void onMessageReceived(AddLayerACKMessage message) {
            System.err.println("Received AddLayerACKMessage with layerID: " + message.getLayerID());
            // remove layerID from List
            addedLayersWaitingForAcknowledgement.remove(message.getLayerID());
            // check if everything is done
            if(addedLayersWaitingForAcknowledgement.isEmpty()){
                // all Layers were added, so initialize them
                initializeAllLayersFromModel();
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }
    private class AddLayerNACKMessageReceiver implements INetworkMessageReceivedEventHandler<AddLayerNACKMessage> {
        @Override
        public void onMessageReceived(AddLayerNACKMessage message) {
            System.err.println(message.getReason());
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {
            //To change body of implemented methods use File | Settings | File Templates.
        }
    }

    /**
     * Initializes all layers with LayerInitializationData from the chosenModel.
     */
    private void initializeAllLayersFromModel() {
        if(layersOrderedForInitialization != null
                && !layersOrderedForInitialization.isEmpty()
                && chosenModel != null){

            // add layerIDs to the list of initialized Layers
            initializedLayersWaitingForAcknowledgement.addAll(layersOrderedForInitialization.parallelStream().map(l -> l.getLayerID()).collect(Collectors.toList()));

            // proceed in order to guarantee, that dependent layers are initialized when depending layers want to access them.
            layersOrderedForInitialization.forEach(

                    (layer) -> {

                        IClientNetworkAdapter clientNetworkAdapter = containerToNetworkAdapterMap.get(distributionInformation.getNodeNameByLayerID(layer.getLayerID()));




                        // try to send AddLayerMessage to the layers LayerContainer
                        try {
                            clientNetworkAdapter.sendNetworkMessage(
                                    new InitializeLayerFromModelMessage(
                                            clientNetworkAdapter.getClientId(),
                                            layer.getLayerID(),
                                            modelContainer.getLayerInitData(chosenModel.getModelID(), layer.getLayerID())
                                    ),
                                    MessageChannel.DATA
                            );

                            System.err.println("Send InitializeLayerFromModelMessage.");

                        } catch (NotConnectedException | ConnectionLostException e) {
                            e.printStackTrace();
                        }
                    }

            );

        }
    }

    /**
     * Receives and handles InitializeLayerFromModelACKMessages from LayerContainers. When all ACK messages are received,
     * the GlobalClock Sync mechanism is started and all SimulationControllers are informed that initialization is done.
     */
    private class InitializeLayerFromModelACKMessageReceiver implements INetworkMessageReceivedEventHandler<InitializeLayerFromModelACKMessage> {

        @Override
        public void onMessageReceived(InitializeLayerFromModelACKMessage message) {
            System.err.println("Received InitializeLayerFromModelACKMessage.");

            // remove layerID from List
            initializedLayersWaitingForAcknowledgement.remove(message.getLayerID());

            // check if everything is done
            if(initializedLayersWaitingForAcknowledgement.isEmpty()){

                // initialize SAPDClock Synchro...
                initializeTimerSynchronisation();

                // inform all SimulationControllers that Initialization is done
                nodeRegistry.getIdentifiersByNodeType(NodeType.SIMULATIONCONTROLLER)
                        .parallelStream()
                        .forEach( identifier -> {
                            try {
                                serverNetworkAdapter.sendNetworkMessage(
                                        new InitializationCompletedACKMessage(
                                                serverNetworkAdapter.getConnectedClients()
                                                        .parallelStream()
                                                        .filter(
                                                                client -> client.getName().equals(identifier)
                                                        )
                                                        .findFirst()
                                                        .get()
                                                        .getId()
                                        ),
                                        MessageChannel.DATA
                                );

                                System.err.println("Send InitializationCompletedACKMessage.");
                            } catch (NotConnectedException | ConnectionLostException e) {
                                e.printStackTrace();
                            }
                        }
                        );
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    private class InitializeLayerFromModelNACKMessageReceiver implements INetworkMessageReceivedEventHandler<InitializeLayerFromModelNACKMessage> {
        @Override
        public void onMessageReceived(InitializeLayerFromModelNACKMessage message) {
            System.err.println("ERROR: Received InitializeLayerFromModelACKMessage");
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {
            //To change body of implemented methods use File | Settings | File Templates.
        }
    }

    /**
     * Starts the SAPDTimer as Sponsor to kick synchronization.
     */
    private void initializeTimerSynchronisation() {
        ScheduledExecutorService ses = Executors.newScheduledThreadPool(1);
        try {
            ses.schedule(() -> sapdTimer.startTimer(true), new AppSettings().getInt("SAPDTimerSponsorStartDelay"), TimeUnit.MILLISECONDS);
        } catch (SettingException e) {
            e.printStackTrace();
        }
    }


    /*
    ################ Execution Process ##################
    */

    /**
     * Receives and handles StartSimulationMessages from SimulationControllers. Will start CSE process.
     */
    private class StartSimulationMessageReceiver implements INetworkMessageReceivedEventHandler<StartSimulationMessage> {
        @Override
        public void onMessageReceived(StartSimulationMessage message) {

            System.err.println("Received StartSimulationMessage");

            if(everythingIsReady()){
                advanceAllLayersOneTick();
            } else {
                try {
                    serverNetworkAdapter.sendNetworkMessage(
                            new StartSimulationNACKMessage(
                                    message.getClientId(),
                                    "Some Layer is not ready, or a connection was lost. Please check the status of all Containers."),
                            MessageChannel.DATA);
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
     * Tells every LayerContainer to advance its layers by one tick.
     */
    private void advanceAllLayersOneTick() {
        // Success: add container to waiting list.
        advancedLayerContainerAwaitingAcknowledgement.addAll(containerToNetworkAdapterMap.values().parallelStream().map(c -> {
            try {
                return c.getClientId();
            } catch (NotConnectedException e) {
                e.printStackTrace();
                return -1;
            }
        }).collect(Collectors.toList()));

        // send every LayerContainer an AdvanceOneTickMessage
        containerToNetworkAdapterMap
                .forEach((containerName, clientAdapter) ->
                        {
                            try {
                                clientAdapter.sendNetworkMessage(new AdvanceOneTickMessage(clientAdapter.getClientId()), MessageChannel.DATA);
                            } catch (ConnectionLostException | NotConnectedException e) {
                                e.printStackTrace();
                            }
                        }
                );
    }

    /**
     * Check if everything is ready for simulation execution
     * @return
     */
    private boolean everythingIsReady() {
        return initializedLayersWaitingForAcknowledgement.isEmpty()
                && addedLayersWaitingForAcknowledgement.isEmpty()
                && chosenModel != null
                && !layerToContainerMap.isEmpty()
                && !containerToNetworkAdapterMap.isEmpty();
    }

    /**
     * Receives and handles TickFinishedMessage from LayerContainers. When all LayerContainers reported their tick to
     * be finished, the maximal tick duration is computed and compared to the current currentMaxTickDuration. If the new maximal
     * duration is longer than the currentMaxTickDuration minus a predefined value, currentMaxTickDuration is updated.
     * If currentMaxTickDuration did not change within certain bounds for MAX_TICK_DURATION_NOT_CHANGED_THRESHOLD ticks, then
     * execution mode is changed to DCSE.
     */
    private class TickFinishedMessageReceiver implements INetworkMessageReceivedEventHandler<TickFinishedMessage> {
        @Override
        public void onMessageReceived(TickFinishedMessage message) {
            advancedLayerContainerAwaitingAcknowledgement.remove(message.getClientId());

            tickDurationList.add(message.getTickDuration());

            if(advancedLayerContainerAwaitingAcknowledgement.isEmpty()){

                // get maximal duration from received messages
                long newMaxDuration = tickDurationList.parallelStream()
                        .max(Long::compareTo).get();

                // change to DCSE if threshold is reached
                if(maxTickDurationNotChangedSince >= MAX_TICK_DURATION_NOT_CHANGED_THRESHOLD){
                    // System seems to be calibrated, try de-centralized execution
                    changeExecutionModeToDCSE();
                    return;

                } else if(newMaxDuration >= currentMaxTickDuration){
                    // update currentMaxTickDuration if the max value of received durations is at least 10ms longer
                    currentMaxTickDuration = newMaxDuration + MIN_TICK_DIFFERENCE;
                    maxTickDurationNotChangedSince = 0;
                } else {
                    maxTickDurationNotChangedSince++;
                }

                // mode not changed. keep maxDuration and advance by another tick
                tickDurationList.clear();
                //tickDurationList.add(currentMaxTickDuration);
                advanceAllLayersOneTick();
            }
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {

        }
    }

    /**
     * Delegates layer advancement to the LayerContainers, so called DCSE.
     */
    private void changeExecutionModeToDCSE() {
        containerToNetworkAdapterMap.values().parallelStream().forEach(
                client -> {

                    try {
                        long time = sapdTimer.getTime();
                        client.sendNetworkMessage(
                            new StartSimulationMessage(
                                    client.getClientId(),
                                    time + START_TIME_DELAY,
                                    currentMaxTickDuration),
                            MessageChannel.DATA
                        );

                        System.err.println("Send StartSimulationMessages with Time: "+ time );

                    } catch (NotConnectedException | ConnectionLostException e) {
                        e.printStackTrace();
                    }
                }
        );
    }

    /**
     * Receives and handles StartSimulationACKMessage from LayerContainers. Will... well... do nothing atm :P
     */
    private class StartSimulationACKMessageReceiver implements INetworkMessageReceivedEventHandler<StartSimulationACKMessage> {
        @Override
        public void onMessageReceived(StartSimulationACKMessage message) {
            // dunno...
        }

        @Override
        public void onNetworkEvent(NetworkEventType networkEventType, int clientID) {
        }
    }



}