import com.couchbase.client.CouchbaseClient;

import java.io.IOException;
import java.net.URI;
import java.util.LinkedList;
import java.util.List;

/**
 * Created with IntelliJ IDEA.
 * User: chhuening
 * Date: 13.06.13
 * Time: 12:01
 * To change this template use File | Settings | File Templates.
 */
public class DBUtils {


    public static CouchbaseClient GetCouchbaseConnection(String ip, int port, String db_name){
        // Set the URIs and get a client
        List<URI> uris = new LinkedList<URI>();

        // Connect to localhost or to the appropriate URI(s)
        uris.add(URI.create("http://"+ip+":"+port+"/pools"));
        // uris.add(URI.create("http://192.168.1.102:8091/pools"));

        CouchbaseClient client = null;
        try {
            // Use the "default" bucket with no password
            client = new CouchbaseClient(uris, db_name, "");
        } catch (IOException e) {
            System.err.println("IOException connecting to Couchbase: " + e.getMessage());
            System.exit(1);
        }
        return client;
    }
}
