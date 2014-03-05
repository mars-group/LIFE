package de.haw.run.GlobalTypes.Exceptions;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

/**
 * Helper class for accessing the stored error messages.
 */
public class ErrorMessages {

    private Properties errorCodes;

    public ErrorMessages(){
        loadProperties();
    }

    private void loadProperties() {
        try {
                errorCodes = new Properties();
                //errorCodes.load(new FileInputStream("./errorCodes.properties"));
                InputStream in = getClass().getResourceAsStream("errorCodes.properties");
                errorCodes.load(in);
                in.close();
            } catch (IOException ex) {
                System.err.println("Error code file was not found. Please ensure that the errorCodes.properties is present and that the application's start user has sufficient rights to access it.");
            }
    }

    public String get(String errorCode) {
        return errorCodes.getProperty(errorCode);
    }
}
