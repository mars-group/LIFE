using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
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
			listener.Prefixes.Add("http://*:1234");
			listener.Start();

			/* HTTP listener loop to handle incoming requests. */
			while (true) {
				var context = listener.GetContext();
				var uri = context.Request.Url.GetComponents(UriComponents.Path, UriFormat.UriEscaped);
				var qs = context.Request.QueryString;
				switch (context.Request.HttpMethod) {
					// Performs READ operations.
					case "GET":
						if (uri.Equals("property")) {
							// Query a single value.
							WriteResponse(context.Response, Encoding.UTF8.GetBytes(property));
						}
						else if (uri.Equals("properties")) {
							// Query a directory.
							Console.WriteLine("[GET] Query on directory '" + qs["directory"] + "'.");
							var properties = storage.GetDirectory(qs["directory"]);
							var jsonString = JsonConvert.SerializeObject(properties);
							WriteResponse(context.Response, Encoding.UTF8.GetBytes(jsonString));
						}
						break;


						// Performs CREATE operations.
					case "POST":
						if(uri.Equals("stop")){
							simManager.AbortSimulation();
						}
						context.Response.Close();
						break;


						// Performs DELETE operations.
					case "DELETE":
						Console.WriteLine("[DELETE] Removal of entry '" + qs["key"] + "'.");
						storage.Delete(qs["key"]);
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

}


