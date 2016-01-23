package de.haw.run.layercontainer.layercontainercontroller.layermanagement;

import de.haw.run.GlobalTypes.Exceptions.LayerExecutionError;
import de.haw.run.GlobalTypes.Settings.AppSettings;
import de.haw.run.GlobalTypes.Settings.SettingException;
import de.haw.run.layerAPI.TLayerInitializationDataType;
import de.haw.run.layercontainer.ISwitchExecutionMode;
import de.haw.run.services.IGlobalClock;
import de.haw.run.layerAPI.ILayer;

import java.util.List;
import java.util.UUID;
import java.util.concurrent.*;
import java.util.stream.Collectors;

public class LayerController {
    private LayerRepository layerRepository;
    private IGlobalClock globalClock;
    private ISwitchExecutionMode executionModeSwitcher;
    private IExecutionController tickDurationReporter;
    private ScheduledExecutorService threadPoolExecutor;
    private ExecutionMode executionMode;


    private long startDelayForDezentralizedLayerExecution;
    private long executionInterval;

    public LayerController(LayerRepository layerRepository, IGlobalClock globalClock, ISwitchExecutionMode executionModeSwitcher) {

        this.layerRepository = layerRepository;
        this.globalClock = globalClock;
        this.executionModeSwitcher = executionModeSwitcher;

        // set to Central Simulation Execution at start
        this.executionMode = ExecutionMode.CSE;

        this.executionInterval = -1;

        this.tickDurationReporter = new ExecutionController();

        try {
            this.startDelayForDezentralizedLayerExecution = Long.parseLong(new AppSettings().getString("startDelayForDezentralizedLayerExecution"));
        } catch (SettingException e) {
            e.printStackTrace();
        }
    }

    public boolean initializeLayerFromModel(UUID layerID, TLayerInitializationDataType layerInitData) {
        ILayer layer = layerRepository.getLayerByID(layerID);
        return layer.initLayer(layerInitData);
	}

    public long advanceAllLayersOneTick() throws LayerExecutionError {
        List<ILayer> layers = layerRepository.getAllLayers();

        ExecutorService es = Executors.newFixedThreadPool(layers.size());

        List<Callable<Long>> layerThreads = layers.parallelStream()
                .map(l -> new LayerAdvancementThread(l))
                .collect(Collectors.toList());

        try {
            // invoke all LayerThreads
            List<Future<Long>> results = es.invokeAll(layerThreads);

            // try to get the maximum of the returned durations
            return results.parallelStream()
                    .max((a, b) -> {
                        try {
                            return Long.compare(a.get(), b.get());
                        } catch (InterruptedException | ExecutionException e) {
                            e.printStackTrace();
                        }
                        return -1;
                    }).get().get();

        } catch (InterruptedException | ExecutionException e) {
            e.printStackTrace();
        }

        // something went wrong, throw error
        throw new LayerExecutionError();
    }

	public boolean startAllLayers(long startTime, long interval) {
            try {
                List<ILayer> layers = layerRepository.getAllLayers();
                int layerCount = layers.size();

                this.executionInterval = interval;

                threadPoolExecutor = Executors.newScheduledThreadPool(layerCount);

                // set a Timer for the received startTime
                Thread timer = globalClock.setTimer(startTime, () ->
                        // when startTime is reached, schedule execution of advanceOneTick in 'interval' periods
                        layers.parallelStream()
                            .forEach(layer ->
                                    {
                                        threadPoolExecutor.scheduleAtFixedRate(
                                                () -> {
                                                    long start = System.currentTimeMillis();
                                                    layer.advanceOneTick();
                                                    System.out.println("Advancing...8-)");
                                                    long end = System.currentTimeMillis();
                                                    tickDurationReporter.reportTickDuration(layer.getID(), layer.getCurrentTick(), end - start);
                                                },
                                                startDelayForDezentralizedLayerExecution,
                                                interval,
                                                TimeUnit.MILLISECONDS
                                        );
                                    }
                            ));

                // wait for the timer to be done, because LayerComponentController works like that.
                timer.join();

            } catch (Exception e) {
                e.printStackTrace();
                return false;
            }

            return true;
    }

    private void optimizeExecution(long duration) {
        // TODO: Really optimize, just do nothing for now
    }

    private class ExecutionController implements IExecutionController {

        // TODO: Make more sophisticated. Will do for now.

        private UUID longestLayerID;
        private long longestTick;
        private float longestDuration;

        public ExecutionController(){
            longestLayerID = null;
            longestTick = -1;
            longestDuration = -1;
        }

        @Override
        public void reportTickDuration(UUID layerID, long currentTick, long duration) {

            // update local status
            if(duration > longestDuration){
                longestDuration = duration;
                longestTick = currentTick;
                longestLayerID = layerID;
            }

            // check if mode must be switched or execution can be optimized
            if(executionMode.equals(ExecutionMode.DSCE) && duration >= executionInterval){
                executionModeSwitcher.switchMode(currentTick, duration);
            } else if(executionMode.equals(ExecutionMode.DSCE) && duration < executionInterval) {
                // TODO: Consider threshold for optimization
                optimizeExecution(duration);
            }
        }

        private UUID getLongestLayerID() {
            return longestLayerID;
        }

        private long getLongestTick() {
            return longestTick;
        }

        private float getLongestDuration() {
            return longestDuration;
        }
    }



}