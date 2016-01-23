package de.haw.run.prototypes.couchbase;

import com.couchbase.client.CouchbaseClient;
import com.google.gson.Gson;
import de.haw.run.prototypes.common.TestObject;

import java.util.UUID;
import java.util.concurrent.Callable;
import java.util.concurrent.TimeUnit;

public class CouchbaseThread implements Callable<Boolean> {

    private final CouchbaseClient client;
    private int elements;
    private final int total_elements;

    public CouchbaseThread(CouchbaseClient client, int elements){
        this.client = client;
        this.elements = elements;
        this.total_elements = elements;
    }

    @Override
    public Boolean call() {

        Gson gson = new Gson();

        while(!Thread.currentThread().isInterrupted()){
            String s = UUID.randomUUID().toString();
            client.set(UUID.randomUUID().toString(), 0, gson.toJson(new TestObject(s, System.currentTimeMillis())));

            elements--;

            if(elements == 0){
                System.err.println("Thread " + Thread.currentThread().getName() + " created " + total_elements + " Elements.");

                // Shutdown and wait a maximum of three seconds to finish up operations
                client.shutdown(5, TimeUnit.MINUTES);

                return true;
            }
        }

        return false;
    }
}
