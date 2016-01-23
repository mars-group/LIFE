package de.haw.run.services;

public interface IGlobalClock {

    /**
     * Calls the local instance of the SAPDClock to return the current time
     * @return the current global time as an long.
     */
    public long getTime();

    /**
     * Starts the local de.haw.run.services.globalclock.SAPDTimer instance. If started with sponsor = false, which is the default, the node's
     * timer instance will wait for another node to initiate the initial calibration.
     * @param sponsor indicates whether this node should act as the sponsor node to the SAPD Algorithm
     */
    public void startTimer(boolean sponsor);

    /**
     * Default implementation of startTimer(). Assumes to not be the sponsoring node.
     */
    default public void startTimer() {
        startTimer(false);
    }

    /**
     * Stops the timer process
     */
    public void stopTimer();

    /**
     * Sets a threaded Timer and executes the interface method from IAlarmClock, when the Timers time has come.
     * @param wakeUpTime
     * @param wakeMeUp
     * @return The Thread which waits for wakeUpTime, may be joined or not...
     */
    public Thread setTimer(long wakeUpTime, IAlarmClock wakeMeUp);
}