//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 23.01.2016
//  *******************************************************/
using ASC.Communication.Scs.Communication.EndPoints;
using ASC.Communication.Scs.Server;
using System.Collections.Concurrent;

namespace ASC.Communication.ScsServices.Service
{
	/// <summary>
	///     This class is used to build AscService applications.
	/// </summary>
	public static class AscServiceFactory {
        
		private static readonly ConcurrentDictionary<string, IAscServiceApplication> ServiceAppDictionary = new ConcurrentDictionary<string, IAscServiceApplication> ();

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
			return ServiceAppDictionary.GetOrAdd(typeOfService, serviceApp);
		}

        /// <summary>
		/// Gets a service application by type.
		/// </summary>
		/// <returns>The service application by type name if found, null otherwise.</returns>
		/// <param name="typeName">Type name.</param>
		public static IAscServiceApplication GetServiceApplicationByTypeName(string typeName)
		{
			IAscServiceApplication serviceApp;
			ServiceAppDictionary.TryGetValue(typeName, out serviceApp);
			return serviceApp;
		}
    }
}