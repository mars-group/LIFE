

import com.couchbase.client.CouchbaseClient;
import com.google.gson.Gson;
import net.spy.memcached.internal.OperationFuture;

import java.io.IOException;
import java.util.*;
import java.util.concurrent.TimeUnit;


public class GsonTestThreaded {

    public static final int THREAD_COUNT = 8;

    public static void main(String[] args) throws IOException {

        String db_name = "default";

        if(args.length > 0 && args[0] != ""){
            db_name = args[0];
        }

        CouchbaseClient client = DBUtils.GetCouchbaseConnection("141.22.11.177", 8091, db_name);
        if(client==null) return;
        for(int i=0; i < THREAD_COUNT; i++){
            Thread t = new Thread(new CouchbaseThread(client));
            t.setDaemon(true);
            t.start();
        }
        System.in.read();
    }

}
