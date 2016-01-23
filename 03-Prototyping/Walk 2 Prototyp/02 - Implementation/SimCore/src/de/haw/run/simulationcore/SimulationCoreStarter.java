package de.haw.run.simulationcore;

import de.haw.run.GlobalTypes.Exceptions.NodeNameNotSetException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TNodeInformation;
import de.haw.run.noderegistry.Implementation.NodeRegistryComponent;
import de.haw.run.noderegistry.Interface.INodeRegistry;

public class SimulationCoreStarter {

    private static String thisNodeName;

    public static void main(String[] args) throws NodeNameNotSetException {
        if(args.length <= 0){
            throw new NodeNameNotSetException();
        } else {
            thisNodeName = args[0];
        }

        INodeRegistry nodeRegistry = new NodeRegistryComponent();
        AppSettings appSettings = new AppSettings();
        try {
            nodeRegistry.startDiscovery(appSettings.getInt("DiscoveryInterval"));

            // instead of really discovering , just read IP and Port from File

            nodeRegistry.registerThisNode(thisNodeName,
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("SimulationCoreListenAddress"),
                                    appSettings.getInt("SimulationCoreServerPort")
                            ),
                            NodeType.SIMULATIONCORE,
                            thisNodeName
                    )
            );

            nodeRegistry.addNode("LayerContainerA",
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("LayerContainerListenAddress"),
                                    appSettings.getInt("LayerContainerDataListeningPort")
                            ),
                            NodeType.LAYERCONTAINER,
                            "LayerContainerA"
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

            SimulationCoreComponent layerContainerComponent = new SimulationCoreComponent(nodeRegistry);

            while(true){
                // keep running....
            }
        } catch (SettingException e) {
            e.printStackTrace();
        }
	}


}