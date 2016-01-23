import de.haw.run.GlobalTypes.Exceptions.NodeNameNotSetException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TNodeInformation;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.NetworkAdapter.Messages.initialization.PushAllModelsMessage;
import de.haw.run.noderegistry.Implementation.NodeRegistryComponent;
import de.haw.run.noderegistry.Interface.INodeRegistry;

import javax.xml.ws.Holder;
import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.util.LinkedList;
import java.util.List;


public class SimControllerStarter {

    private static BufferedReader br;
    private static INodeRegistry nodeRegistry;
    private static AppSettings appSettings;
    private static LinkedList<TSimulationModel> simModels;
    private static SimulationControllerComponent simulationControllerComponent;
    private static String thisNodeName;

    public static void main(String[] args) throws IOException, NodeNameNotSetException {

        if(args.length <= 0){
            throw new NodeNameNotSetException();
        } else {
            thisNodeName = args[0];
        }


        br = new BufferedReader(new InputStreamReader(System.in));

        nodeRegistry = new NodeRegistryComponent();

        appSettings = new AppSettings();

        simModels = new LinkedList<>();

        try {
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

            nodeRegistry.registerThisNode(thisNodeName,
                    new TNodeInformation(
                            new TConnectionInformation(
                                    appSettings.getString("SimulationControllerListenAddress"),
                                    appSettings.getInt("SimulationControllerServerPort")
                            ),
                            NodeType.SIMULATIONCONTROLLER,
                            thisNodeName
                    )
            );

        } catch (SettingException e) {
            e.printStackTrace();
        }

        simulationControllerComponent = new SimulationControllerComponent(nodeRegistry);

        System.out.println("Retrieving SimulationModels...");

        simulationControllerComponent.getSimulationModels(SimControllerStarter::initializeSimulation);

        while(true){
           //keep running.
        }

    }

    private static void initializeSimulation(PushAllModelsMessage msg) {
        simModels.addAll(msg.getAllModels());
        System.out.println(simModels.get(0).getName() + " found!");

        System.out.println("...done.");

/*        try {
            br.readLine();
        } catch (IOException e) {
            e.printStackTrace();
        }*/


        System.out.println("Starting initialization with Model: " + simModels.get(0).getName());

        simulationControllerComponent.initializeSimulation(simModels.get(0), SimControllerStarter::startSimulation);
    }

    private static void startSimulation(Object bla){
        System.out.println("...done.");
/*

        try {
            br.readLine();
        } catch (IOException e) {
            e.printStackTrace();
        }
*/

        simulationControllerComponent.startSimulation();
    }

}
