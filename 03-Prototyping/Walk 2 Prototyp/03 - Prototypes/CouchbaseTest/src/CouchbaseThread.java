import com.couchbase.client.CouchbaseClient;
import com.google.gson.Gson;

import java.util.*;
import java.util.concurrent.TimeUnit;

public class CouchbaseThread implements Runnable {

    private final CouchbaseClient client;

    public CouchbaseThread(CouchbaseClient client){
        this.client = client;
    }

    @Override
    public void run() {


        Gson gson = new Gson();



        while(!Thread.currentThread().isInterrupted()){
            String s = UUID.randomUUID().toString();
            client.set(UUID.randomUUID().toString(), 0, gson.toJson(new TestObject(s, System.currentTimeMillis())));
         }


        // Shutdown and wait a maximum of three seconds to finish up operations
        client.shutdown(5, TimeUnit.MINUTES);
        System.exit(0);
    }
}
