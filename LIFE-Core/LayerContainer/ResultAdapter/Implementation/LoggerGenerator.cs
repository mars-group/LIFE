using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using LIFE.API.Agent;
using Newtonsoft.Json.Linq;

namespace ResultAdapter.Implementation {


  /// <summary>
  ///   Logger generation and distribution class.
  ///   It is initialized together with the ResultAdapter and called on each new registration.
  /// </summary>
  internal class LoggerGenerator {

    private readonly Dictionary<string, Type> _definitions; // List of compiled loggers.
    private readonly string _rcsHost;                       // ResultConfigService address.
    private readonly string _configId;                      // Configuration identifier.
    private readonly string _generatedCode;                 // The generated code file.


    /// <summary>
    ///   Create a new logger generator.
    /// </summary>
    /// <param name="rcsHost">ResultConfigService address.</param>
    /// <param name="configId">Configuration identifier.</param>
    public LoggerGenerator(string rcsHost, string configId) {
      _definitions = new Dictionary<string, Type>();
      _rcsHost = rcsHost;
      _configId = configId;
      var json = GetConfiguration();
      if (json != null) {
        Console.WriteLine("[LoggerGenerator] Retrieved configuration '"+configId+"' from "+rcsHost+".");
        var loggerDefs = ParseConfiguration(json);
        IList<string> reqDlls;
        var compiler = new LoggerCompiler("ResultAdapterTests/bin/Debug/out/");
        _generatedCode = compiler.GenerateLoggerCode(loggerDefs, out reqDlls);
        var dll = compiler.CompileSourceCode(_generatedCode, reqDlls);
        if (dll != null) {
          Console.WriteLine("[LoggerGenerator] Generated "+dll.GetExportedTypes().Length+" loggers:");
          foreach (var loggerType in dll.GetExportedTypes()) {
            Console.WriteLine(" - "+loggerType.Name);
            _definitions.Add(loggerType.Name, loggerType);
          }
        }
      }
    }


    /// <summary>
    ///   Retrieve the output configuration from the ResultConfigService.
    /// </summary>
    /// <returns>JSON object containing the result output configuration.</returns>
    private JObject GetConfiguration() {
      var http = new HttpClient();
      try {
        var getTask = http.GetAsync("http://"+_rcsHost+"/api/ResultConfigs/"+_configId);
        getTask.Wait(4000);
        if (getTask.Result.StatusCode == HttpStatusCode.OK) {
          var readTask = getTask.Result.Content.ReadAsStringAsync();
          readTask.Wait();
          http.Dispose();
          return JObject.Parse(readTask.Result);
        }
      }
      catch (Exception ex) {
        Console.Error.WriteLine("[LoggerGenerator] Failed to generate output loggers!");
        Console.Error.WriteLine(" - Output configuration ID: '"+_configId+"'");
        Console.Error.WriteLine(" - ResultConfigService host: '"+_rcsHost+"'");
        Console.Error.WriteLine(" - Exception: "+ex.GetType().Name+" => "+ex.InnerException.GetType().Name);
      }
      return null;
    }


    /// <summary>
    ///   Parse the output configuration and create the logger definitions.
    /// </summary>
    /// <param name="config">RCS output configuration (JSON).</param>
    /// <returns>A list of logger definitions to generate classes from.</returns>
    private static IEnumerable<LoggerConfig> ParseConfiguration(JObject config) {
      var loggers = new List<LoggerConfig>();
      foreach (var agentDef in config["Agents"]) {
        if ((bool) agentDef["OutputEnabled"]) {
          var fields = new Dictionary<string, bool>();
          foreach (var field in agentDef["OutputProperties"]) {
            if ((bool) field["Selected"]) {
              fields.Add(field["Name"].ToString(), (bool) field["Static"]);
            }
          }
          string spatialType = null;
          if ((bool) agentDef["SpatialOutput"]) {
            spatialType = agentDef["SpatialType"].ToString();
          }
          loggers.Add(new LoggerConfig {
            TypeName = agentDef["TypeName"].ToString(),
            FullName = agentDef["FullName"].ToString(),
            OutputFrequency = int.Parse(agentDef["Frequency"].ToString()),
            SpatialType = spatialType,
            IsStationary = agentDef["MovementType"].ToString().Equals("stationary"),
            Properties = fields,
            VisParameters = agentDef["VisualizationParams"].ToString().Split('\n')
          });
        }
      }
      return loggers;
    }


    /// <summary>
    ///   Get a result logger.
    /// </summary>
    /// <param name="simObject">The simulation object to set a logger for.</param>
    /// <returns>An agent result logger instance for the given type.</returns>
    public IGeneratedLogger GetResultLogger(ITickClient simObject) {
      var agentType = simObject.GetType().Name;
      if (_definitions.ContainsKey("ResultLogger_"+agentType)) {
        var instance = Activator.CreateInstance(_definitions[agentType], simObject);
        return (IGeneratedLogger) instance;
      }
      return null;
    }


    /// <summary>
    ///   Check, if a logger prototype for a given type exists.
    /// </summary>
    /// <param name="simObject">The simulation object to check for.</param>
    /// <returns>Boolean value, whether logger exists or not.</returns>
    public bool HasLoggerDefinition(ITickClient simObject) {
      return _definitions.ContainsKey("ResultLogger_"+simObject.GetType().Name);
    }


    /// <summary>
    ///   Output the logger generator's connection settings and available loggers.
    /// </summary>
    /// <returns>Formatted output string.</returns>
    public string ToString(bool codeOutput) {
      return "[LoggerGenerator] \n"+
             " - Config ID: "+_configId+"\n"+
             " - RCS address: "+_rcsHost+"\n"+
             " - Definitions: "+_definitions.Count+
             (codeOutput? "\n - Generated code:\n----------\n"+_generatedCode+"\n----------" : "");
    }


    /// <summary>
    ///   Save the generated logger code as a file (for debugging purposes).
    /// </summary>
    /// <param name="fileName">The save name.</param>
    public void WriteGeneratedCodeFile(string fileName) {
      System.IO.File.WriteAllText(fileName, _generatedCode);
    }
  }
}
