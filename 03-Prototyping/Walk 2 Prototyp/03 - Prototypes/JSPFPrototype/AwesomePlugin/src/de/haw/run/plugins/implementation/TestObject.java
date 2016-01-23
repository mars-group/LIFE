package de.haw.run.plugins.implementation;


import de.haw.run.pluginapi.interfaces.ISendable;

import java.io.Serializable;

public class TestObject implements ISendable {

    private final String name;

    public TestObject(String name){
        this.name = name;
    }

    public String getName() {
        return name;
    }
}
