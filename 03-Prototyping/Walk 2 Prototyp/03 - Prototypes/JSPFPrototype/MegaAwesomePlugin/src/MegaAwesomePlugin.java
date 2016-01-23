import de.haw.run.pluginapi.interfaces.IRunPlugin;
import de.haw.run.pluginapi.interfaces.IWalkPlugin;
import net.xeoh.plugins.base.annotations.PluginImplementation;
import net.xeoh.plugins.base.annotations.injections.InjectPlugin;

@PluginImplementation
public class MegaAwesomePlugin implements IWalkPlugin {

    @InjectPlugin
    public IRunPlugin rp;



    @Override
    public void sagHallo() {
        System.out.println(" I am a WALK Plugin and can use the IRunInterface, experience my power: " + rp.tuWas());
    }
}
