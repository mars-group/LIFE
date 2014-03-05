package HazelCastPrototyp.Impl;

import java.io.Serializable;

/**
 * Created with IntelliJ IDEA.
 * User: Nils
 * Date: 04.11.13
 * Time: 13:24
 * To change this template use File | Settings | File Templates.
 */
public class SuperPower  implements Serializable {

    private String name;
    private String beschreibung;

    public String getName() {
        return name;
    }

    public String getBeschreibung() {
        return beschreibung;
    }

    @Override
    public String toString() {
        return "SuperPower{" +
                "name='" + name + '\'' +
                ", beschreibung='" + beschreibung + '\'' +
                '}';
    }

    public SuperPower(String name , String beschreibung){
        this.name = name;
        this.beschreibung = beschreibung;

    }


}
