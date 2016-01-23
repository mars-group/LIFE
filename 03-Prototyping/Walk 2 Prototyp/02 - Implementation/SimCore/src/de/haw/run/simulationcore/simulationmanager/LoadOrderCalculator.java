package de.haw.run.simulationcore.simulationmanager;

import de.haw.run.GlobalTypes.TransportTypes.TLayer;
import de.haw.run.GlobalTypes.TransportTypes.TSimulationModel;

import java.util.LinkedList;
import java.util.List;
import java.util.UUID;
import java.util.stream.Collectors;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 18:56
 */
public class LoadOrderCalculator {

    /**
     * Returns the load order based on the dependencies between the layers in the provided model.
     * @param model
     * @return An ordered list with layerIDs of the layers to load
     */
    public static List<TLayer> calculateLoadOrder(TSimulationModel model){
        // TODO: IMPLEMENT THIS REALLY!
        return model.getLayers();
    }

}
