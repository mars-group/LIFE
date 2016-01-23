package net.xeoh.plugins.remote.remotediscovery.impl.common.discoverymanager.impl;

import net.xeoh.plugins.base.Plugin;
import net.xeoh.plugins.remote.PublishMethod;

import java.io.Serializable;
import java.net.URI;

/**
 * @author rb
 */
public class ExportEntry implements Serializable {
    /** */
    private static final long serialVersionUID = 953738903386891643L;

    /** */
    public Plugin plugin;

    /** */
    public PublishMethod method;

    /** */
    public URI uri;

    /** */
    public long timeOfExport;
}