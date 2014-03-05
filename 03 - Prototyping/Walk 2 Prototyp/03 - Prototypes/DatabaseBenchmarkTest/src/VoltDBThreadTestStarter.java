import de.haw.run.prototypes.common.DBUtils;
import org.voltdb.client.ClientFactory;
import org.voltdb.client.Client;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.*;

/**
 * Project: RUN
 * User: chhuening
 * Date: 27.06.13
 * Time: 16:45
 */
public class VoltDBThreadTestStarter {


    private static final int THREAD_COUNT = 1024;
    private static final int ELEM_COUNT = 50000 / THREAD_COUNT;

    public static void main(String[] args) throws IOException, InterruptedException {
        ExecutorService es = Executors.newFixedThreadPool(THREAD_COUNT);

        List<Callable<Boolean>> callables = new LinkedList<>();

        String[] ips = new String[] {
                "141.22.32.8",
                //"141.22.32.9",
                //"141.22.32.10"
        };

        Client[] clients = new Client[3];
            for(int i=0;i<clients.length; i++){
                Client client = ClientFactory.createClient();
                client.createConnection(ips[i % ips.length]);
                clients[i] = client;
        }

        for(int i=0; i < THREAD_COUNT; i++){
            callables.add(
                    new VoltDBThread(
                            ELEM_COUNT,
                            clients[i % clients.length]
                    )
            );
        }

        long start = System.currentTimeMillis();
        List<Future<Boolean>> results = es.invokeAll(callables);
        long end = System.currentTimeMillis();
        System.out.println("Duration: " + (end-start) + "ms, " + (end-start)/1000 + "secs, " + (end-start)/1000/60.0 + "mins.");


        for(int i=0;i<ips.length; i++){
            clients[i].close();
        }

        results.forEach( f -> {
            try {
                if(!f.isDone() || !f.get()){
                    System.err.println("There were errors while executing the Inserts.");
                }
            } catch (InterruptedException e) {
                e.printStackTrace();
            } catch (ExecutionException e) {
                e.printStackTrace();
            }
        });

        es.shutdownNow();
    }
}
