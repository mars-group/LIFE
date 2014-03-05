import de.haw.run.pluginapi.interfaces.IRunPlugin;
import net.xeoh.plugins.base.PluginManager;
import net.xeoh.plugins.base.impl.PluginManagerFactory;
import net.xeoh.plugins.remote.ExportResult;
import net.xeoh.plugins.remote.RemoteAPI;
import net.xeoh.plugins.remote.RemoteAPILipe;

import java.io.File;

public class ServerStarter {

    public static void main(String[] args) {
        PluginManager pm = PluginManagerFactory.createPluginManager();
        pm.addPluginsFrom(new File("out/artifacts/AwesomePlugin_jar/").toURI());

        IRunPlugin rp = pm.getPlugin(IRunPlugin.class);

        RemoteAPI remote = pm.getPlugin(RemoteAPILipe.class);

        ExportResult exportResult = remote.exportPlugin(rp);

        System.out.println(rp.tuWas());

    }

}
