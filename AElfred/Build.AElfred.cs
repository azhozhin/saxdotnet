using System;
using System.Globalization;
using Build;

namespace Build.AElfred
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

      string[] sources = null;
      string[] references = null;

      /* Build AElfred.dll */
 
      sources = new string[] {
        "Handlers.cs",
        "NamespaceSupport.cs",
        "XmlUtils.cs",
        "SaxDriver.cs",
        "XmlParser.cs",
        "AssemblyInfo.cs"
      };
      references = new string[] { 
        "Sax.dll"
      };
  
      BuildAssembly("AElfred.dll", "library", BuildDir, sources, null, references, debug, null);
    }
  }
}
