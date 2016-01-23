package de.haw.run.layerAPI;

import net.xeoh.plugins.base.Plugin;
import java.util.UUID;

/**
 * This interface must be implemented by anyone who
 * wants to create a new Layer runnable in a LayerContainer.
 * The interface IServices may be used inside of that
 * implementation to access the provided services
 * from the LayerContainer.
 */
public interface ILayer extends Plugin {

    /**
     * Initializes the layer with layerInitData.
     * Use this instead of the constructor, as it is
     * guaranteed to be called in the correct load order.
     * @caution MUST NOT USE IGlobalClock (!!!), since
     * this will be in sync only after initialization
     * of all layers.
     * @pre This layer was successfully added to
     * its container.
     * @post This layer is in a state which allows
     * it to start the simulation.
     * @param layerInitData A datatype holding the
     * information of how to initialize a layer.
     * @return A boolean indicating whether or not
     * initialization was successful.
     */
	public boolean initLayer(TLayerInitializationDataType layerInitData);

    /**
     * Advances the layer by one tick. A tick is
     * the smallest unit of time possible in the current model.
     * If this layer is a static one, advanceOneTick
     * may be an empty implementation which simply
     * increases the tickCount.
     * @pre initLayer() was called and returned true.
     * @post getCurrentTick() returns a by 1
     * increased value
     */
	public void advanceOneTick();

    /**
     * The current tick the layer is in.
     * @return Positive long value, if in active
     * simulation or if simulation has ended, -1 otherwise
     */
	public long getCurrentTick();

    /**
     * The unique ID of this layer as a UUID.
     * @return UUID representing the unique ID
     * of this layer.
     */
    public UUID getID();

}