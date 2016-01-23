package de.haw.run.noderegistry.Interface;

import de.haw.run.GlobalTypes.Exceptions.ThisNodeNotSetException;
import de.haw.run.GlobalTypes.NodeType;
import de.haw.run.GlobalTypes.TransportTypes.TConnectionInformation;
import de.haw.run.GlobalTypes.TransportTypes.TNodeInformation;

import java.util.List;
import java.util.Map;

public interface INodeRegistry {

	public void addNode(String identifier, TNodeInformation nodeInformation);

	public void addNodes(Map<String, TNodeInformation> map);

	public Map<String, TConnectionInformation> getAllLayerContainerAddresses();

	public void startDiscovery(int sendInterval);

    public void registerThisNode(String identifier, TNodeInformation nodeInformation);

    /**
     * Returns TNodeInformation about this node.
     * @return
     */
    public TNodeInformation getThisNode() throws ThisNodeNotSetException;

    public TConnectionInformation getConnectionInformationByNodeName(String nodeName);

    /**
     * Returns a list of TConnectionInformation for each node matching type
     * @param type
     * @return A list of TConnectionInformation
     */
    public List<TConnectionInformation> getConnectionInformationByNodeType(NodeType type);

    public List<String> getIdentifiersByNodeType(NodeType type);
}