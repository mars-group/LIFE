package de.haw.run.layercontainer;


/**
 * User: Chris
 * Date: 02.09.13
 */

public interface ISwitchExecutionMode {

    /**
     * Order LayerContainerController to switch the ExecutionMode from DCSE to CSE.
     * @param tick
     * @param longestDuration
     */
    public void switchMode(long tick, long longestDuration);

}
