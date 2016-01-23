package weather;

import de.haw.run.layerAPI.TLayerInitializationDataType;
import net.xeoh.plugins.base.annotations.PluginImplementation;
import weather.Interface.IWeatherLayer;

import java.util.Random;
import java.util.UUID;

@PluginImplementation
public class WeatherLayer implements IWeatherLayer {


    private Random rand;
    private int tickcount;
    private WeatherType weatherType;
    private UUID id;

    @Override
    public boolean initLayer(TLayerInitializationDataType layerInitData) {
        tickcount = 0;
        weatherType = WeatherType.SUNNY;
        rand = new Random();
        return true;
    }

    @Override
    public void advanceOneTick() {
        weatherType = WeatherType.values()[rand.nextInt(WeatherType.values().length)];
        tickcount++;
    }

    @Override
    public long getCurrentTick() {
        return tickcount;
    }

    @Override
    public UUID getID() {
        return this.id;
    }


    public void setID(UUID id) {
        this.id = id;
    }

    @Override
    public String getCurrentWeather(){
        return weatherType.toString();
    }
}
