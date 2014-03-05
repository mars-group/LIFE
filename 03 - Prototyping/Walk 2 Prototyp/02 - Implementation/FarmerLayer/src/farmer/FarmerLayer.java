package farmer;

import farmer.Interfaces.IFarmerLayer;
import forest.Interfaces.IForestLayer;
import de.haw.run.layerAPI.TLayerInitializationDataType;
import net.xeoh.plugins.base.annotations.PluginImplementation;
import net.xeoh.plugins.base.annotations.injections.InjectPlugin;
import weather.Interface.IWeatherLayer;

import java.util.LinkedList;
import java.util.List;
import java.util.UUID;

@PluginImplementation
public class FarmerLayer implements IFarmerLayer {

    private UUID id;
    private int tickcount;
    private List<Farmer> farmers;

    @InjectPlugin
    public IWeatherLayer weather;

    @InjectPlugin
    public IForestLayer forest;

    @Override
    public boolean initLayer(TLayerInitializationDataType layerInitData) {
        tickcount = 0;

        farmers = new LinkedList<>();
        for(int i=0; i < 100; i++){
            farmers.add(new Farmer(weather, forest));
        }

        return true;
    }

    @Override
    public void advanceOneTick() {
        farmers.forEach(Farmer::act);
        tickcount++;
    }

    @Override
    public long getCurrentTick() {
        return tickcount;
    }

    @Override
    public UUID getID() {
        return id;
    }

    public void setID(UUID id) {
        this.id = id;
    }
}
