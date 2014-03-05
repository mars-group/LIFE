package de.haw.run.GlobalTypes.TransportTypes;

import de.haw.run.GlobalTypes.Exceptions.NodeNameNotRegisteredException;

import java.io.Serializable;
import java.util.LinkedList;
import java.util.List;
import java.util.Map;
import java.util.UUID;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 18:44
 */
public class TDistributionInformation implements Serializable {

    private Map<UUID, String> distributionMap;

    public TDistributionInformation(Map<UUID, String> distributionMap) {
        this.distributionMap = distributionMap;
    }

    public String getNodeNameByLayerID(UUID layerID) {
        return distributionMap.get(layerID);
    }

    public List<UUID> getLayersForNode(String nodeName) throws NodeNameNotRegisteredException {
        if(distributionMap.containsValue(nodeName)){
            List<UUID> result = new LinkedList<>();
            distributionMap.forEach((uuid, string) -> {
                if(string.equals(nodeName)){
                    result.add(uuid);
                }
            });
            return result;
        } else {
            throw new NodeNameNotRegisteredException();
        }
    }
}
