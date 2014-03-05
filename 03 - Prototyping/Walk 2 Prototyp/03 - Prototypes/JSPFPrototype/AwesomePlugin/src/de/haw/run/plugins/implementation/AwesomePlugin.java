package de.haw.run.plugins.implementation;

import de.haw.run.pluginapi.interfaces.IRunPlugin;
import de.haw.run.pluginapi.interfaces.ISendable;
import net.xeoh.plugins.base.annotations.*;
import java.io.InputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.OutputStream;
import java.net.ServerSocket;
import java.net.Socket;

@PluginImplementation
public class AwesomePlugin implements IRunPlugin {

    public String tuWas() {
        return "I am a plugin and alive!";
    }

    @Override
    public void writeOut(ISendable something) {
        System.out.println(something.getName());
    }

    @Override
    public ISendable getTestObject(String s) {
        return new TestObject(s);
    }

    @Override
    public void send(ISendable objectToSend) {
        try{
            //Socket s = new Socket("141.22.11.177",54321);
            Socket s = new Socket("localhost",54321);
            OutputStream os = s.getOutputStream();
            ObjectOutputStream oos = new ObjectOutputStream(os);
            oos.writeObject(objectToSend);
            oos.close();
            os.close();
            s.close();
        }catch(Exception e){System.out.println(e);}
    }


    @Override
    public ISendable receive() {
        int port = 54321;

        try {
            ServerSocket ss = new ServerSocket(port);
            Socket s = ss.accept();
            InputStream is = s.getInputStream();
            ObjectInputStream ois = new ObjectInputStream(is);
            TestObject to = (TestObject) ois.readObject();
            is.close();
            s.close();
            ss.close();
            if(to != null) return to;
        } catch (Exception e) {
            System.out.println(e);
        }

        return null;
    }
}
