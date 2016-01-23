package de.haw.run.prototypes.mysql;

import de.haw.run.prototypes.common.DBUtils;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.*;


public class MySQLThreadTestStarter {

    private static final int THREAD_COUNT = 20;
    private static final int ELEM_COUNT = 5000 / THREAD_COUNT;

    public static void main(String[] args) throws IOException, InterruptedException {

        ExecutorService es = Executors.newFixedThreadPool(THREAD_COUNT);

        List<Callable<Boolean>> callables = new LinkedList<>();

        for(int i=0; i < THREAD_COUNT; i++){
              callables.add(
                      new MySQLThread(
                              DBUtils.GetMySQLConnection("3ten.de", 3306, "datacube_test_ch"),
                              ELEM_COUNT
                      )
              );
        }

        long start = System.currentTimeMillis();
        List<Future<Boolean>> results = es.invokeAll(callables);
        long end = System.currentTimeMillis();
        System.out.println("Duration: " + (end-start) + "ms, " + (end-start)/1000 + "secs, " + (end-start)/1000/60.0 + "mins.");

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
