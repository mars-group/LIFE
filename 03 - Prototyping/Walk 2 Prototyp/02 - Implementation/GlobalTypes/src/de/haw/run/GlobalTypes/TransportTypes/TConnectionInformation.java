package de.haw.run.GlobalTypes.TransportTypes;

import java.io.Serializable;

/**
 * Project: RUN
 * User: chhuening
 * Date: 03.09.13
 * Time: 20:56
 */
public class TConnectionInformation implements Serializable {
    private String ipAddress;
    private int port;


    public TConnectionInformation(String ipAddress, int port) {
        this.ipAddress = ipAddress;
        this.port = port;
    }


    public String getIpAddress() {
        return ipAddress;
    }

    public int getPort() {
        return port;
    }
}
