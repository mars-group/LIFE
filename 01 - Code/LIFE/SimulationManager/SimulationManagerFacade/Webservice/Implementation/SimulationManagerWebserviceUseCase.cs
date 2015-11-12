using System;
using System.Net;


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
		public SimulationManagerWebserviceUseCase() {
			var listener = new HttpListener();
			listener.Prefixes.Add("http://*:1234/");
			listener.Start();

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
							//simManager.AbortSimulation();
						}
						// POST /step 
						if(uri.Equals("step")){
							//simManager.ResumeSimulation();
						}
						// POST /pause
						if(uri.Equals("pause")){
							//simManager.PauseSimulation()
						}
						WriteResponse(context.Response, new byte[0]);
						context.Response.Close();
						break;


						// Performs DELETE operations.
					case "DELETE":
						Console.WriteLine("[DELETE] Removal of entry '" + qs["key"] + "'.");
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


