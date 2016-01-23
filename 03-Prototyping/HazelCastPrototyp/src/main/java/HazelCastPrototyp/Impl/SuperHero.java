package HazelCastPrototyp.Impl;

import java.io.Serializable;
import java.util.ArrayList;
import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: Nils
 * Date: 04.11.13
 * Time: 12:53
 * To change this template use File | Settings | File Templates.
 */

public class SuperHero implements Serializable{

    private String name;
    private List<SuperPower> superPowers;

     public SuperHero(String name){
        this.name = name;
         superPowers = new ArrayList<SuperPower>();
     }

    public void addSuperPower(SuperPower superPower){
        superPowers.add(superPower);
    }

    public List<SuperPower> getSuperPowers() {
        return superPowers;
    }

    public String getName() {
        return name;
    }

    @Override
    public String toString() {
        return "Superhero{" +
                "name='" + name + '\'' +
                ", superPowers=" + superPowers +
                '}';
    }
}
