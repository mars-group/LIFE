package de.haw.run.pluginapi.interfaces;



public interface IRunPlugin extends ICommonPlugin {

    String tuWas();

    void writeOut(ISendable something);

    void send(ISendable objectToSend);

    ISendable getTestObject(String s);

    default void printReceivedTestObject(){
        System.out.println(receive().getName());
    }

    ISendable receive();
}
