package de.haw.run.GlobalTypes.TransportTypes;

import java.io.Serializable;
import java.util.List;
import java.util.UUID;


public class TSimulationModel implements Serializable {
    private UUID modelID;
    private String name;
    private List<TLayer> layers;

    public TSimulationModel(UUID modelID, String name, List<TLayer> layers){
        this.modelID = modelID;
        this.name = name;
        this.layers = layers;
    }

    public UUID getModelID() {
        return modelID;
    }


    public String getName() {
        return name;
    }

    public List<TLayer> getLayers() {
        return layers;
    }
}
