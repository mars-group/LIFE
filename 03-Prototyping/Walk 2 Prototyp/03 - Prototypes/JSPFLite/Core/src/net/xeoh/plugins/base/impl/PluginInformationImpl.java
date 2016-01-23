/*
 * PluginConfigurationImpl.java
 *
 * Copyright (c) 2007, Ralf Biedert All rights reserved.
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
package net.xeoh.plugins.base.impl;

import static net.jcores.jre.CoreKeeper.$;

import java.lang.reflect.InvocationTargetException;
import java.lang.reflect.Method;
import java.util.ArrayList;
import java.util.Collection;
import java.util.logging.Logger;

import net.xeoh.plugins.base.Plugin;
import net.xeoh.plugins.base.PluginInformation;
import net.xeoh.plugins.base.PluginManager;
import net.xeoh.plugins.base.annotations.Capabilities;
import net.xeoh.plugins.base.annotations.PluginImplementation;
import net.xeoh.plugins.base.annotations.injections.InjectPlugin;
import net.xeoh.plugins.base.annotations.meta.Author;
import net.xeoh.plugins.base.annotations.meta.Stateless;
import net.xeoh.plugins.base.annotations.meta.Version;
import net.xeoh.plugins.base.impl.registry.PluginMetaInformation;
import net.xeoh.plugins.base.impl.registry.PluginRegistry;
import net.xeoh.plugins.base.options.GetInformationOption;

/**
 * TODO: Make plugin threadsafe
 * 
 * @author Ralf Biedert
 * 
 */
@Author(name = "Ralf Biedert")
@Version(version = 1 * Version.UNIT_MAJOR)
@Stateless
@PluginImplementation
public class PluginInformationImpl implements PluginInformation {
    /** */
    final Logger logger = Logger.getLogger(this.getClass().getName());

    /**  */
    @InjectPlugin
    public PluginManager pluginManager;

    /** Dum dum dum dum .... don't touch this ... */
    private PluginInformationImpl() { }
    
    /*
     * (non-Javadoc)
     * 
     * @see net.xeoh.plugins.base.PluginInformation#getInformation(net.xeoh.plugins.base.
     * PluginInformation.Information, net.xeoh.plugins.base.Plugin)
     */
    public Collection<String> getInformation(final Information item, final Plugin plugin) {

        // Needed to query some special information
        final PluginManagerImpl pmi = (PluginManagerImpl) this.pluginManager;

        // Prepare return values ...
        final Collection<String> rval = new ArrayList<String>();

        switch (item) {

        case CAPABILITIES:
            // Caps are only supported for plugins currently
            final String[] caps = getCaps(plugin);
            for (final String string : caps) {
                rval.add(string);
            }
            break;

        case AUTHORS:
            final Author author = plugin.getClass().getAnnotation(Author.class);
            if (author == null) break;
            rval.add(author.name());
            break;

        case VERSION:
            final Version version = plugin.getClass().getAnnotation(Version.class);
            if (version == null) break;
            rval.add(Integer.toString(version.version()));
            
            if(plugin == this.pluginManager) {
                final String build = $(PluginManagerImpl.class.getResourceAsStream("jspf.version")).text().split("\n").hashmap().get("build");
                rval.add("jspf.build:" + build);
            }
            break;

        case CLASSPATH_ORIGIN:
            final PluginRegistry pluginRegistry = pmi.getPluginRegistry();
            final PluginMetaInformation metaInformation = pluginRegistry.getMetaInformationFor(plugin);
            if (metaInformation != null && metaInformation.classMeta != null && metaInformation.classMeta.pluginOrigin != null)
                    rval.add(metaInformation.classMeta.pluginOrigin.toString());
            break;

        default:
            this.logger.info("Requested InformationItem is not known!");
            break;
        }

        return rval;
    }

    /**
     * @param plugin
     * @return
     */
    private String[] getCaps(final Plugin plugin) {
        final Class<? extends Plugin> spawnClass = plugin.getClass();

        final Method[] methods = spawnClass.getMethods();

        // Search for proper method
        for (final Method method : methods) {

            // Init methods will be marked by the corresponding annotation.
            final Capabilities caps = method.getAnnotation(Capabilities.class);
            if (caps != null) {

                Object result = null;
                try {
                    result = method.invoke(plugin, new Object[0]);
                } catch (final IllegalArgumentException e) {
                    //
                } catch (final IllegalAccessException e) {
                    //
                } catch (final InvocationTargetException e) {
                    //
                }
                if (result != null && result instanceof String[])
                    return (String[]) result;
            }
        }

        return new String[0];
    }

    /* (non-Javadoc)
     * @see net.xeoh.plugins.base.PluginInformation#getInformation(net.xeoh.plugins.base.Plugin, java.lang.Class)
     */
    @Override
    public <T extends GetInformationOption> T getInformation(Plugin plugin, Class<T> query) {
        return null;
    }
}
