package de.haw.run.NetworkAdapter.Implementation;

import de.haw.run.NetworkAdapter.Interface.INetworkMessageReceivedEventHandler;
import de.haw.run.NetworkAdapter.Messages.NetworkMessage;


import java.util.Iterator;
import java.util.LinkedList;
import java.util.List;

/**
 * This class manages a list of listeners for the event, that a certain network message arrives.<br/>
 * It also informs all listeners comfortably about the event.
 */
class SubscriberCollection implements Iterable<INetworkMessageReceivedEventHandler>{

    private List<INetworkMessageReceivedEventHandler> subscribers;

    //Initializes a new, empty collection of subscribers.
    public SubscriberCollection() {
        subscribers = new LinkedList<INetworkMessageReceivedEventHandler>();
    }

    /**
     * adds an event listener to the collection of subscribers
     *
     * @param subscriber
     */
    public void addSubscriber(INetworkMessageReceivedEventHandler subscriber) {
        subscribers.add(subscriber);
    }

    /**
     * Synchronously delivers the message to all subscribers.
     * Does nothing, if no listeners present.
     *
     * @param message
     */
    public void deliverMessage(NetworkMessage message) {
        for (INetworkMessageReceivedEventHandler listener : subscribers) {
            listener.onMessageReceived(message);
        }
    }

    @Override
    public Iterator<INetworkMessageReceivedEventHandler> iterator() {
        return subscribers.iterator();
    }
}