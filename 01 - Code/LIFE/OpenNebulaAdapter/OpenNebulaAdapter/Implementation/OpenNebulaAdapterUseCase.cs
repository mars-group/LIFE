using System;
using OpenNebulaAdapter.Entities;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Implementation
{
    public class OpenNebulaAdapterUseCase
    {
        private OneClient _one;

        public OpenNebulaAdapterUseCase() {
                        // First create the client
            const string proxyUrl = "http://141.22.29.2:2633/RPC2";
            const string adminUser = "serveradmin"; //should be user with driver server_* to allow requests delegation
            const string adminPwd = "80051ee6a7b403ae88cb1fa8e5d9d0877eddfbc0"; //SHA1 password
            
            _one = new OneClient(proxyUrl, adminUser, adminPwd);
        }

        public bool CreateVMsFromNodeConfig(NodeConfig nodeConfig) {

            try {
                foreach (var node in nodeConfig.Nodes) {

                    var template = "NAME = \"" + node.NodeName + " Template\"\n"
                                   + "CONTEXT=[NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n"
                                   + "CPU=\"" + node.CpuCount + "\"\n"
                                   + "DISK=[\n"
                                   + "\tDRIVER=\"qcow2\",\n"
                                   + "\tIMAGE=\"UbuntuServer14\",\n"
                                   + "\tIMAGE_UNAME=\"christian\"]\n"
                                   + "DISK=[\n"
                                   + "\tSIZE=\"4096\",\n"
                                   + "\tTYPE=\"swap\"]\n"
                                   + "GRAPHICS=[\n"
                                   + "\tKEYMAP=\"de\",\n"
                                   + "\tLISTEN=\"0.0.0.0\",\n"
                                   + "\tTYPE=\"VNC\"]\n"
                                   + "MEMORY=\"" + (node.RamAmount * 1024) + "\"\n"
                                   + "NIC=[MODEL=\"virtio\",NETWORK=\"MARS SimulationNetwork\",NETWORK_UNAME=\"christian\"]\n"
                                   + "OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n"
                                   + "VCPU=\"" + node.CpuCount + "\"";

                    var templateID = _one.TemplateAllocate(template);

                    int RemoteId = _one.TemplateInstanciateVM(templateID, node.NodeName, false, "");

                    _one.TemplateDelete(templateID);
                }
            }
            catch (Exception ex) {
                return false;
            }

            return true;
        }

    }
}
