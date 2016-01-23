package de.haw.run.GlobalTypes.TransportTypes;


import java.io.Serializable;
import java.util.UUID;

public class TLayer implements Serializable {

    private UUID layerID;
    private String layerName;
    private String pluginFileName;

    public TLayer(UUID layerID, String layerName, String pluginFileName) {
        this.layerID = layerID;
        this.layerName = layerName;
        this.pluginFileName = pluginFileName;
    }

    public String getLayerName() {
        return layerName;
    }

    public String getPluginFileName() {
        return pluginFileName;
    }

    public UUID getLayerID() {
        return layerID;
    }
}
