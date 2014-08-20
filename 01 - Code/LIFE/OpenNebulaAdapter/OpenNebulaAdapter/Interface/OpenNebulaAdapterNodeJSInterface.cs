using System;
using Terradue.OpenNebula;

namespace OpenNebulaAdapter.Interface
{
    public class OpenNebulaAdapterNodeJSInterface
    {

        public static void main(String[] args) {
            // First create the client
            string proxyUrl = "<YOUR_SERVER_URL>";
            string adminUser = "<YOUR_ADMIN_USERNAME>"; //should be user with driver server_* to allow requests delegation
            string adminPwd = "<YOUR_ADMIN_PASSWORD>"; //SHA1 password
            var one = new OneClient(proxyUrl, adminUser, adminPwd);
            
            // Do a request as admin
            USER_POOL pool = one.UserGetPoolInfo();

            // Do a request on behalf of a normal user
            string targetUser = "<YOUR_TARGET_USERNAME>";
            one.StartDelegate(targetUser);
            //int RemoteId = one.TemplateInstanciateVM(idTemplate, vmName, false, "");
            one.EndDelegate();
        }
    }
}
