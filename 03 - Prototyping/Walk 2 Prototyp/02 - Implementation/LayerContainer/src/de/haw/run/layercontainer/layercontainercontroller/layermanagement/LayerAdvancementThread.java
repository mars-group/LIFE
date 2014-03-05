package de.haw.run.layercontainer.layercontainercontroller.layermanagement;

import de.haw.run.layerAPI.ILayer;

import java.util.concurrent.Callable;

/**
 * User: Chris
 * Date: 01.09.13
 */
public class LayerAdvancementThread implements Callable<Long> {

    private ILayer layer;

    /**
     * Advances the given layer by one tick and measures the duration of execution
     * @param layer  The layer to advance and measure
     */
    public LayerAdvancementThread(ILayer layer){

        this.layer = layer;
    }

    @Override
    public Long call() {
        long start = System.currentTimeMillis();
        layer.advanceOneTick();
        long end  = System.currentTimeMillis();
        return end-start;
    }
}
