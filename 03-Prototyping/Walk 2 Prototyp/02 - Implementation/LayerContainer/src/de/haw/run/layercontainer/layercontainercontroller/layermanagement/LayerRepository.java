package de.haw.run.layercontainer.layercontainercontroller.layermanagement;


import de.haw.run.GlobalTypes.Exceptions.LayerNotAddableException;
import de.haw.run.layerAPI.ILayer;
import net.xeoh.plugins.base.PluginManager;
import net.xeoh.plugins.base.impl.PluginManagerFactory;
import net.xeoh.plugins.base.util.PluginManagerUtil;

import java.net.URI;
import java.util.*;

public class LayerRepository {
	private Map<UUID, ILayer> layerMap;
    private List<ILayer> loadedPlugins;

    private PluginManager pm;
    private PluginManagerUtil pmu;

    public LayerRepository(){
        pm = PluginManagerFactory.createPluginManager();
        pmu = new PluginManagerUtil(pm);
        loadedPlugins = new LinkedList<>();
        layerMap = new Hashtable<>();
    }

    /**
     * Adds a Layer from layerURI with the provided ID. The ID is assumed to be provided by the SimulationCore.
     * @param layerURI
     * @param layerID
     * @return
     * @throws LayerNotAddableException
     */
	public UUID addLayer(URI layerURI, UUID layerID)throws LayerNotAddableException {
		ILayer layer = loadLayer(layerURI);
        if(layer == null){
            throw new LayerNotAddableException();
        }
        layerMap.put(layerID, layer);
        return layerID;
	}

	public boolean deleteLayer(UUID layerID) {
	    if(layerMap.remove(layerID) == null){
            return true;
        }
        return false;
	}

    public ILayer getLayerByID(UUID layerID){
        return layerMap.get(layerID);
    }

	public List<ILayer> getAllLayers() {
		return new LinkedList<>(layerMap.values());
	}

	private ILayer loadLayer(URI layerURI) {
		pm.addPluginsFrom(layerURI);

        // Get all Plugins currently loaded in PluginManager
        LinkedList<ILayer> plugins = new LinkedList<>(pmu.getPlugins(ILayer.class));

        // Find the single new plugin
        ILayer newPlugin = plugins.stream().parallel()
                .filter(p -> !loadedPlugins.contains(p))
                .findFirst()
                .get();

        // add it to the loaded plugins
        loadedPlugins.add(newPlugin);

        // export the Plugin so that other LayerContainer Instances are able to find it
        pm.getRemote().exportPlugin(newPlugin);

        return newPlugin;
	}

    public int getLayerCount() {
        return layerMap.size();
    }
}