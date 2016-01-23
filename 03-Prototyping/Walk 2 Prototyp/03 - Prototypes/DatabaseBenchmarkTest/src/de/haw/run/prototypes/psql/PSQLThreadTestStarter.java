package de.haw.run.prototypes.psql;

import de.haw.run.prototypes.common.DBUtils;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.*;


public class PSQLThreadTestStarter {

    private static final int THREAD_COUNT = 20;
    private static final int ELEM_COUNT = 500000 / THREAD_COUNT;

    public static void main(String[] args) throws IOException, InterruptedException {

        ExecutorService es = Executors.newFixedThreadPool(THREAD_COUNT);

        List<Callable<Boolean>> callables = new LinkedList<>();

        for(int i=0; i < THREAD_COUNT; i++){
              callables.add(
                      new PSQLThread(
                              DBUtils.GetPostGRESQLConnection("141.22.11.126", 5432, "datacube_test"),
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
