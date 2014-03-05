package de.haw.run.noderegistry.Implementation;

import de.haw.run.GlobalTypes.Exceptions.ThisNodeNotSetException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TNodeInformation;
import de.haw.run.noderegistry.Interface.INodeRegistry;
import sun.reflect.generics.reflectiveObjects.NotImplementedException;

import java.util.*;

public class NodeRegistryComponent implements INodeRegistry {

    private Map<String, TNodeInformation> nodeMap;
    private String thisNodeIdentifier;

    public NodeRegistryComponent(){
        this.nodeMap = new Hashtable<>();
        this.thisNodeIdentifier = "";
    }


    @Override
    public void addNode(String identifier, TNodeInformation nodeInformation) {
        nodeMap.put(identifier, nodeInformation);
    }

    @Override
    public void addNodes(Map<String, TNodeInformation> map) {
        nodeMap.putAll(map);
    }

    @Override
    public Map<String, TConnectionInformation> getAllLayerContainerAddresses() {
        Map<String, TConnectionInformation> layerContainer = new Hashtable<>();
        nodeMap.forEach((string, node) -> {
            if(node.getNodeType().equals(NodeType.LAYERCONTAINER)){
                layerContainer.put(string, node.getConnectionInformation());
            }
        });
        return layerContainer;
    }

    @Override
    public TConnectionInformation getConnectionInformationByNodeName(String nodeName) {
        return nodeMap.get(nodeName).getConnectionInformation();
    }

    @Override
    public List<TConnectionInformation> getConnectionInformationByNodeType(NodeType type) {
        List<TConnectionInformation> result = new LinkedList<>();
        nodeMap.forEach((string, node) -> {
            if(node.getNodeType().equals(type)){
                result.add(node.getConnectionInformation());
            }
        });
        return result;
    }

    @Override
    public List<String> getIdentifiersByNodeType(NodeType type) {
        List<String> result = new LinkedList<>();
        nodeMap.forEach((string, node) -> {
            if(node.getNodeType().equals(type)){
                result.add(string);
            }
        });
        return result;
    }


    @Override
    public void startDiscovery(int sendInterval) {
        // TODO: Implement!
    }

    @Override
    public void registerThisNode(String identifier, TNodeInformation nodeInformation) {
        this.thisNodeIdentifier = identifier;
        addNode(identifier, nodeInformation);
    }

    @Override
    public TNodeInformation getThisNode() throws ThisNodeNotSetException {
        if(nodeMap.containsKey(this.thisNodeIdentifier)){
            return nodeMap.get(this.thisNodeIdentifier);
        } else {
            throw new ThisNodeNotSetException();
        }
    }


}