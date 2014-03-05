package de.haw.run.simulationcore.modellcontainer.entities;


import de.haw.run.GlobalTypes.TransportTypes.TLayer;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import sun.reflect.generics.reflectiveObjects.NotImplementedException;

import java.util.List;
import java.util.UUID;

public class SimulationModel {

    private UUID modelID;
    private String name;
    private List<TLayer> layers;

    public SimulationModel(TSimulationModel model) {
        this.name = model.getName();
        this.layers = model.getLayers();
    }

    public void setModelID(UUID modelID) {
        this.modelID = modelID;
    }

    public UUID getModelID() {
        return modelID;
    }

    public String getName() {
        return name;
    }

    public LayerInitializationDataType getLayerInitDataByLayerID(UUID layerID) {
        return new LayerInitializationDataType();
    }

    public List<TLayer> getLayers() {
        return layers;
    }

}
