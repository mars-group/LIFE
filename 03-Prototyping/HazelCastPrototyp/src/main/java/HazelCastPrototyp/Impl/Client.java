package HazelCastPrototyp.Impl;

/**
 * Created with IntelliJ IDEA.
 * User: Nils
 * Date: 04.11.13
 * Time: 12:54
 * To change this template use File | Settings | File Templates.
 */

import com.hazelcast.client.HazelcastClient;
import com.hazelcast.client.config.ClientConfig;
import com.hazelcast.core.HazelcastInstance;

import java.util.List;


public class Client {


    private HazelcastInstance client;


    public Client() {
        HazelcastInstance server = HazelcastInstanceManager.getHazelcastInstance();

        ClientConfig clientConfig = new ClientConfig();
        clientConfig.addAddress("127.0.0.1:5701");
        client = HazelcastClient.newHazelcastClient(clientConfig);

    }

    public HazelcastInstance getClient() {
        return client;
    }


}
