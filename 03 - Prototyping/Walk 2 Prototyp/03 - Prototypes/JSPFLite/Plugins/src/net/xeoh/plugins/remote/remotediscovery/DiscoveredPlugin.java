/*
 * DiscoveredPlugin.java
 * 
 * Copyright (c) 2009, Ralf Biedert All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 * Redistributions of source code must retain the above copyright notice, this list of
 * conditions and the following disclaimer. Redistributions in binary form must reproduce the
 * above copyright notice, this list of conditions and the following disclaimer in the
 * documentation and/or other materials provided with the distribution.
 * 
 * Neither the name of the author nor the names of its contributors may be used to endorse or
 * promote products derived from this software without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
 * OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
 * COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
 * EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR
 * TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 */
package net.xeoh.plugins.remote.remotediscovery;

import net.xeoh.plugins.remote.PublishMethod;

import java.net.URI;
import java.util.List;

/**
 * Reflects one discovered plugin.
 * 
 * @author Ralf Biedert
 *
 */
public interface DiscoveredPlugin {

    /**
     * Where the plugin can be found.
     * 
     * @return .
     */
    public URI getPublishURI();

    /**
     * Returns the relative distance of the exported plugin. The lower the number, the closer.
     * 
     * @return .
     */
    public int getDistance();

    /**
     * The method the plugins was published.
     * 
     * @return .
     */
    public PublishMethod getPublishMethod();

    /**
     * Returns the capabilities.
     *  
     * @return .
     */
    public List<String> getCapabilities();

    /**
     * Returns the time since the export of the pluign in milliseconds.
     * 
     * @return .
     */
    public long getTimeSinceExport();
}
