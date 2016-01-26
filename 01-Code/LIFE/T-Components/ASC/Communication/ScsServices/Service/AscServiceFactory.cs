using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace ASC.Communication.ScsServices.Service {
    /// <summary>
    ///     This class is used to build AscService applications.
    /// </summary>
    public static class AscServiceFactory {
        
		private static readonly IDictionary<string, IAscServiceApplication> ServiceAppDictionary = new ConcurrentDictionary<string, IAscServiceApplication> ();

		/// <summary>
        /// Creates an ASC Service with the provided and multiCast Address and by using the default 
        /// </summary>
        /// <param name="port">The endpoint port to be used for the udp socket</param>
        /// <param name="multicastGroup">The mcastaddress to use for communication</param>
        /// <returns></returns>
		/// <param name = "typeOfService">The type of the service to host.</param>
        public static IAscServiceApplication CreateService(int port, string multicastGroup, string typeOfService)
		{
		    IAscServiceApplication serviceApp;
		    if (ServiceAppDictionary.TryGetValue(typeOfService, out serviceApp))
		    {
		        return serviceApp;
		    }
		    serviceApp = new AscServiceApplication(AcsServerFactory.CreateServer(AscEndPoint.CreateEndPoint(port, multicastGroup)));
		    ServiceAppDictionary.Add(typeOfService, serviceApp);
		    return serviceApp;
		}

        /// <summary>
		/// Gets a service application by type.
		/// </summary>
		/// <returns>The service application by type name if found, null otherwise.</returns>
		/// <param name="typeName">Type name.</param>
		public static IAscServiceApplication GetServiceApplicationByTypeName(string typeName)
		{
		    return ServiceAppDictionary.ContainsKey (typeName) ? ServiceAppDictionary [typeName] : null;
		}
    }
}