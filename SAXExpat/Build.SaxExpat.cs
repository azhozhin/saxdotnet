using System;
using System.IO;
using System.Globalization;
using System.Diagnostics;
using Build;

namespace Build.SaxExpat
{
  /**<summary>
   * Class for building all SaxExpat projects. For building ExpatInterop.dll
   * it must be able to find ildasm and ilasm as "ildasm" and "ilasm".
   * </summary>
   */
  class Builder: BaseBuilder
  {
    
    /// <summary>
    /// Executes process with arguments, waiting for its completion.
    /// </summary>
    static void CallProcess(string procName, string procArgs, string logFile)
    {
      Process proc = new Process();
      proc.StartInfo.FileName = procName;
      proc.StartInfo.Arguments = procArgs;
      proc.EnableRaisingEvents = true;
      StreamWriter writer = null;
      if (logFile != null && logFile != String.Empty) {
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.RedirectStandardOutput = true;
        proc.StartInfo.CreateNoWindow = true;
        writer = new StreamWriter(logFile);
      }
      else {
        proc.StartInfo.UseShellExecute = true;     
      }
      proc.Start();
      if (writer != null) {
        writer.Write(proc.StandardOutput.ReadToEnd());
        writer.Close();
      }
      proc.WaitForExit(60000);
      proc.Close();
    }
    
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      // parse args into variables
      bool debug = false;
      // convert to lower case
      for (int index = 0; index < args.Length; index++)
        args[index] = args[index].ToLower(CultureInfo.InvariantCulture);
      debug = Array.IndexOf(args, "/debug") != -1 || Array.IndexOf(args, "--debug") != -1;

      string projectDir = null;
      string[] sources = null;
      string[] resources = null;
      string[] references = null;
      string resxFile;
      string resFile;

      /* Build KdsText.dll */

      string utilDir = Path.Combine(BuildDir, "Util");
      projectDir = Path.Combine(utilDir, "Text");
      sources = new string[] {
        "TextUtils.cs",
        "TextRes.cs",
        "AssemblyInfo.cs"
      };
      resources = new string[] { "Kds.Text.KdsText.resources" };
      resxFile = Path.Combine(projectDir, "KdsText.resx");
      resFile = Path.Combine(projectDir, resources[0]);
      ConvertResources(resxFile, resFile);
      string kdsTextDll = BuildAssembly(
        "KdsText.dll", "library", projectDir, sources, resources, null, debug, "/unsafe");

      /* Build ExpatInterop.dll */

      projectDir = Path.Combine(BuildDir, "ExpatInterop");
      sources = new string[] {
        "LibExpat.cs",
        "ExpatHelpers.cs",
        "ExpatParser.cs",
        "AssemblyInfo.cs"
      };
      references = new string[] { kdsTextDll };
      string interopDll = BuildAssembly(
        "ExpatInterop.dll", "library", projectDir, sources, null, references, debug, "/unsafe /d:EXPAT_1_95_8_UP");
        
      /* Build KdsSax.dll */

      projectDir = Path.Combine(BuildDir, "SAX");
      sources = new string[] {
        "Sax.cs",
        "SaxHelpers.cs",
        "AssemblyInfo.cs"
      };
      resources = new string[] { "Kds.Xml.Sax.KdsSax.resources" };
      resxFile = Path.Combine(projectDir, "KdsSax.resx");
      resFile = Path.Combine(projectDir, resources[0]);
      ConvertResources(resxFile, resFile);
      string saxDll = Path.Combine(BuildDir, "Sax.dll");
      references = new string[] { saxDll };
      string kdsSaxDll = BuildAssembly(
        "KdsSax.dll", "library", projectDir, sources, resources, references, debug, "");

      /* Build SaxExpat.dll */

      projectDir = Path.Combine(BuildDir, "SAXExpat");
      sources = new string[] {
        "SaxExpat.cs",
        "AssemblyInfo.cs"
      };
      resources = new string[] { "Kds.Xml.Expat.SaxExpat.resources" };
      resxFile = Path.Combine(projectDir, "SaxExpat.resx");
      resFile = Path.Combine(projectDir, resources[0]);
      ConvertResources(resxFile, resFile);
      references = new string[] {
        saxDll,
        kdsSaxDll,
        kdsTextDll,
        interopDll
      };
      string SaxExpatDll = BuildAssembly(
        "SaxExpat.dll", "library", projectDir, sources, resources, references, debug, "/unsafe /d:EXPAT_1_95_8_UP");
    }
  }
}
