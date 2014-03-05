package de.haw.run.GlobalTypes.Settings;

import de.haw.run.GlobalTypes.Exceptions.ErrorMessages;

import java.io.IOException;
import java.io.InputStream;
import java.util.Properties;

/**
 * This class can be used for reading the application's settings.
 */
public class AppSettings {

    //since we're about to handle a lot of errors, load error description file
    private Properties appSettings = getAppSettings();

    private ErrorMessages errorMessages;

    public AppSettings(){
        errorMessages = new ErrorMessages();
    }

    private Properties getAppSettings() {
        try {
            Properties tmp = new Properties();
            //tmp.load(new FileInputStream("./appSettings.properties"));
            InputStream in = getClass().getResourceAsStream("appSettings.properties");
            tmp.load(in);
            in.close();
            appSettings = tmp;
        } catch (IOException ex) {
            System.err.print("'Application settings file was not found. Please ensure that './appSettings.properties' is present and that the application's start user has sufficient rights to access it");
        }

        return appSettings;
    }

    public String getString(String key) throws SettingException {
        String result = getAppSettings().getProperty(key);
        if (result == null) {
            throw new SettingException(errorMessages.get("missingAppSetting"));
        }
        return result;
    }

    public int getInt(String key) throws SettingException {
        try {
            return Integer.parseInt(getAppSettings().getProperty(key));
        } catch (NumberFormatException ex) {
            throw new SettingException(errorMessages.get("noIntForSetting"));
        }
    }
}
