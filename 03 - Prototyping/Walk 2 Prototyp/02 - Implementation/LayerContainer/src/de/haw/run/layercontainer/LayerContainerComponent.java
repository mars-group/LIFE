package de.haw.run.layercontainer;

import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.NetworkAdapter.Implementation.ServerNetworkAdapterComponent;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.NetworkAdapter.Interface.IServerNetworkAdapter;
import de.haw.run.layercontainer.layercontainercontroller.LayerContainerControllerComponent;
import de.haw.run.noderegistry.Interface.INodeRegistry;

public class LayerContainerComponent {

    private AppSettings appSettings;

    private LayerContainerControllerComponent layerContainerControllerComponent;
    private IServerNetworkAdapter serverNetworkAdapter;
    private INodeRegistry nodeRegistry;

    public LayerContainerComponent(INodeRegistry nodeRegistry) throws SettingException, TechnicalException, ConnectionLostException {
        this.nodeRegistry = nodeRegistry;
        this.appSettings = new AppSettings();

        this.serverNetworkAdapter = new ServerNetworkAdapterComponent(appSettings.getInt("LayerContainerDataListeningPort"));
        this.serverNetworkAdapter.startHosting();
        this.layerContainerControllerComponent = new LayerContainerControllerComponent(serverNetworkAdapter, nodeRegistry);

    }
}