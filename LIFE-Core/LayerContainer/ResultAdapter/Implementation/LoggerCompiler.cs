using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ResultAdapter.Implementation {

  /// <summary>
  ///   The logger compiler assembles the logger source code and compiles the
  ///   logger classes into an assembly, which then is loaded into the runtime.
  /// </summary>
  internal class LoggerCompiler {


    /// <summary>
    ///   Assemble all logger classes in a source code file.
    /// </summary>
    /// <param name="loggerClasses">The C# classes to compile.</param>
    /// <param name="usings">The required using directives.</param>
    /// <returns>String for complete source code file.</returns>
    internal static string GenerateSourceCodeFile(IEnumerable<string> loggerClasses, IEnumerable<string> usings) {
      var usingStr = new StringBuilder();
      foreach (var str in usings) usingStr.Append("using "+str+";\n");
      usingStr.Append("// ReSharper disable InconsistentNaming\n");
      var loggerCode = new StringBuilder();
      foreach (var logger in loggerClasses) loggerCode.Append(logger);
      return string.Format(
        "{0}" +
        "\nnamespace ResultLoggerGenerated {{\n" +
        "{1}" +
        "}}\n",
        usingStr, loggerCode
      );
    }


    /// <summary>
    ///   Generate the class for a result logger.
    /// </summary>
    /// <param name="snippets">The logger code fragments.</param>
    /// <returns>C# code for the logger class.</returns>
    internal static string GenerateLoggerClass(LoggerCodeFragment snippets) {
      var code = string.Format(
        "\n\n" +
        "  /// <summary>\n" +
        "  ///   Generated result logger for the '{0}' agent type.\n" +
        "  /// </summary>\n" +
        "  public class ResultLogger_{0} : IGeneratedLogger {{\n" +
        "    private readonly {0} _agent;\n\n" +
        "    public ResultLogger_{0}(ITickClient agent) {{\n" +
        "      _agent = ({0}) agent;\n" +
        "    }}\n\n" +
        "    public AgentMetadataEntry GetMetatableEntry() {{\n{1}    }}\n\n" +
        "    public AgentFrame GetKeyFrame() {{\n{2}    }}\n\n" +
        "    public AgentFrame GetDeltaFrame() {{\n{3}    }}\n" +
        "  }}\n",
        snippets.TypeName,
        snippets.MetaCode,
        snippets.KeyframeCode,
        snippets.DeltaframeCode
      );
      return code;
    }


    /// <summary>
    ///   Generate the meta data entry output function for the logger.
    /// </summary>
    /// <param name="config">Logger configuration to use.</param>
    /// <returns>Generator function content for the agent metadata entry.</returns>
    internal static string GenerateFragmentMetadata(LoggerConfig config) {
      var statProps = new List<string>();
      foreach (var prop in config.Properties) if (prop.Value) statProps.Add(prop.Key);

      // Build the static property list for the meta table entry.
      var staticsListing = "";
      for (var i = 0; i < statProps.Count; i++) {
        staticsListing += string.Format(
          "          {{\"{0}\", _agent.{0}}}{1}\n",
          statProps[i],
          i<statProps.Count-1? "," : ""
        );
      }

      // Build the static position/orientation output, if applicable.
      var spatialData = "";
      if (config.SpatialType != null && config.IsStationary) {
        string posStr, ortStr;
        var success = GetSpatialOutputStrings(config.SpatialType, out posStr, out ortStr);
        if (success) {
          spatialData =
            ",\n        StaticPosition = "+posStr+
            ",\n        StaticOrientation = "+ortStr;
        }
      }


      // Build and return the meta data structure.
      return string.Format(
        "      return new AgentMetadataEntry {{\n" +
        "        AgentId = _agent.ID.ToString(),\n" +
        "        AgentType = _agent.GetType().Name,\n" +
        "        Layer = _agent.Layer.GetType().Name{0}{1}\n" +
        "      }};\n",
        spatialData,
        !staticsListing.Equals("")? string.Format(
          ",\n" +
          "        StaticProperties = new Dictionary<string, object> {{\n{0}" +
          "        }}",
          staticsListing
        ) : ""
      );
    }


    /// <summary>
    ///   Generate the logger's key frame output function.
    /// </summary>
    /// <param name="config">Logger configuration to use.</param>
    /// <returns>Key frame generation function content.</returns>
    internal static string GenerateFragmentKeyframe(LoggerConfig config) {
      var dynProps = new List<string>();
      foreach (var prop in config.Properties) if (!prop.Value) dynProps.Add(prop.Key);

      // Build the spatial output strings.
      string posStr = "null", ortStr = "null";
      if (config.SpatialType != null && !config.IsStationary) {
        GetSpatialOutputStrings(config.SpatialType, out posStr, out ortStr);
      }

      // Build the property output listing.
      var propList = "null";
      if (dynProps.Count > 0) {
        propList = "new Dictionary<string, object> {\n";
        for (var i = 0; i < dynProps.Count; i++) {
          propList += string.Format(
            "          {{\"{0}\", _agent.{0}}}{1}\n",
            dynProps[i], i<dynProps.Count-1? "," : ""
          );
        }
        propList += "        }";
      }

      // Return the key frame.
      return string.Format(
        "      return new AgentFrame {{\n" +
        "        IsKeyframe = true,\n"+
        "        AgentId = _agent.ID.ToString(),\n" +
        "        Tick = _agent.GetTick(),\n"+
        "        Position = {0},\n"+
        "        Orientation = {1},\n"+
        "        Properties = {2}\n"+
        "      }};\n",
        posStr, ortStr, propList
      );
    }


    /// <summary>
    ///   Generate the delta frame output code.
    /// </summary>
    /// <param name="config">Logger configuration to use.</param>
    /// <returns>C# snippet for the delta frame output function.</returns>
    internal static string GenerateFragmentDeltaframe(LoggerConfig config) {
      //TODO Hier die Deltafunktion zusammenbauen!
      return string.Format(
        "      return new AgentFrame {{\n" +
        "        IsKeyframe = false,\n"+
        "        AgentId = _agent.ID.ToString(),\n" +
        "        Tick = _agent.GetTick()\n"+
        "      }};\n"
      );
    }


    /// <summary>
    ///   Build the output strings for the position and orientation array.
    /// </summary>
    /// <param name="sType">The agent's spatial type [GPS, Grid, 2D, 3D].</param>
    /// <param name="posStr">Position output array string.</param>
    /// <param name="ortStr">Orientation output array string.</param>
    /// <returns>String generation success boolean.</returns>
    private static bool GetSpatialOutputStrings(string sType, out string posStr, out string ortStr) {
      switch (sType.ToUpper()) {  //| Check the agent's spatial type. For now,
        case "GPS":               //| only the LIFE base agents are supported.
          posStr = "new object[] {_agent.Latitude, _agent.Longitude}";
          ortStr = "new object[] {_agent.Bearing}";
          return true;
        case "GRID":
          posStr = "new object[] {_agent.X, _agent.Y}";
          ortStr = "new object[] {(int) _agent.GridDirection}";
          return true;
        case "2D":
          posStr = "new object[] {_agent.Position.X, _agent.Position.Y}";
          ortStr = "new object[] {_agent.Position.Yaw}";
          return true;
        case "3D":
          posStr = "new object[] {_agent.Position.X, _agent.Position.Y, _agent.Position.Z}";
          ortStr = "new object[] {_agent.Position.Yaw, _agent.Position.Pitch}";
          return true;
        default:
          posStr = "null";
          ortStr = "null";
          return false;
      }
    }





    internal static Assembly CompileLoggers() {

      var codeFile = @"
        using System;
        using ResultAdapterTests;

        namespace RoslynCompileSample {

          public class Writer {

            private readonly Sheep _sheep;

            public void Write(string message) {
              Console.WriteLine($""you said '{message}!'"");
            }
          }
        }";


      var dlls = new List<string>();
      //dlls.Add(ResultAdapterTests.dll"); // We need the IGeneratedSimResult.


      // Add all references needed for this code to compile.
      var references = new List<MetadataReference> {
        MetadataReference.CreateFromFile(typeof (object).GetTypeInfo().Assembly.Location),
        //MetadataReference.CreateFromFile(typeof (Enumerable).GetTypeInfo().Assembly.Location),
      };
      foreach (var dll in dlls) references.Add(MetadataReference.CreateFromFile(dll));
      var cmpOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);


      // Create the compilation object based on the code to compile and default parameters.
      var compilation = CSharpCompilation.Create(
        Path.GetRandomFileName(),                     // Name of the DLL.
        new[] {CSharpSyntaxTree.ParseText(codeFile)}, // The syntax tree.
        references,                                   // Required assemblies.
        cmpOptions                                    // Output type.
      );
      var stream = new MemoryStream();
      var result = compilation.Emit(stream);

      // Compilation failed: Output compiler errors.
      if (!result.Success) {
        Console.Error.WriteLine("Result logger compilation failed!");
        var failures = result.Diagnostics.Where(diagnostic =>
          diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error
        );
        foreach (var err in failures) {
          Console.Error.WriteLine("{0}: {1}", err.Id, err.GetMessage());
        }
        stream.Dispose();
        return null;
      }

      // DLL successfully built! Return loaded assembly.
      Console.WriteLine("Compilation successful! Now instantiating and executing the code ...");
      stream.Seek(0, SeekOrigin.Begin);
      var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
      stream.Dispose();
      return assembly;
    }


    /// <summary>
    ///   Analyze the required imports and assembly references.
    /// </summary>
    /// <param name="types">The agent types used in the loggers.</param>
    /// <param name="dir">DLL base directory.</param>
    /// <param name="usings">The usings needed by the loggers.</param>
    /// <param name="dlls">Required libraries to link against.</param>
    /// <returns>Success flag. Set to 'false', if some error occured.</returns>
    internal static bool AnalyseUsings(IEnumerable<string> types, string dir,
      out IEnumerable<string> usings, out IEnumerable<string> dlls) {

      // Add the base DLL and usings that are always required.
      dlls = new List<string> {
        dir+"/ResultAdapter.dll"
      };
      usings = new List<string> {
        "System.Collections.Generic",
        "LIFE.API.Agent",
        "ResultAdapter.Implementation"
      };




/*
      // Get the directory that holds all relevant model assemblies.
      const string modelDir = "layers/addins/";
      var pathToModel = "";
      foreach (var dir in Directory.GetDirectories(modelDir)) {
        if (!dir.Equals("tmp")) {    //| This function is a big workaround!
          pathToModel = dir;         //| We'd rather need a way to get the
          break;                     //| model directory (also for multi model
        }                            //| support we hopefully have one day).
      }
      if (pathToModel.Equals("")) return false;  // No model found!

      // Else run the assembly resolver in a separate app domain (for DLL unload).
      using (var isolated = new Isolated<AssemblyResolver>()) {
        var success = isolated.Value.Resolve(pathToModel, false, types, ref usings, out dlls);
        if (!success) return false;
      }

      // Concatenate the usings and return.
      var sb = new StringBuilder();
      foreach (var str in usings) sb.Append(str + "\n");
      usingStr = sb.ToString();
*/
      return true;
    }
  }
}





 /*


  /// <summary>
  ///   This class provides DLL reflection logic for the agents types used by the loggers.
  /// </summary>
  // ReSharper disable once ClassNeverInstantiated.Global
  internal class AssemblyResolver : MarshalByRefObject {


    // ReSharper disable once MemberCanBeMadeStatic.Global
    internal bool Resolve(string path, bool debug, IList<string> types, ref List<string> usings, out List<string> dlls) {
      dlls = new List<string>();

      // Add an assembly resolver to load the dependencies.
      AppDomain.CurrentDomain.AssemblyResolve += (sender, args) => {
        foreach (var ass in AppDomain.CurrentDomain.GetAssemblies()) {
          if (ass.FullName == args.Name) return ass;
        }
        var dllPath = Path.Combine(path, new AssemblyName(args.Name).Name + ".dll");
        if (debug) Console.WriteLine("[AssemblyResolver] Resolving " + dllPath);
        if (File.Exists(dllPath)) return Assembly.LoadFile(new FileInfo(dllPath).FullName);
        if (debug) Console.Error.WriteLine("[AssemblyResolver] Resolution of '"+dllPath+"' failed!");
        return null;
      };


      // Check for the LIFE API. This library is required for all output types.
      Type tc = null;
      var lifePath = path + "/LifeAPI.dll";
      if (File.Exists(lifePath)) {
        var assembly = Assembly.LoadFile(new FileInfo(lifePath).FullName);
        tc = assembly.GetType("LifeAPI.Agent.ITickClient");
      }
      if (tc == null) return false;


      // Loop over all libraries and check for the required types.
      var dllFile = Directory.GetFiles(path, "*.dll");
      foreach (var dll in dllFile) {
        try {
          var assembly = Assembly.LoadFile(new FileInfo(dll).FullName);
          var dllpath = path + "/" + assembly.FullName.Split(',')[0] + ".dll";
          if (!dlls.Contains(dllpath)) dlls.Add(dllpath);
          var includedTypes = assembly.GetTypes();
          foreach (var type in includedTypes) {
            if (types.Contains(type.Name)) {
              var usingStr = "using " + type.Namespace + ";";
              if (!usings.Contains(usingStr)) usings.Add(usingStr);
              if (debug) {
                Console.WriteLine("[AssemblyResolver] Found type '{0}' at '{1}' in DLL '{2}'",
                  type.Name, type.Namespace, assembly.FullName.Split(',')[0]);
              }
            }
          }
        }
        catch (Exception) {
          if (debug) Console.Error.WriteLine("[AssemblyResolver] Error loading '"+dll+"'!");
          return false;
        }
      }
      return true;
    }
  } */