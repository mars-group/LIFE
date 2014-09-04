using System;
using OpenNebulaAdapter.Entities;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Implementation
{
    public class OpenNebulaAdapterUseCase
    {
        public OpenNebulaAdapterUseCase() {
                        // First create the client
            const string proxyUrl = "http://141.22.29.2:2633/RPC2";
            const string adminUser = "serveradmin"; //should be user with driver server_* to allow requests delegation
            const string adminPwd = "80051ee6a7b403ae88cb1fa8e5d9d0877eddfbc0"; //SHA1 password
            
            var one = new OneClient(proxyUrl, adminUser, adminPwd);
            
            // Do a request as admin
            USER_POOL pool = one.UserGetPoolInfo();

            
            // Do a request on behalf of a normal user
            string targetUser = "christian";

            var template = "NAME = vmTest\n" +
                           "CONTEXT=[NETWORK=\"YES\",SSH_PUBLIC_KEY=\"$USER[SSH_PUBLIC_KEY]\"]\n"
                           + "CPU=\"2\"\n"
                           + "DISK=[\n"
                           + "\tDRIVER=\"qcow2\",\n"
                           + "\tIMAGE=\"UbuntuServer14\",\n" 
                           + "\tIMAGE_UNAME=\"christian\"]\n"
                           + "DISK=[\n" 
                           + "\tSIZE=\"10240\",\n"
                           + "\tTYPE=\"swap\"]\n"
                           + "GRAPHICS=[\n"
                           + "\tKEYMAP=\"de\",\n"
                           + "\tLISTEN=\"0.0.0.0\",\n"
                           + "\tTYPE=\"VNC\"]\n"
                           + "MEMORY=\"8192\"\n"
                           + "NIC=[MODEL=\"virtio\",NETWORK=\"NewAwesomeNetworkWithoutIsolation\",NETWORK_UNAME=\"christian\"]\n"
                           + "OS=[ARCH=\"x86_64\",BOOT=\"hd\"]\n"
                           + "VCPU=\"2\"";


            one.StartDelegate(targetUser);

            var templateID = one.TemplateAllocate(template);
            //one.TemplateInstanciateVM(templateID, )


           // int RemoteId = one.TemplateInstanciateVM(15, "TestVM", false, "");
            one.EndDelegate();
        }

        public bool createVMsFromNodeConfig(NodeConfig nodeConfig) {
            
        }

    }
}
