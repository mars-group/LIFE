import com.couchbase.client.CouchbaseClient;

import java.io.IOException;
import java.net.URI;
import java.sql.Connection;
import java.sql.DriverManager;
import java.sql.SQLException;
import java.util.LinkedList;
import java.util.List;


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

    public static Connection GetPostGRESQLConnection(String ip, int port, String db_name){
        Connection con = null;

        String url = "jdbc:postgresql://"+ip+":+"+port+"/"+db_name;
        String user = "postgres";
        String password = "IlmSG400";

        try {
            con = DriverManager.getConnection(url, user, password);
        } catch(SQLException ex) {
            System.err.println(ex);
        }

        return con;
    }

}
