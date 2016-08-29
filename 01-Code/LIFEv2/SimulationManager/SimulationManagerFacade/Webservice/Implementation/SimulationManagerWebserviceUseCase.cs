//  /*******************************************************
//  * Copyright (C) Christian Hüning - All Rights Reserved
//  * Unauthorized copying of this file, via any medium is strictly prohibited
//  * Proprietary and confidential
//  * This file is part of the MARS LIFE project, which is part of the MARS System
//  * More information under: http://www.mars-group.org
//  * Written by Christian Hüning <christianhuening@gmail.com>, 30.12.2015
//  *******************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using SimulationManagerFacade.Interface;
using SimulationManagerWebservice;
using SMConnector.TransportTypes;
using System.Threading.Tasks;
using Microsoft.Net.Http.Server;
using Newtonsoft.Json;

namespace SimulationManagerFacade.Webservice.Implementation
{


	/// <summary>
	///   A REST-style webserver that receives the client's key-value queries. 
	/// </summary>
	public class SimulationManagerWebserviceUseCase : ISimulationManagerWebservice {


		/// <summary>
		///   Create a new webserver.
		/// </summary>
		/// <param name="storage">The storage solution to use (CSV or ETCD).</param>
		/// <param name="address">The address and port number to listen on.</param>
		public SimulationManagerWebserviceUseCase(ISimulationManagerApplicationCore simManager) {
			var listener = new WebListener();
			listener.UrlPrefixes.Add("http://*:1234/");
			listener.Start();

			/* HTTP listener loop to handle incoming requests. */
			Task.Run (() => {
				while (true) {
					var context = listener.GetContextAsync().Result;
				    var uri = context.Request.Path;//context.Request.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
					var qs = context.Request.QueryString;
					switch (context.Request.Method) {
						// Performs READ operations.
					case "GET":
						Console.WriteLine ("Received a GET request");
						WriteResponse(context.Response, new byte[0]);
						break;


						// Performs control operations
						case "POST":
							// POST /stop 
							if(uri.Equals("stop")){
								var content = new StreamReader(context.Request.Body).ReadToEnd();
								var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
								WriteResponse(context.Response, new byte[0]);
							    simManager.AbortSimulation(new TModelDescription(dict["model"]));
							}
							// POST /step 
							if(uri.Equals("step")){
								var content = new StreamReader(context.Request.Body).ReadToEnd();
								var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
								WriteResponse(context.Response, new byte[0]);
								simManager.StepSimulation (new TModelDescription(dict["model"]));
							}
							// POST /pause
							if(uri.Equals("pause")){
								var content = new StreamReader(context.Request.Body).ReadToEnd();
								var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
								WriteResponse(context.Response, new byte[0]);
								simManager.PauseSimulation(new TModelDescription(dict["model"]));
							}
							// POST /step 
							if(uri.Equals("resume")){
								var content = new StreamReader(context.Request.Body).ReadToEnd();
								var dict = JsonConvert.DeserializeObject<Dictionary<string,string>>(content);
								WriteResponse(context.Response, new byte[0]);
								simManager.ResumeSimulation(new TModelDescription(dict["model"]));
							}
							break;
					}
				}
			}
			);
			// ReSharper disable once FunctionNeverReturns
		}


		/// <summary>
		///   Write data into response and return 200 OK code.
		/// </summary>
		/// <param name="response">Listener response object.</param>
		/// <param name="output">The response's payload.</param>
		private static void WriteResponse(Response response, byte[] output) {
			response.StatusCode = 200;
			response.ContentLength = output.Length;
			response.Body.Write(output, 0, output.Length);
		}


		/// <summary>
		///   Returns a 404 error message.
		/// </summary>
		/// <param name="response">Listener response object.</param>
		/// <param name="description">Error description text.</param>
		private static void Write404(Response response, string description) {
			response.StatusCode = 404;
		}
			
	}
}


