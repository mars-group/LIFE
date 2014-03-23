using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CommonTypes.DataTypes;
using CommonTypes.Types;
using MulticastAdapter.Interface;
using Newtonsoft.Json;


namespace NodeRegistry.Implementation
{
    public class NodeRegistryManager
    {
        private List<NodeInformationType> activeNodeList;
        private IMulticastReciever Reciever;



        public NodeRegistryManager()
        {
            if (Boolean.Parse(ConfigurationManager.AppSettings.Get("ReadNodesFromConfig")))
            {
               ReadNodesFromAppConfig();
            }
        }

        private void ReadNodesFromAppConfig()
        {
             String[] setupNodes = ConfigurationManager.AppSettings.GetValues("SetupNoes");

            foreach (var IP in setupNodes)
            {
              
            }


        }

        public List<NodeInformationType> GetActiveNodes()
        {
            return activeNodeList.Select(type => type).ToList();
        }


        


    }


   

}
