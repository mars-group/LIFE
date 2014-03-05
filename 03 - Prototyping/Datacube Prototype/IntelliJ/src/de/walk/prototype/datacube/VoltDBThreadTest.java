package de.walk.prototype.datacube;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;
import java.util.concurrent.*;

/**
 * Created with IntelliJ IDEA.
 * User: Jan Busch
 * Date: 06.11.13
 * Time: 10:43
 */
public class VoltDBThreadTest {
    private static final int THREAD_COUNT = 8;
    private static final int ELEM_COUNT = 10000 / THREAD_COUNT;

    public static void main(String[] args) throws IOException, InterruptedException {

        ExecutorService es = Executors.newFixedThreadPool(THREAD_COUNT);

        List<Callable<Boolean>> callables = new LinkedList<Callable<Boolean>>();

        for (int i = 0; i < THREAD_COUNT; i++) {
            callables.add(
                    new VoltDBThread(
                            ELEM_COUNT
                    )
            );
        }


        System.out.println("Starting benchmark with " + THREAD_COUNT + " with " + ELEM_COUNT + " elements.");
        long start = System.currentTimeMillis();
        List<Future<Boolean>> results = es.invokeAll(callables);
        long end = System.currentTimeMillis();
        System.out.println("Duration: " + (end - start) + "ms, " + (end - start) / 1000 + "secs, " + (end - start) / 1000 / 60.0 + "mins.");

        for (Future<Boolean> f : results) {
            try {
                if (!f.isDone() || !f.get()) {
                    System.err.println("There were errors while executing the Inserts.");
                }
            } catch (InterruptedException e) {
                e.printStackTrace();
            } catch (ExecutionException e) {
                e.printStackTrace();
            }
        }

        es.shutdownNow();
    }
}
