package forest;

import forest.Interfaces.IForestLayer;
import de.haw.run.layerAPI.TLayerInitializationDataType;
import net.xeoh.plugins.base.annotations.PluginImplementation;

import java.util.LinkedList;
import java.util.List;
import java.util.Random;
import java.util.UUID;
import java.util.stream.Collectors;

@PluginImplementation
public class ForestLayer implements IForestLayer {
    private List<Tree> trees;
    private long tickcount;
    private UUID id;
    private Random rand;

    @Override
    public boolean initLayer(TLayerInitializationDataType layerInitData) {
        rand = new Random();
        trees = new LinkedList<>();
        for(int i=0; i < 1000; i++){
            trees.add(new Tree(rand.nextInt(200), rand.nextInt(150), rand.nextInt(100), i));
        }
        return true;
    }

    @Override
    public void advanceOneTick() {
       tickcount++;
        try {
            Thread.sleep(1000);
        } catch (InterruptedException e) {
            e.printStackTrace();
        }
    }

    @Override
    public long getCurrentTick() {
        return tickcount;
    }

    @Override
    public UUID getID() {
        return id;
    }


    public void setID(UUID id) {
        this.id = id;
    }

    @Override
    public List<Integer> getTrees() {
        if(trees.isEmpty()){
            return new LinkedList<>();
        } else {
            return trees.parallelStream().map(tr -> tr.getID()).collect(Collectors.toList());
        }
    }

    @Override
    public Integer removeTree(Integer tree) {
        Tree tr = trees.parallelStream().filter(t -> t.getID() == tree).findFirst().get();
        trees.remove(tr);
        return tree;
    }
}
