package de.haw.run.simulationcore;

import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.Exceptions.ThisNodeNotSetException;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TLayer;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.NetworkAdapter.Implementation.ServerNetworkAdapterComponent;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.IServerNetworkAdapter;
import de.haw.run.noderegistry.Interface.INodeRegistry;
import de.haw.run.services.IGlobalClock;
import de.haw.run.services.globalclock.SAPDTimer;
import de.haw.run.simulationcore.modellcontainer.IModelContainer;
import de.haw.run.simulationcore.modellcontainer.ModelContainerComponent;
import de.haw.run.simulationcore.simulationmanager.SimulationManager;

import java.util.LinkedList;
import java.util.List;
import java.util.UUID;

public class SimulationCoreComponent {

    private final IModelContainer modelContainer;
    private SimulationManager simulationManager;
    private IServerNetworkAdapter serverNetworkAdapter;
    private IGlobalClock sapdTimer;
    private AppSettings appSettings;
    private INodeRegistry nodeRegistry;


    public SimulationCoreComponent(INodeRegistry nodeRegistry) {
        this.nodeRegistry = nodeRegistry;
        try {
            this.sapdTimer = new SAPDTimer(nodeRegistry.getThisNode().getIdentifier());
        } catch (ThisNodeNotSetException e) {
            e.printStackTrace();
        }
        this.appSettings = new AppSettings();
        this.modelContainer = new ModelContainerComponent();

        // MOCK Implementierung! TODO: Später ersetzen!
        this.modelContainer.addModel(getMOCKModel());


        try {

            serverNetworkAdapter = new ServerNetworkAdapterComponent(appSettings.getInt("SimulationCoreServerPort"));
            serverNetworkAdapter.startHosting();
            simulationManager = new SimulationManager(serverNetworkAdapter, nodeRegistry, sapdTimer, modelContainer);


        } catch (SettingException | TechnicalException | ConnectionLostException e) {
            e.printStackTrace();
        }
    }

    private TSimulationModel getMOCKModel(){

        List<TLayer> layers = new LinkedList<>();
        layers.add(new TLayer(UUID.randomUUID(), "forest", "ForestLayer.jar"));
        layers.add(new TLayer(UUID.randomUUID(), "weather", "WeatherLayer.jar"));
        layers.add(new TLayer(UUID.randomUUID(), "farmer", "FarmerLayer.jar"));



        return new TSimulationModel(
                UUID.randomUUID(),
                "AbdoulayeMockModel",
                layers
        );
    }

}