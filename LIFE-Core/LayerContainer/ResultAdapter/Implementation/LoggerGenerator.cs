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
    private readonly LoggerCompiler _compiler;              // Source code templater and compiler.
    private readonly string _rcsHost;                       // ResultConfigService address.
    private readonly string _configId;                      // Configuration identifier.
    private string _generatedCode;                          // The generated code file.

    internal string CodeFile; //TODO


    /// <summary>
    ///   Create a new logger generator.
    /// </summary>
    /// <param name="rcsHost">ResultConfigService address.</param>
    /// <param name="configId">Configuration identifier.</param>
    public LoggerGenerator(string rcsHost, string configId) {
      _definitions = new Dictionary<string, Type>();
      _compiler = new LoggerCompiler();
      _rcsHost = rcsHost;
      _configId = configId;
      var json = GetConfiguration();
      if (json != null) {
        var loggerDefs = ParseConfiguration(json);
        GenerateLoggerPrototypes(loggerDefs);
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
    ///   Generate the logger prototypes for the given configs.
    /// </summary>
    /// <param name="loggerConfigs">Logger descriptions.</param>
    private void GenerateLoggerPrototypes(IEnumerable<LoggerConfig> loggerConfigs) {
      var loggerClasses = new List<string>();
      foreach (var loggerConfig in loggerConfigs) {
        var codeSnippets = new LoggerCodeFragment {
          TypeName = loggerConfig.TypeName,
          MetaCode = LoggerCompiler.GenerateFragmentMetadata(loggerConfig),
          KeyframeCode = LoggerCompiler.GenerateFragmentKeyframe(loggerConfig),
          DeltaframeCode = LoggerCompiler.GenerateFragmentDeltaframe(loggerConfig)
        };
        loggerClasses.Add(LoggerCompiler.GenerateLoggerClass(codeSnippets));
        //TODO
        _definitions.Add(loggerConfig.TypeName, loggerConfig.GetType());
      }
      _generatedCode = LoggerCompiler.GenerateSourceCodeFile(loggerClasses);

      CodeFile = _generatedCode; //TODO

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
  }
}
