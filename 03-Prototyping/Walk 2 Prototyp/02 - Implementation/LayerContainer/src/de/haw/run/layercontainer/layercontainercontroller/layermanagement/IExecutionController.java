package de.haw.run.layercontainer.layercontainercontroller.layermanagement;

import java.util.UUID;

/**
 * Project: RUN
 * User: chhuening
 * Date: 02.09.13
 * Time: 18:49
 */
public interface IExecutionController {

    public void reportTickDuration(UUID layerID, long currentTick, long duration);

}
