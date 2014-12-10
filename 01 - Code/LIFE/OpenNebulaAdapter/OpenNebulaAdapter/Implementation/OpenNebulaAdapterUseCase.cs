// /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 06.11.2014
//  *******************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;
using OpenNebulaAdapter.Entities;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Implementation {
    public class OpenNebulaAdapterUseCase {
        private const int MARS_NETWORK_ID = 21;
        private readonly OneClient _one;

        private IDictionary<string, VMInfo[]> _vmDictionary;

        public OpenNebulaAdapterUseCase() {
            // First create the client
            const string proxyUrl = "http://141.22.29.2:2633/RPC2";
            const string adminUser = "serveradmin"; //should be user with driver server_* to allow requests delegation
            const string adminPwd = "80051ee6a7b403ae88cb1fa8e5d9d0877eddfbc0"; //SHA1 password

            _one = new OneClient(proxyUrl, adminUser, adminPwd);
            _vmDictionary = new Dictionary<string, VMInfo[]>();
        }

        public IDictionary<string, VMInfo[]> CreateVMsFromNodeConfig(NodeConfig nodeConfig) {
            // re-initialize VM Dictionary, since each run requires a new one
            _vmDictionary = new Dictionary<string, VMInfo[]>();

            // first create one SimulationManager
            int simManagerVMID = -1;
            IPAddress simManagerVMIp = null;
            const int simManagerVMPort = 44521;
            try {
                StringBuilder simManagerTemplate = new StringBuilder();
                simManagerTemplate.Append("NAME = \"Temporary SimulationManager Template\"\n");
                simManagerTemplate.Append
                    ("CONTEXT=[FILES_DS=\"$FILE[IMAGE_ID=50]\",HOSTNAME=\"$NAME\",NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n");
                simManagerTemplate.Append("CPU=\"0.01\"\n");
                simManagerTemplate.Append("DISK=[\n");
                simManagerTemplate.Append("\tDRIVER=\"qcow2\",\n");
                simManagerTemplate.Append("\tIMAGE=\"MARS SimulationManager\",\n");
                simManagerTemplate.Append("\tIMAGE_UNAME=\"christian\"]\n");
                simManagerTemplate.Append("DISK=[\n");
                simManagerTemplate.Append("\tSIZE=\"4096\",\n");
                simManagerTemplate.Append("\tTYPE=\"swap\"]\n");
                simManagerTemplate.Append("GRAPHICS=[\n");
                simManagerTemplate.Append("\tKEYMAP=\"de\",\n");
                simManagerTemplate.Append("\tLISTEN=\"0.0.0.0\",\n");
                simManagerTemplate.Append("\tTYPE=\"VNC\"]\n");
                simManagerTemplate.Append("MEMORY=\"1024\"\n");
                simManagerTemplate.Append
                    ("NIC=[MODEL=\"virtio\",NETWORK_ID=\"" + MARS_NETWORK_ID + "\",NETWORK_UNAME=\"christian\"]\n");
                simManagerTemplate.Append("OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n");
                simManagerTemplate.Append("VCPU=\"4\"");

                int simManagerTemplateID = _one.TemplateAllocate(simManagerTemplate.ToString());
                simManagerVMID = _one.TemplateInstanciateVM(simManagerTemplateID, "SimulationManager", false, "");
                _vmDictionary.Add
                    ("SimulationManager", new VMInfo[] {new VMInfo {id = simManagerVMID, vmStatus = "PENDING"}});

                _one.TemplateDelete(simManagerTemplateID);
            }
            catch (Exception ex) {
                // if anyone of these somehow fails, delete simManagerVM and return falsy
                if (simManagerVMID > -1) _one.VMAction(simManagerVMID, "delete");
                throw;
            }

            // TODO: WAIT until SimManager got IP!

            // now create our virtual router and use the SimManagers IP in the forwarding table

            // first retrieve SimManagers IP:
            XmlNode[] template = (XmlNode[]) _one.VMGetInfo(simManagerVMID).TEMPLATE;
            XmlNodeList vmDetails = template.First(entry => entry.Name == "NIC").ChildNodes;
            IEnumerator ienum = vmDetails.GetEnumerator();

            while (ienum.MoveNext()) {
                XmlNode curr = (XmlNode) ienum.Current;
                if (curr.Name == "IP") simManagerVMIp = IPAddress.Parse(curr.InnerText);
            }

            if (simManagerVMIp == null) throw new SimulationManagerGotNoIPAddressException();

            // create Virtual Router

            int vrID = -1;
            try {
                StringBuilder vrRouterTemplate = new StringBuilder();
                vrRouterTemplate.Append("NAME = \"Temporary MARS VirtualRouter Template\"\n");
                vrRouterTemplate.Append
                    ("CONTEXT=[DHCP=\"NO\",DNS=\"141.22.192.100 141.22.29.101\",FORWARDING=\"" + simManagerVMIp + ":"
                     + simManagerVMPort
                     + "\",NETWORK=\"YES\",NTP_SERVER=\"141.22.192.100\",PRIVNET=\"$NETWORK[TEMPLATE, NETWORK_ID=\\\"21\\\"]\",PUBNET=\"$NETWORK[TEMPLATE, NETWORK_ID=\\\"12\\\"]\",RADVD=\"NO\",ROOT_PASSWORD=\"JDEkYnVUV1dvT0gkenRoWDlWRTNyVWM5MHBqL0hsLktIMAo=\",SEARCH=\"local.domain\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\",TARGET=\"hdb\",TEMPLATE=\"$TEMPLATE\"]\n");
                vrRouterTemplate.Append("CPU=\"0.01\"\n");
                vrRouterTemplate.Append
                    ("DISK=[CACHE=\"none\",DEV_PREFIX=\"vd\",DRIVER=\"raw\",IMAGE=\"OpenNebula 4.8 Virtual Router\",IMAGE_UNAME=\"christian\"]\n");
                vrRouterTemplate.Append("DISK=[FORMAT=\"swap\",SIZE=\"4096\",TYPE=\"swap\"]\n");
                vrRouterTemplate.Append("FEATURES=[ACPI=\"yes\",APIC=\"yes\",LOCALTIME=\"yes\",PAE=\"yes\"]\n");
                vrRouterTemplate.Append("GRAPHICS=[LISTEN=\"0.0.0.0\",TYPE=\"VNC\"]\n");
                vrRouterTemplate.Append("LOGO=\"images/logos/linux.png\"\n");
                vrRouterTemplate.Append("MEMORY=\"512\"\n");
                vrRouterTemplate.Append
                    ("NIC=[IP=\"10.10.0.1\",MODEL=\"virtio\",NETWORK_ID=\"21\",NETWORK_UNAME=\"christian\"]\n");
                vrRouterTemplate.Append
                    ("NIC=[IP=\"141.22.29.106\",MODEL=\"virtio\",NETWORK_ID=\"12\",NETWORK_UNAME=\"christian\"]\n");
                vrRouterTemplate.Append("OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n");
                vrRouterTemplate.Append("VCPU=\"2\"\n");

                vrID = _one.TemplateAllocate(vrRouterTemplate.ToString());
                VMInfo vrDict = new VMInfo {
                    id = _one.TemplateInstanciateVM(vrID, "Virtual Router", false, ""),
                    vmStatus = "PENDING"
                };
                _vmDictionary.Add("VirtualRouter", new VMInfo[] {vrDict});
                _one.TemplateDelete(vrID);
            }
            catch (Exception ex) {
                if (vrID > -1) _one.TemplateDelete(vrID);
                throw;
            }


            // now create all the nodes

            try {
                List<VMInfo> vmAry = new List<VMInfo>();

                foreach (Node node in nodeConfig.Nodes) {
                    StringBuilder stb = new StringBuilder();
                    stb.Append("NAME = \"Temporary " + node.NodeName + " Template\"\n");
                    stb.Append
                        ("CONTEXT=[FILES_DS=\"$FILE[IMAGE_ID=50]\",HOSTNAME=\"$NAME\",NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n");
                    stb.Append("CPU=\"0.01\"\n");
                    stb.Append("DISK=[\n");
                    stb.Append("\tDRIVER=\"qcow2\",\n");
                    stb.Append("\tIMAGE=\"MARS LayerContainer\",\n");
                    stb.Append("\tIMAGE_UNAME=\"christian\"]\n");
                    stb.Append("DISK=[\n");
                    stb.Append("\tSIZE=\"4096\",\n");
                    stb.Append("\tTYPE=\"swap\"]\n");
                    stb.Append("GRAPHICS=[\n");
                    stb.Append("\tKEYMAP=\"de\",\n");
                    stb.Append("\tLISTEN=\"0.0.0.0\",\n");
                    stb.Append("\tTYPE=\"VNC\"]\n");
                    stb.Append("MEMORY=\"" + (node.RamAmount*1024) + "\"\n");
                    stb.Append("NIC=[MODEL=\"virtio\",NETWORK_ID=\"21\",NETWORK_UNAME=\"christian\"]\n");
                    stb.Append("OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n");
                    stb.Append("VCPU=\"" + node.CpuCount + "\"");

                    int templateID = _one.TemplateAllocate(stb.ToString());

                    vmAry.Add
                        (new VMInfo {
                            id = _one.TemplateInstanciateVM(templateID, node.NodeName, false, ""),
                            vmStatus = "PENDING"
                        });


                    _one.TemplateDelete(templateID);
                }

                _vmDictionary.Add("LayerContainer", vmAry.ToArray());
            }
            catch (Exception ex) {
                // if anyone of these somehow fails, delete all created vms and return falsy
                foreach (VMInfo remoteID in _vmDictionary.SelectMany(remoteIDentry => remoteIDentry.Value)) {
                    _one.VMAction(remoteID.id, "delete");
                }

                throw;
            }


            // all is well, return no error
            return _vmDictionary;
        }


        public void deleteVMs(int[] vmArray) {
            foreach (int vmId in vmArray) {
                _one.VMAction(vmId, "delete");
            }
        }

        public string GetVmInfo(int vmID) {
            LcmStates status;
            LcmStates.TryParse(_one.VMGetInfo(vmID).LCM_STATE, out status);
            return status.ToString();
        }
    }

    public class SimulationManagerGotNoIPAddressException : Exception {}

    internal enum LcmStates {
        LCM_INIT = 0,
        PROLOG = 1,
        BOOT = 2,
        RUNNING = 3,
        MIGRATE = 4,
        SAVE_STOP = 5,
        SAVE_SUSPEND = 6,
        SAVE_MIGRATE = 7,
        PROLOG_MIGRATE = 8,
        PROLOG_RESUME = 9,
        EPILOG_STOP = 10,
        EPILOG = 11,
        SHUTDOWN = 12,
        CANCEL = 13,
        FAILURE = 14,
        CLEANUP_RESUBMIT = 15,
        UNKNOWN = 16,
        HOTPLUG = 17,
        SHUTDOWN_POWEROFF = 18,
        BOOT_UNKNOWN = 19,
        BOOT_POWEROFF = 20,
        BOOT_SUSPENDED = 21,
        BOOT_STOPPED = 22,
        CLEANUP_DELETE = 23,
        HOTPLUG_SNAPSHOT = 24,
        HOTPLUG_NIC = 25,
        HOTPLUG_SAVEAS = 26,
        HOTPLUG_SAVEAS_POWEROFF = 27,
        HOTPLUG_SAVEAS_SUSPENDED = 28,
        SHUTDOWN_UNDEPLOY = 29,
        EPILOG_UNDEPLOY = 30,
        PROLOG_UNDEPLOY = 31,
        BOOT_UNDEPLOY = 32
    }
}