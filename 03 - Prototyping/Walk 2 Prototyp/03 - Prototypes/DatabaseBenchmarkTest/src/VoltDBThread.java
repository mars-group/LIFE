import de.haw.run.prototypes.common.TestObject;
import org.voltdb.client.Client;
import org.voltdb.client.ClientConfig;
import org.voltdb.client.ClientFactory;
import org.voltdb.client.ProcCallException;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.UUID;
import java.util.concurrent.Callable;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

/**
 * Project: RUN
 * User: chhuening
 * Date: 27.06.13
 * Time: 16:45
 */
public class VoltDBThread implements Callable<Boolean> {

    private final Client client;
    private int elements;
    private final int total_elements;

    public VoltDBThread(int elements, Client client) throws IOException {
        this.elements = elements;
        this.total_elements = elements;
             this.client = client;
//        client = ClientFactory.createClient();
//        client.createConnection(ipAddress);
    }

    @Override
    public Boolean call() throws Exception {
        while(!Thread.currentThread().isInterrupted() && elements-- > 0){
            String s = UUID.randomUUID().toString();
            TestObject to = new TestObject(s, System.currentTimeMillis());
            client.callProcedure("InsertDimension1", to.getT1(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension2", to.getT2(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension3", to.getT3(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension4", to.getT4(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension5", to.getT5(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension6", to.getT6(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension7", to.getT7(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension8", to.getT8(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension9", to.getT9(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertDimension10", to.getT10(), to.getTime(), to.getId().toString());
            client.callProcedure("InsertFact", "fact_value", to.getId().toString());
        }
//        System.out.println("Thread " + Thread.currentThread().getName() + " created " + total_elements + " Elements.");
        return true;
    }
}
