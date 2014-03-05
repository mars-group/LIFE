package de.haw.run.simulationcore.modellcontainer;

import de.haw.run.simulationcore.modellcontainer.entities.SimulationModel;

import java.util.*;
import java.util.stream.Collectors;

public class ModelRepository {

    private Map<UUID, SimulationModel> models;

    public ModelRepository(){
        this.models = new Hashtable<>();
    }

	public List<SimulationModel> getAllModels() {
		return new LinkedList<>(models.values());
	}

    public UUID addModel(SimulationModel model){
        UUID id = UUID.randomUUID();
        model.setModelID(id);
        models.put(id, model);
        return id;
    }

    public SimulationModel getModelByID(UUID modelID){
        return models.get(modelID);
    }

    public List<SimulationModel> getModelsByName(String name){
        return models.values().parallelStream().filter(
                model -> model.getName().equals(name)
        ).collect(Collectors.toList());
    }

    public SimulationModel deleteModelByID(UUID modelID) {
        return models.remove(modelID);
    }
}