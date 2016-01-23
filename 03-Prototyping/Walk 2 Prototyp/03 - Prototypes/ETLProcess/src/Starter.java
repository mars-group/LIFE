import com.couchbase.client.CouchbaseClient;
import com.couchbase.client.protocol.views.View;
import com.couchbase.client.protocol.views.Query;
import com.couchbase.client.protocol.views.ViewResponse;
import com.couchbase.client.protocol.views.ViewRow;
import com.google.gson.Gson;

import java.sql.*;
import java.util.Iterator;
import java.util.concurrent.TimeUnit;


public class Starter {

    public static void main(String[] args){
        CouchbaseClient client = DBUtils.GetCouchbaseConnection("141.22.11.177", 8091, "default");
        if(client == null) return;

        Connection con = DBUtils.GetPostGRESQLConnection("141.22.11.126", 5432, "datacube_test");

        Gson gson = new Gson();

        //retreive data from couchbase
        View view = client.getView("oldestDoc","GetOldestDocument");
        Query query = new Query();
        query.setIncludeDocs(true);
        query.setLimit(1);

        ViewResponse response = client.query(view, query);

        Iterator<ViewRow> iter = response.iterator();

        while(iter.hasNext()){
            ViewRow v = iter.next();
            Object doc = v.getDocument();
            // jump over if document is null
            if(doc==null) continue;

            TestObject to = gson.fromJson((String)doc, TestObject.class);

            try {
                Statement st = null;
                ResultSet rs = null;
                st = con.createStatement();
                long time = to.getTime();
                String value = to.getT1();
                st.execute(
                        "INSERT INTO d_t1 (value, time) VALUES ('"+to.getT1()+"',"+time+");\n" +
                        "INSERT INTO d_t2 (value, time) VALUES ('"+to.getT2()+"',"+time+");\n" +
                        "INSERT INTO d_t3 (value, time) VALUES ('"+to.getT3()+"',"+time+");\n" +
                        "INSERT INTO d_t4 (value, time) VALUES ('"+to.getT4()+"',"+time+");\n" +
                        "INSERT INTO d_t5 (value, time) VALUES ('"+to.getT5()+"',"+time+");\n" +
                        "INSERT INTO d_t6 (value, time) VALUES ('"+to.getT6()+"',"+time+");\n" +
                        "INSERT INTO d_t7 (value, time) VALUES ('"+to.getT7()+"',"+time+");\n" +
                        "INSERT INTO d_t8 (value, time) VALUES ('"+to.getT8()+"',"+time+");\n" +
                        "INSERT INTO d_t9 (value, time) VALUES ('"+to.getT9()+"',"+time+");\n" +
                        "INSERT INTO d_t10 (value, time) VALUES ('"+to.getT10()+"',"+time+");\n" +

                        "INSERT INTO facts (f_fact, f_t1, f_t2, f_t3, f_t4, f_t5, f_t6, f_t7, f_t8, f_t9, f_t10) VALUES ('fact_value',\n" +
                        "                           (SELECT id FROM d_t1 WHERE d_t1.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t2 WHERE d_t2.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t3 WHERE d_t3.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t4 WHERE d_t4.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t5 WHERE d_t5.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t6 WHERE d_t6.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t7 WHERE d_t7.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t8 WHERE d_t8.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t9 WHERE d_t9.time = '"+time+"' AND value = '"+value+"'),\n" +
                        "                           (SELECT id FROM d_t10 WHERE d_t10.time = '"+time+"')\n" +
                        "                         );");
            } catch (SQLException ex) {
                System.err.println(ex);
            }

            client.delete(v.getId());

        }


        // Shutdown and wait a maximum of three seconds to finish up operations
        client.shutdown(5, TimeUnit.MINUTES);
        System.exit(0);

    }
}
