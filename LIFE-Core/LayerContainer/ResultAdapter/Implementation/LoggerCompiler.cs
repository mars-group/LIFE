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

    private readonly string _systemPath; // Path to the .NET Core runtime DLLs.
    private readonly string _modelPath;  // Path to the simulation model folder.


    /// <summary>
    ///   Create a new logger compiler.
    /// </summary>
    /// <param name="modelPath">Model folder path, relative to working directory.</param>
    public LoggerCompiler(string modelPath) {
      _modelPath = Path.GetFullPath(modelPath);
      _systemPath = Path.GetDirectoryName(typeof(object).GetTypeInfo().Assembly.Location);
    }


    /// <summary>
    ///   Generate the logger code for a set of logger definitions.
    /// </summary>
    /// <param name="configs">The specifications of the loggers to create.</param>
    /// <param name="dlls">References libraries needed for compilation.</param>
    /// <returns>The complete source code file, nicely formatted and annotated. :-) </returns>
    internal string GenerateLoggerCode(IEnumerable<LoggerConfig> configs, out IList<string> dlls) {
      var usings = new List<string> {
        "LIFE.API.Agent",
        "ResultAdapter.Implementation",
        "System.Collections.Generic"
      };
      var loggerClasses = new List<string>();
      foreach (var config in configs) {
        loggerClasses.Add(GenerateLoggerClass(config));
        var ns = config.FullName.Split(',')[0];    //| Get the agent namespace
        var li = ns.LastIndexOf('.');              //| from the fully-qualified
        if (li != -1) ns = ns.Substring(0, li);    //| name and add it to the
        if (!usings.Contains(ns)) usings.Add(ns);  //| usings, if not done yet.
      }
      usings.Sort();

      // Also get the DLLs. Because the dependency resolver still sucks, we just do the overkill
      // and add all libraries as references. Better too much than missing one. It won't hurt anyway.
      dlls = new List<string>();
      foreach (var fileinfo in new DirectoryInfo(_modelPath).GetFileSystemInfos("*.dll")) {
        dlls.Add(fileinfo.FullName);
      }

      // Now assemble the logger code file and return it.
      var codeFile = GenerateSourceCodeFile(loggerClasses, usings);
      return codeFile;
    }


    /// <summary>
    ///   Compile a code file and load it into the running program.
    /// </summary>
    /// <param name="codeFile">The complete source code file.</param>
    /// <param name="dlls">All referenced DLLs needed for compilation.</param>
    /// <returns>Compiled and loaded DLL or 'null', if any compilation error occured.</returns>
    internal Assembly CompileSourceCode(string codeFile, IEnumerable<string> dlls) {

      Console.WriteLine("-0--");

      var references = new List<MetadataReference> {
        MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
        MetadataReference.CreateFromFile(Path.Combine(_systemPath, "mscorlib.dll")),
        MetadataReference.CreateFromFile(Path.Combine(_systemPath, "System.Runtime.dll"))
      };

      Console.WriteLine("-1-");

      foreach (var dll in dlls) references.Add(MetadataReference.CreateFromFile(dll));
      var cmpOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

      Console.WriteLine("-2-");

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
        Console.Error.WriteLine("[LoggerCompiler] Error: Source code compilation failed!");
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
      Console.WriteLine("[LoggerCompiler] Compilation succeeded.");
      stream.Seek(0, SeekOrigin.Begin);
      var assembly = AssemblyLoadContext.Default.LoadFromStream(stream);
      stream.Dispose();
      return assembly;
    }



    #region Source Code Generation Methods.

    /// <summary>
    ///   Assemble all logger classes in a source code file.
    /// </summary>
    /// <param name="loggerClasses">The C# classes to compile.</param>
    /// <param name="usings">The required using directives.</param>
    /// <returns>String for complete source code file.</returns>
    private static string GenerateSourceCodeFile(IEnumerable<string> loggerClasses, IEnumerable<string> usings) {
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
    /// <param name="config">Logger configuration directives.</param>
    /// <returns>C# code for the logger class.</returns>
    private static string GenerateLoggerClass(LoggerConfig config) {
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
        config.TypeName,
        GenerateFragmentMetadata(config),
        GenerateFragmentKeyframe(config),
        GenerateFragmentDeltaframe(config)
      );
      return code;
    }


    /// <summary>
    ///   Generate the meta data entry output function for the logger.
    /// </summary>
    /// <param name="config">Logger configuration to use.</param>
    /// <returns>Generator function content for the agent metadata entry.</returns>
    private static string GenerateFragmentMetadata(LoggerConfig config) {
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
    private static string GenerateFragmentKeyframe(LoggerConfig config) {
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
    private static string GenerateFragmentDeltaframe(LoggerConfig config) {
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

    #endregion
  }
}
