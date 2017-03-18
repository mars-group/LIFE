using System.Collections.Generic;
using System.Text;

namespace ResultAdapter.Implementation {

  /// <summary>
  ///   The logger compiler assembles the logger source code and compiles the
  ///   logger classes into an assembly, which then is loaded into the runtime.
  /// </summary>
  internal class LoggerCompiler {


    /// <summary>
    ///   Assemble all logger classes in a source code file and analyze using directives.
    /// </summary>
    /// <param name="loggerClasses">The C# classes to compile.</param>
    /// <returns>String for complete source code file.</returns>
    internal static string GenerateSourceCodeFile(IEnumerable<string> loggerClasses) {

      var usingList = new List<string> {
        "using System.Collections.Generic;",
        "using LIFE.API.Agent;",
        "using ResultAdapter.Implementation;"
      };

      //TODO ...
      //TODO Usings analysieren.

      // Join the classes with the required imports to a single source code file.
      var usings = new StringBuilder();
      foreach (var str in usingList) usings.Append(str + "\n");
      usings.Append("// ReSharper disable InconsistentNaming\n");
      var loggerCode = new StringBuilder();
      foreach (var logger in loggerClasses) loggerCode.Append(logger);
      return string.Format(
        "{0}" +
        "\nnamespace ResultLoggerGenerated {{\n" +
        "{1}" +
        "}}\n",
        usings,
        loggerCode
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
        "    public string GetKeyFrame() {{\n{2}    }}\n\n" +
        "    public string GetDeltaFrame() {{\n{3}    }}\n" +
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
        switch (config.SpatialType.ToUpper()) { //| Check the agent's spatial type. For now,
          case "GPS":                           //| only the LIFE base agents are supported.
            spatialData = ",\n" +
            "        StaticPosition = new object[] {_agent.Latitude, _agent.Longitude},\n" +
            "        StaticOrientation = new object[] {_agent.Bearing}";
            break;
          case "GRID":
            spatialData = ",\n" +
            "        StaticPosition = new object[] {_agent.X, _agent.Y},\n" +
            "        StaticOrientation = new object[] {(int) _agent.GridDirection}";
            break;
          case "2D":
            spatialData = ",\n" +
            "        StaticPosition = new object[] {_agent.Position.X, _agent.Position.Y},\n" +
            "        StaticOrientation = new object[] {_agent.Position.Yaw}";
            break;
          case "3D":
            spatialData = ",\n" +
            "        StaticPosition = new object[] {_agent.Position.X, _agent.Position.Y, _agent.Position.Z},\n" +
            "        StaticOrientation = new object[] {_agent.Position.Yaw, _agent.Position.Pitch}";
            break;
          default:
            spatialData = "";
            break;
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

      //TODO Umbauen!
      return "      return \"\";\n";


      var dynProps = new List<string>();
      foreach (var prop in config.Properties) if (!prop.Value) dynProps.Add(prop.Key);

      // Build the keyframe output code.
      var posOut = config.SpatialType != null && !config.IsStationary;
      var propQuery = "";
      var propFormat = "";
      for (var i = 0; i < dynProps.Count; i ++) {
        propQuery += string.Format(
          "        _agent.{0}{1}\n",
          dynProps[i],
          i<dynProps.Count-1? "," : ""
        );
        propFormat += string.Format(
          "        \"  \\\"{0}\\\": \\\"{{{1}}}\\\"{2}\\n\" +\n",
          dynProps[i],
          posOut? i+6 : i,
          i<dynProps.Count-1? "," : ""
        );
      }

      // Build and return the string that represents the function content.
      return string.Format(
        "      return string.Format(\n{0}{1}{2}" +
        "      );\n",

        // Position and orientation output for mobile units.
        posOut?
        "        \"\\\"Position\\\": [{0}, {1}, {2}],\\n\" +\n" +
        "        \"\\\"Orientation\\\": [{3}, {4}, {5}]"
        : "",

        // Custom property output, if available.
        dynProps.Count > 0?
        (posOut? ",\\n\" +\n" : "") + // Close spatial tag, if we had it.
        "        \"\\\"Properties\\\": {{\\n\" +\n" + propFormat +
        "        \"}}\\n\",\n"
        : "\\n\",\n",

        // Internal string format paramter list.
        (posOut? string.Format(
          "        _agent.SpatialData.Position.X,\n" +
          "        _agent.SpatialData.Position.Y,\n" +
          "        _agent.SpatialData.Position.Z,\n" +
          "        _agent.SpatialData.Direction.Yaw,\n" +
          "        _agent.SpatialData.Direction.Pitch,\n" +
          "        0.0f"
        ) : "") +
        (dynProps.Count > 0? (posOut? ",\n" : "")+propQuery : "\n")
      );
    }


    /// <summary>
    ///   Generate the delta frame output code.
    /// </summary>
    /// <param name="config">Logger configuration to use.</param>
    /// <returns>C# snippet for the delta frame output function.</returns>
    internal static string GenerateFragmentDeltaframe(LoggerConfig config) {
      //TODO Hier die Deltafunktion zusammenbauen!
      return "      return \"\";\n";
    }






    /*
    /// <summary>
    ///   Analyze the required imports and assembly references.
    /// </summary>
    /// <param name="types">The agent types used in the loggers.</param>
    /// <param name="usingStr">The usings needed by the loggers.</param>
    /// <param name="dlls">Required libraries to link against.</param>
    /// <returns>Success flag. Set to 'false', if some error occured.</returns>
    private static bool AnalyseUsings(IList<string> types, out string usingStr, out List<string> dlls) {
      usingStr = "";
      dlls = new List<string>();
      var usings = new List<string> {
        "using System;",
        "using System.Collections.Generic;",
        "using LifeAPI.Agent;",
        "using ResultAdapter.Implementation.ResultLoggerGenerator;"
      };

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
      return true;
    }



    /// <summary>
    ///   Compile the result logger classes to MSIL during runtime via the Roslyn compiler.
    /// </summary>
    /// <param name="usings">Import directives needed by the logger classes.</param>
    /// <param name="dlls">Required dependencies (dll paths).</param>
    /// <param name="loggerClasses">The C# classes to compile.</param>
    /// <returns>The loaded assembly or 'null', if something failed.</returns>
    private static Assembly CompileCode(string usings, IEnumerable<string> dlls, IEnumerable<string> loggerClasses) {

      // Join the classes with the required imports to a single source code file.
      var loggerCode = new StringBuilder();
      foreach (var logger in loggerClasses) loggerCode.Append(logger);
      var codeFile = string.Format(
        "{0}" +
        "\nnamespace ResultLoggerGenerated {{\n" +
        "{1}" +
        "}}\n",
        usings,
        loggerCode
      );
      //Console.WriteLine(codeFile);

      // All references needed for the logger code to compile.
      var references = new List<MetadataReference> {
        MetadataReference.CreateFromFile(typeof (object).Assembly.Location),     // mscorlib.dll
        MetadataReference.CreateFromFile(typeof (Enumerable).Assembly.Location), // System.Core.dll
        MetadataReference.CreateFromFile("ResultAdapter.dll")  // We need the IGeneratedSimResult.
      };
      foreach (var dll in dlls) references.Add(MetadataReference.CreateFromFile(dll));


      // Create the compilation object based on the code to compile and default parameters.
      var compilation = CSharpCompilation.Create(
        Path.GetRandomFileName(),                      // Name of the DLL.
        new[] {CSharpSyntaxTree.ParseText(codeFile)},  // The syntax tree.
        references,                                    // Required assemblies.
        new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary) // Output type.
      );
      var stream = new MemoryStream();
      var result = compilation.Emit(stream);

      // Compilation failed: Output compiler errors.
      if (!result.Success) {
        var failures = result.Diagnostics.Where(diagnostic =>
          diagnostic.IsWarningAsError || diagnostic.Severity == DiagnosticSeverity.Error
        );
        foreach (var err in failures) {
          Console.Error.WriteLine("{0}: {1}", err.Id, err.GetMessage());
        }
        return null;
      }

      // DLL successfully built! Return loaded assembly.
      stream.Seek(0, SeekOrigin.Begin);
      return Assembly.Load(stream.ToArray());
    }*/




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
  }



  /// <summary>
  ///   Wrapper class to execute code in a separate app domain.
  /// </summary>
  /// <typeparam name="T">Class type to instantiate.</typeparam>
  internal sealed class Isolated<T> : IDisposable where T : MarshalByRefObject {

    private AppDomain _domain;           // App domain reference.
    public T Value { get; private set; } // Instance to execute.


    /// <summary>
    ///   Create new app domain and instance.
    /// </summary>
    public Isolated() {
      _domain = AppDomain.CreateDomain(
        "Isolated:" + Guid.NewGuid(),
        null,
        AppDomain.CurrentDomain.SetupInformation
      );
      var type = typeof (T);
      Value = (T) _domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
    }


    /// <summary>
    ///   Destroy this app domain.
    /// </summary>
    public void Dispose() {
      if (_domain != null) {
        try { AppDomain.Unload(_domain); }
        catch (Exception ex) { Console.WriteLine(ex); }
        _domain = null;
      }
    }
  }  */
}
