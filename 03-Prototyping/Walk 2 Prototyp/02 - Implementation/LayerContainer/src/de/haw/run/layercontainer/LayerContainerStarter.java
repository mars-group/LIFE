package de.haw.run.layercontainer;

import de.haw.run.GlobalTypes.Exceptions.NodeNameNotSetException;
import de.haw.run.GlobalTypes.Exceptions.TechnicalException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TNodeInformation;
import de.haw.run.NetworkAdapter.Interface.Exceptions.ConnectionLostException;
import de.haw.run.noderegistry.Implementation.NodeRegistryComponent;
import de.haw.run.noderegistry.Interface.INodeRegistry;

public class LayerContainerStarter {

    private static String thisNodeName;

    public static void main(String[] args) throws NodeNameNotSetException {
        if(args.length <= 0){
            throw new NodeNameNotSetException();
        } else {
            thisNodeName = args[0];
        }


        INodeRegistry nodeRegistry = new NodeRegistryComponent();
        try {
            AppSettings appSettings = new AppSettings();
            nodeRegistry.startDiscovery(appSettings.getInt("DiscoveryInterval"));

            // instead of really discovering , just red IP and Port from File
            // TODO: Implement Mcast Discovery
            nodeRegistry.addNode("SimCore",
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("SimulationCoreListenAddress"),
                                    appSettings.getInt("SimulationCoreServerPort")
                            ),
                            NodeType.SIMULATIONCORE,
                            "SimCore"
                    )
            );

            nodeRegistry.registerThisNode(thisNodeName,
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("LayerContainerListenAddress"),
                                    appSettings.getInt("LayerContainerDataListeningPort")
                            ),
                            NodeType.LAYERCONTAINER,
                            thisNodeName
                    )
            );

            nodeRegistry.addNode("SimController",
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("SimulationControllerListenAddress"),
                                    appSettings.getInt("SimulationControllerServerPort")
                            ),
                            NodeType.SIMULATIONCONTROLLER,
                            "SimController"
                    )
            );

            LayerContainerComponent layerContainerComponent = new LayerContainerComponent(nodeRegistry);

            while(true){
                // keep running....
            }
        } catch (SettingException | ConnectionLostException | TechnicalException e) {
            e.printStackTrace();
        }
    }
}