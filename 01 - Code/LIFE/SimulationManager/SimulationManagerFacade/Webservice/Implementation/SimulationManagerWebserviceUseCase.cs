﻿using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using System.Text;
using System.Web.Script.Serialization;
using System.Collections.Generic;
using SimulationManagerFacade.Interface;


namespace SimulationManagerWebservice
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
			var listener = new HttpListener();
			listener.Prefixes.Add("http://*:1234/");
			listener.Start();

			// create a JSON Deserializer
			var jss = new JavaScriptSerializer ();

			/* HTTP listener loop to handle incoming requests. */
			while (true) {
				var context = listener.GetContext();
				var uri = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
				var qs = context.Request.QueryString;
				switch (context.Request.HttpMethod) {
					// Performs READ operations.
				case "GET":
					Console.WriteLine ("Received a GET request");
					WriteResponse(context.Response, new byte[0]);
						break;


						// Performs CREATE operations.
					case "POST":
						// POST /stop 
						if(uri.Equals("stop")){
							var content = new StreamReader(context.Request.InputStream).ReadToEnd();
							var dict = jss.Deserialize<Dictionary<string,string>>(content);
							WriteResponse(context.Response, new byte[0]);
						simManager.AbortSimulation(dict["model"]);
						}
						// POST /step 
						if(uri.Equals("step")){
							var content = new StreamReader(context.Request.InputStream).ReadToEnd();
							var dict = jss.Deserialize<Dictionary<string,string>>(content);
							WriteResponse(context.Response, new byte[0]);
							simManager.ResumeSimulation (dict["model"]);
						}
						// POST /pause
						if(uri.Equals("pause")){
							var content = new StreamReader(context.Request.InputStream).ReadToEnd();
							var dict = jss.Deserialize<Dictionary<string,string>>(content);
							WriteResponse(context.Response, new byte[0]);
							simManager.PauseSimulation(dict["model"]);
						}

						context.Response.Close();
						break;


						// Performs DELETE operations.
					case "DELETE":
						context.Response.Close();
						break;
				}
			}
			// ReSharper disable once FunctionNeverReturns
		}


		/// <summary>
		///   Write data into response and return 200 OK code.
		/// </summary>
		/// <param name="response">Listener response object.</param>
		/// <param name="output">The response's payload.</param>
		private static void WriteResponse(HttpListenerResponse response, byte[] output) {
			response.StatusCode = 200;
			response.StatusDescription = "OK";
			response.ContentLength64 = output.Length;
			response.OutputStream.Write(output, 0, output.Length);
			response.Close();
		}


		/// <summary>
		///   Returns a 404 error message.
		/// </summary>
		/// <param name="response">Listener response object.</param>
		/// <param name="description">Error description text.</param>
		private static void Write404(HttpListenerResponse response, string description) {
			response.StatusCode = 404;
			response.StatusDescription = description;
			response.Close();
		}
			
	}
}


