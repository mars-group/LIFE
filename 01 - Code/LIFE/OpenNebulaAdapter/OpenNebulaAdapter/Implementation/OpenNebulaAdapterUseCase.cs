using System;
using System.Collections.Generic;
using System.Text;
using OpenNebulaAdapter.Entities;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Implementation
{
    public class OpenNebulaAdapterUseCase
    {
        private readonly OneClient _one;
        private readonly List<int> _remoteIDs;

        public OpenNebulaAdapterUseCase() {
                        // First create the client
            const string proxyUrl = "http://141.22.29.2:2633/RPC2";
            const string adminUser = "serveradmin"; //should be user with driver server_* to allow requests delegation
            const string adminPwd = "80051ee6a7b403ae88cb1fa8e5d9d0877eddfbc0"; //SHA1 password
            
            _one = new OneClient(proxyUrl, adminUser, adminPwd);
            _remoteIDs = new List<int>();
        }

        public int[] CreateVMsFromNodeConfig(NodeConfig nodeConfig) {

            try {
                foreach (var node in nodeConfig.Nodes) {
                    var stb = new StringBuilder();
                    stb.Append("NAME = \"Temporary " + node.NodeName + " Template\"\n");
                    stb.Append("CONTEXT=[FILES_DS=\"$FILE[IMAGE_ID=50]\",HOSTNAME=\"$NAME\",NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n");
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
                    stb.Append("MEMORY=\"" + (node.RamAmount * 1024) + "\"\n");
                    stb.Append("NIC=[MODEL=\"virtio\",NETWORK=\"MARS SimulationNetwork\",NETWORK_UNAME=\"christian\"]\n");
                    stb.Append("OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n");
                    stb.Append("VCPU=\"" + node.CpuCount + "\"");

                    var templateID = _one.TemplateAllocate(stb.ToString());

                    _remoteIDs.Add(_one.TemplateInstanciateVM(templateID, node.NodeName, false, ""));

                    _one.TemplateDelete(templateID);
                }
            }
            catch (Exception ex) {
                // if anyone of these somehow fails, delete all created vms and return falsy
                foreach (var remoteID in _remoteIDs) {
                    _one.VMAction(remoteID, "delete");
                }
                throw;
            }

            // and one SimulationManager
            int simManagerVMID = -1;
            try {
                var simManagerTemplate = new StringBuilder();
                simManagerTemplate.Append("NAME = \"Temporary SimulationManager Template\"\n");
                simManagerTemplate.Append("CONTEXT=[FILES_DS=\"$FILE[IMAGE_ID=50]\",HOSTNAME=\"$NAME\",NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n");
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
                simManagerTemplate.Append("NIC=[MODEL=\"virtio\",NETWORK=\"MARS SimulationNetwork\",NETWORK_UNAME=\"christian\"]\n");
                simManagerTemplate.Append("OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n");
                simManagerTemplate.Append("VCPU=\"4\"");

                var simManagerTemplateID = _one.TemplateAllocate(simManagerTemplate.ToString());

                simManagerVMID = _one.TemplateInstanciateVM(simManagerTemplateID, "SimulationManager", false, "");

                _one.TemplateDelete(simManagerTemplateID);
            }
            catch (Exception ex) {
                // if anyone of these somehow fails, delete simManagerVM and return falsy
                if (simManagerVMID > -1) { _one.VMAction(simManagerVMID, "delete"); }
                throw;
            }

            // all is well, return no error
            return _remoteIDs.ToArray();
        }

        public void deleteVMs(int[] vmArray) {
            foreach (var vmId in vmArray) {
                _one.VMAction(vmId, "delete");    
            }
        }

        public VM getVMInfo(int vmID) {

            return _one.VMGetInfo(vmID);

        }
    }
}
