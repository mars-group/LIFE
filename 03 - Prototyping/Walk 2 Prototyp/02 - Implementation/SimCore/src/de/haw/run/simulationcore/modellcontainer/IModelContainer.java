package de.haw.run.simulationcore.modellcontainer;



import de.haw.run.GlobalTypes.Exceptions.BaseURLNotAvailableException;
import de.haw.run.GlobalTypes.TransportTypes.TLayer;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.layerAPI.TLayerInitializationDataType;

import java.io.File;
import java.net.URI;
import java.util.List;
import java.util.UUID;

public interface IModelContainer {

	public List<TSimulationModel> getAllModels();

	public TSimulationModel getModelByID(UUID modelId);

	public void addModel(TSimulationModel model);

	public void addModelFromFile(File modelFile);

	public TSimulationModel deleteModel(TSimulationModel model);

	public TSimulationModel deleteModel(UUID modelID);

    public URI getPluginURIForLayerID(TLayer layer) throws BaseURLNotAvailableException;

    public TLayerInitializationDataType getLayerInitData(UUID modelID, UUID layerID);
}