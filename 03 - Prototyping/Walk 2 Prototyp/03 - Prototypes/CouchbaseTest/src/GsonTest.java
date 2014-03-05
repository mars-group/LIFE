import com.couchbase.client.CouchbaseClient;
import com.google.gson.Gson;
import net.spy.memcached.internal.OperationFuture;

import java.util.*;
import java.util.concurrent.TimeUnit;


public class GsonTest {

    public static final int VALUE_COUNT = 500000;

    public static void main(String[] args){
        CouchbaseClient client = DBUtils.GetCouchbaseConnection("141.22.11.177", 8091, "default");
        if(client==null) return;

        Gson gson = new Gson();

        // create storage for testdata
        HashMap<String, String> data = new LinkedHashMap<>();

        System.out.println("Creating TestData...");

        //create testdata
        for(int i=0; i<VALUE_COUNT; i++){
            data.put(UUID.randomUUID().toString(), gson.toJson(new TestObject("bla", System.currentTimeMillis())));
        }
        System.out.println("...done " + data.size() + "datasets created.");

        // create storage for db operations
        final List<OperationFuture<Boolean>> ops = new LinkedList<>();

        System.out.println("Executing DB Operations...");
        long s1 = System.currentTimeMillis();
        // execute db operations
        data.entrySet().stream().parallel()
                .forEach(entry -> {
                    ops.add(client.set(entry.getKey(), 0, entry.getValue()));
                });
        long e1 = System.currentTimeMillis();
        System.out.println("...done. Took: " + (e1-s1)/1000 + "seconds.");



/*        System.out.println("Validating DB Operations...");
        long s2 = System.currentTimeMillis();
        ops.stream()
                .forEach(of -> {

                    // Check to see if our set succeeded
                    try {
                        if (of.get().booleanValue()) {
                            //System.out.println("Set Succeeded");
                        } else {
                            System.err.println("Set failed: " + of.getStatus().getMessage());
                        }
                    } catch (InterruptedException e) {
                        System.err.println("InterruptedException while doing set: " + e.getMessage());
                    } catch (ExecutionException e) {
                        System.err.println("ExecutionException while doing set: " + e.getMessage());
                    }
                });
        long e2 = System.currentTimeMillis();
        System.out.println("...done. Took: " + (e2-s2)/1000 + "seconds.");*/


//        String res = (String)client.get(KEY);
//        System.out.println(res);

        // Shutdown and wait a maximum of three seconds to finish up operations
        client.shutdown(3, TimeUnit.SECONDS);
        System.exit(0);

    }

}
