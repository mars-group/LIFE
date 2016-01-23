package de.walk.prototype.datacube;

import org.voltdb.client.Client;
import org.voltdb.client.ClientFactory;

import java.io.IOException;
import java.util.UUID;
import java.util.concurrent.Callable;

/**
 * Created with IntelliJ IDEA.
 * User: Jan Busch
 * Date: 06.11.13
 * Time: 10:47
 * To change this template use File | Settings | File Templates.
 */
public class VoltDBThread implements Callable<Boolean> {

    private final Client client;
    private int elements;
    private final int total_elements;

    public VoltDBThread(int elements) throws IOException {
        this.elements = elements;
        this.total_elements = elements;

        client = ClientFactory.createClient();
        client.createConnection("localhost"); //"141.22.32.8"
    }


    @Override
    public Boolean call() throws Exception {
        while (!Thread.currentThread().isInterrupted()) {
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


            elements--;

            if (elements == 0) {
                System.out.println("Thread " + Thread.currentThread().getName() + " created " + total_elements + " Elements.");
                return true;
            }

        }
        return false;
    }
}
