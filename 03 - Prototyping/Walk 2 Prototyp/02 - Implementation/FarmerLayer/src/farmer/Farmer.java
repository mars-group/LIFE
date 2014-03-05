package farmer;


import forest.Interfaces.IForestLayer;
import weather.Interface.IWeatherLayer;

import java.util.Optional;

public class Farmer {

    private IWeatherLayer weather;
    private IForestLayer forest;

    public Farmer(IWeatherLayer weather, IForestLayer forest){

        this.weather = weather;
        this.forest = forest;
    }

    public void act() {
        if(weather.getCurrentWeather().equals("MASSIVE MEATBALLS, HACK ATTACK ;-)")){
            Optional<Integer> result = forest.getTrees().parallelStream().findAny();
            if(result.isPresent()){
                cutTree(result.get());
            }



        }
    }

    private Integer cutTree(Integer tree){
        return forest.removeTree(tree);
    }
}
