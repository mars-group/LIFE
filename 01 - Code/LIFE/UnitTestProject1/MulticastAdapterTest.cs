using System;
using AppSettingsManager;
using ConfigurationAdapter.Interface;
using MulticastAdapter.Implementation;
using MulticastAdapter.Interface.Config;
using MulticastAdapter.Interface.Config.Types;
using NUnit.Framework;


namespace MulticastAdapterTestProject
{

    public class MulticastAdapterTest
    {


        [Test]
        public void TestPortIncrement() {
            var startListenPort = 50555;

            var globalConfig =new GlobalConfig("255.7.77.7", 60543, startListenPort, 4);

            var senderConfig = new MulticastSenderConfig();
            
            //test if both adapter  can handle the same starting port
            var adapter1 = new MulticastAdapterComponent(globalConfig, senderConfig);
            var adapter2 = new MulticastAdapterComponent(globalConfig, senderConfig);

            // if no exceptions are thrown we are good to go
            Assert.True(true);

        }



    }
}
