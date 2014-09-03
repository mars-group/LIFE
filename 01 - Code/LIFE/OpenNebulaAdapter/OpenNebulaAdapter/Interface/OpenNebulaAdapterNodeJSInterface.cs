using System;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Interface
{
    public class OpenNebulaAdapterNodeJSInterface
    {

        public static void Main(String[] args) {
            // First create the client
            const string proxyUrl = "http://141.22.29.2:2633/RPC2";
            const string adminUser = "serveradmin"; //should be user with driver server_* to allow requests delegation
            const string adminPwd = "80051ee6a7b403ae88cb1fa8e5d9d0877eddfbc0"; //SHA1 password
            
            var one = new OneClient(proxyUrl, adminUser, adminPwd);
            
            // Do a request as admin
            USER_POOL pool = one.UserGetPoolInfo();
            var templateInfo = one.TemplateGetInfo(15);

            
            // Do a request on behalf of a normal user
            string targetUser = "christian";
            one.StartDelegate(targetUser);
            
            int RemoteId = one.TemplateInstanciateVM(15, "TestVM", false, "");
            one.EndDelegate();
        }
    }
}
