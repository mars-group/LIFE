package de.haw.run.simulationcore.modellcontainer;

import de.haw.run.GlobalTypes.Exceptions.BaseURLNotAvailableException;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.GlobalTypes.TransportTypes.TLayer;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;
import de.haw.run.simulationcore.modellcontainer.entities.LayerInitializationDataType;
import de.haw.run.simulationcore.modellcontainer.entities.SimulationModel;
import de.haw.run.layerAPI.TLayerInitializationDataType;

import java.io.File;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

public class ModelContainerComponent implements IModelContainer {

    private ModelRepository modelRepository;
    private ModelParser modelParser;



    public ModelContainerComponent() {
        this.modelParser = new ModelParser();
        this.modelRepository = new ModelRepository();
        AppSettings appSettings = new AppSettings();

        try {
            RunHTTPServer.createWebServer(appSettings.getInt("RunHTTPServerPort"), appSettings.getString("RunHTTPServerWebRoot"));
        } catch (SettingException e) {
            e.printStackTrace();
        }
    }

    @Override
    public List<TSimulationModel> getAllModels() {
        return modelRepository.getAllModels().parallelStream()
                .map(model -> new TSimulationModel(model.getModelID(), model.getName(), model.getLayers()))
                .collect(Collectors.toList());
    }

    @Override
    public TSimulationModel getModelByID(UUID modelId) {
        SimulationModel simModel = modelRepository.getModelByID(modelId);
        return new TSimulationModel(simModel.getModelID(), simModel.getName(), simModel.getLayers());
    }

    @Override
    public void addModel(TSimulationModel model) {
        modelRepository.addModel(new SimulationModel(model));
    }

    @Override
    public void addModelFromFile(File modelFile) {
        modelRepository.addModel(
                modelParser.parseModelFromFile(modelFile)
        );
    }

    @Override
    public TSimulationModel deleteModel(TSimulationModel model) {
        return deleteModel(model.getModelID());
    }

    @Override
    public TSimulationModel deleteModel(UUID modelID) {
        SimulationModel simModel = modelRepository.deleteModelByID(modelID);
        return new TSimulationModel(simModel.getModelID(), simModel.getName(), simModel.getLayers());
    }

    @Override
    public URI getPluginURIForLayerID(TLayer layer) throws BaseURLNotAvailableException {
        try {
            return new URI(RunHTTPServer.getBaseURL().toString() + layer.getPluginFileName());
        } catch (BaseURLNotAvailableException e) {
            throw(e);
        } catch (URISyntaxException uriEx) {
             throw new BaseURLNotAvailableException(uriEx);
        }
    }

    @Override
    public TLayerInitializationDataType getLayerInitData(UUID modelID, UUID layerID) {
        SimulationModel model = modelRepository.getModelByID(modelID);
        LayerInitializationDataType layerInitData = model.getLayerInitDataByLayerID(layerID);
        return new TLayerInitializationDataType();
    }
}