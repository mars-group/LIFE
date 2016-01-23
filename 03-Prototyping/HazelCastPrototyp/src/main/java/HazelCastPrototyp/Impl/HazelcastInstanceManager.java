package HazelCastPrototyp.Impl;

import com.hazelcast.core.Hazelcast;
import com.hazelcast.core.HazelcastInstance;

/**
 * Created with IntelliJ IDEA.
 * User: Nils
 * Date: 04.11.13
 * Time: 13:12
 * To change this template use File | Settings | File Templates.
 */
public class HazelcastInstanceManager {


    public static HazelcastInstance hazelcastInstance;

    public static HazelcastInstance getHazelcastInstance(){
        if (hazelcastInstance == null){
            hazelcastInstance = Hazelcast.newHazelcastInstance();

        }
        return hazelcastInstance;

    }


}
