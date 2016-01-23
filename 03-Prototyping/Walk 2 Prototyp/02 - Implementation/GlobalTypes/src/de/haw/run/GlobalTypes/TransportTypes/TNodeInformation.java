package de.haw.run.GlobalTypes.TransportTypes;

import de.haw.run.GlobalTypes.NodeType;

import java.io.Serializable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 21:32
 */
public class TNodeInformation implements Serializable {

    private TConnectionInformation connectionInformation;
    private NodeType nodeType;
    private String identifier;

    public TNodeInformation(TConnectionInformation connectionInformation, NodeType nodeType, String identifier) {
        this.connectionInformation = connectionInformation;
        this.nodeType = nodeType;
        this.identifier = identifier;
    }

    public TConnectionInformation getConnectionInformation() {
        return connectionInformation;
    }

    public NodeType getNodeType() {
        return nodeType;
    }

    public String getIdentifier() {
        return identifier;
    }
}
