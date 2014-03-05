/*
 * ExportInfo.java
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
package net.xeoh.plugins.remote.remotediscovery.impl.common.discoverymanager;

import java.io.IOException;
import java.io.ObjectStreamException;
import java.io.Serializable;
import java.util.Collection;

/**
 * 
 * @author Ralf Biedert
 */
public class ExportInfo implements Serializable {
    /** */
    private static final long serialVersionUID = -1167139290124669982L;

    /** */
    private int version = 100;

    /** true if the plugin was exported there */
    public boolean isExported;

    /** */
    public Collection<ExportedPlugin> allExported;

    /**
     * @param out
     * @throws java.io.IOException
     */
    private void writeObject(java.io.ObjectOutputStream out) throws IOException {
        out.writeInt(this.version);
        out.writeBoolean(this.isExported);
        out.writeObject(this.allExported);
    }

    /**
     * @param in
     * @throws java.io.IOException
     * @throws ClassNotFoundException
     */
    @SuppressWarnings("unchecked")
    private void readObject(java.io.ObjectInputStream in) throws IOException,
                                                         ClassNotFoundException {
        this.version = in.readInt();
        this.isExported = in.readBoolean();
        this.allExported = (Collection<ExportedPlugin>) in.readObject();
    }

    /**
     * @throws java.io.ObjectStreamException
     */
    @SuppressWarnings("unused")
    private void readObjectNoData() throws ObjectStreamException {
        // What to do here?
    }
}
