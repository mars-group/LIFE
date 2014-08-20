﻿//
//  OneClient.Host.cs
//
//  Author:
//       Enguerran Boissier <enguerran.boissier@terradue.com>
//
//  Copyright (c) 2014 Terradue

using System;
using CookComputing.XmlRpc;

namespace Terradue.OpenNebula {
    /// <summary>
    /// DotNet4One Client calling XML-RPC requests exposed by an OpenNebula server.
    /// XML-RPC commands for Host.
    /// </summary>
    public partial class OneClient {

        /// <summary>
        /// Enables the host.
        /// </summary>
        /// <returns><c>true</c>, if host was enabled, <c>false</c> otherwise.</returns>
        /// <param name="hostId">Host identifier.</param>
        /// <param name="enable">If set to <c>true</c> enable.</param>
        public bool HostEnable(int hostId, bool enable){
            bool result = false;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostEnable(this.SessionSHA, hostId, enable);
            result = (bool)openNebulaReturnArr.GetValue(0);
            return result;
        }

        /// <summary>
        /// Updates the host.
        /// </summary>
        /// <returns><c>true</c>, if host was updated, <c>false</c> otherwise.</returns>
        /// <param name="hostId">Host identifier.</param>
        /// <param name="template">Template.</param>
        /// <param name="type">Type.</param>
        public bool HostUpdate(int hostId, string template, int type){
            bool result = false;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostUpdate(this.SessionSHA, hostId, template, type);
            result = (bool)openNebulaReturnArr.GetValue(0);
            return result;
        }

        /// <summary>
        /// Allocates the host.
        /// </summary>
        /// <returns>The host.</returns>
        /// <param name="hostname">Hostname.</param>
        /// <param name="infoManagerName">Info manager name.</param>
        /// <param name="vmManagerName">Vm manager name.</param>
        /// <param name="vnManagerName">Vn manager name.</param>
        /// <param name="clusterId">Cluster identifier.</param>
        public int HostAllocate(string hostname, string infoManagerName, string vmManagerName, string vnManagerName, int clusterId){
            int result = 0;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostAllocate(this.SessionSHA, hostname, infoManagerName, vmManagerName, vnManagerName, clusterId);
            result = (int)openNebulaReturnArr.GetValue(1);
            return result;
        }

        /// <summary>
        /// Deletes the host.
        /// </summary>
        /// <returns><c>true</c>, if host was deleted, <c>false</c> otherwise.</returns>
        /// <param name="hostId">Host identifier.</param>
        public bool HostDelete(int hostId){
            bool result = false;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostDelete(this.SessionSHA, hostId);
            result = (bool)openNebulaReturnArr.GetValue(0);
            return result;
        }
       
        /// <summary>
        /// Renames the host.
        /// </summary>
        /// <returns><c>true</c>, if host was renamed, <c>false</c> otherwise.</returns>
        /// <param name="hostId">Host identifier.</param>
        /// <param name="newName">New name.</param>
        public bool HostRename(int hostId, string newName){
            bool result = false;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostRename(this.SessionSHA, hostId, newName);
            result = (bool)openNebulaReturnArr.GetValue(0);
            return result;
        }

        /// <summary>
        /// Gets the host info.
        /// </summary>
        /// <returns>The host info.</returns>
        /// <param name="hostId">Host identifier.</param>
        public HOST HostGetInfo(int hostId){
            HOST result = null;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostInfo(this.SessionSHA, hostId);
            result = (HOST)Deserialize(typeof(HOST), openNebulaReturnArr.GetValue(1).ToString());
            return result;
        }

        /// <summary>
        /// Gets the host list info.
        /// </summary>
        /// <returns>The host list info.</returns>
        public HOST_POOL HostGetPoolInfo(){
            HOST_POOL result = null;
            XmlRpcHostManagement xrum = (XmlRpcHostManagement)GetProxy(typeof(XmlRpcHostManagement));
            Array openNebulaReturnArr = xrum.oneHostPoolInfo(this.SessionSHA);
            result = (HOST_POOL)Deserialize(typeof(HOST_POOL), openNebulaReturnArr.GetValue(1).ToString());
            return result;
        }

    }
}

