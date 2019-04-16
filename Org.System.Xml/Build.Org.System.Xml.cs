using System;
using System.IO;
using System.Globalization;
using Build;

namespace Build.Sax
{
  /**<summary>
   * Main class for build application.
   * </summary>
   */
  class Builder: BaseBuilder
  {
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
      string resxFile;
      string resFile;

      /* Build Org.System.Xml.dll */

      projectDir = BuildDir;
      sources = new string[] {
        "Types.cs",
        "Namespaces.cs",
        "XmlChars.cs",
        "XmlRes.cs",
        "AssemblyInfo.cs"
      };
      resources = new string[] {"Org.System.Xml.Xml.resources"};
      resxFile = Path.Combine(projectDir, "Xml.resx");
      resFile = Path.Combine(projectDir, resources[0]);
      ConvertResources(resxFile, resFile);

      BuildAssembly("Org.System.Xml.dll", "library", projectDir, sources, resources, null, debug, "/unsafe");
    }
  }
}
