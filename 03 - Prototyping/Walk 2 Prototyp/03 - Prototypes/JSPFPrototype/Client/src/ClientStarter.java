import de.haw.run.pluginapi.interfaces.ICommonPlugin;
import de.haw.run.pluginapi.interfaces.IWalkPlugin;
import net.xeoh.plugins.base.PluginManager;
import net.xeoh.plugins.base.impl.PluginManagerFactory;
import net.xeoh.plugins.base.options.addpluginsfrom.OptionReportAfter;
import net.xeoh.plugins.base.util.PluginManagerUtil;

import java.io.File;
import java.io.IOException;
import java.net.URI;
import java.net.URISyntaxException;
import java.util.ArrayList;
import java.util.List;


public class ClientStarter {

    public static void main(String[] args) throws IOException, URISyntaxException {

        PluginManager pm = PluginManagerFactory.createPluginManager();
        PluginManagerUtil util = new PluginManagerUtil(pm);

        //pm.addPluginsFrom(new File("./out/artifacts/AwesomePlugin_jar/").toURI(), new OptionReportAfter());

        //pm.addPluginsFrom(new File("./out/artifacts/MegaAwesomePlugin_jar/").toURI(), new OptionReportAfter());

        pm.addPluginsFrom(new URI("http://3ten.de/download/ch/"), new OptionReportAfter());

        //pm.addPluginsFrom(new URI("http://3ten.de/download/ch/MegaAwesomePlugin.jar"), new OptionReportAfter());

        List<ICommonPlugin> plugins = new ArrayList<>(util.getPlugins(ICommonPlugin.class));


        plugins.stream().forEach(p -> System.out.println(p.toString()));

        IWalkPlugin wp = pm.getPlugin(IWalkPlugin.class);

        wp.sagHallo();

    }
}
