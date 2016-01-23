package forest.Interfaces;



import de.haw.run.layerAPI.ILayer;

import java.util.List;


public interface IForestLayer extends ILayer {
    public List<Integer> getTrees();

    public Integer removeTree(Integer treeID);
}
