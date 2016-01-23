package de.haw.run.NetworkAdapter.Messages.initialization;

import de.haw.run.NetworkAdapter.Messages.NetworkMessage;

import java.util.UUID;

/**
 * Created with IntelliJ IDEA.
 * User: Chris
 * Date: 09.09.13
 * Time: 19:03
 * To change this template use File | Settings | File Templates.
 */
public class InitializeLayerFromModelNACKMessage extends NetworkMessage {
    public InitializeLayerFromModelNACKMessage(int clientId, UUID layerID) {
        super(clientId);
    }
}
