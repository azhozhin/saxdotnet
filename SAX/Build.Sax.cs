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
      bool conformanceDemo = false;
      bool treeViewDemo = false;
      // convert to lower case
      for (int index = 0; index < args.Length; index++)
        args[index] = args[index].ToLower(CultureInfo.InvariantCulture);
      debug = Array.IndexOf(args, "/debug") != -1 || Array.IndexOf(args, "--debug") != -1;
      conformanceDemo = Array.IndexOf(args, "conformance") != -1;
      treeViewDemo = Array.IndexOf(args, "treeview") != -1;
      if (Array.IndexOf(args, "all") != -1) {
        conformanceDemo = true;
        treeViewDemo = true;
      }

      string projectDir = null;
      string[] sources = null;
      string[] resources = null;
      string[] references = null;
      string resxFile;
      string resFile;

      /* Build Sax.dll */

      projectDir = BuildDir;
      sources = new string[] {
        "Sax.cs",
        "SaxHelpers.cs",
        "SaxRes.cs",
        "AssemblyInfo.cs"
      };
      resources = new string[] {"Org.System.Xml.Sax.Sax.resources"};
      resxFile = Path.Combine(projectDir, "Sax.resx");
      resFile = Path.Combine(projectDir, resources[0]);
      ConvertResources(resxFile, resFile);

      BuildAssembly("Sax.dll", "library", projectDir, sources, resources, null, debug, null);

      /* Build Conformance demo */

      string demoDir = Path.Combine(BuildDir, "Demo");
      string demoSaxDll = Path.Combine(demoDir, "Sax.dll");

      if (conformanceDemo) {
        projectDir = Path.Combine(demoDir, "Conformance");
        sources = new string[] {
          "XmlConf.cs",
          "SaxConf.cs",
          "AssemblyInfo.cs"
        };
        references = new string[] {"System.Xml.dll", demoSaxDll};
        BuildAssembly("XmlConf.exe", "exe", projectDir, sources, null, references, debug, null);
      }
      
      /* Build Treeview demo */

      if (treeViewDemo) {
        projectDir = Path.Combine(demoDir, "Treeview");
        sources = new string[] {
          "MainForm.cs",
          "PickDialog.cs",
          "AssemblyInfo.cs"
        };
        resources = new string[] {
          "SaxTreeviewDemo.MainForm.resources",
          "SaxTreeviewDemo.PickDlg.resources"
        };
        resxFile = Path.Combine(projectDir, "MainForm.resx");
        resFile = Path.Combine(projectDir, resources[0]);
        ConvertResources(resxFile, resFile);
        resxFile = Path.Combine(projectDir, "PickDialog.resx");
        resFile = Path.Combine(projectDir, resources[1]);
        ConvertResources(resxFile, resFile);
        references = new string[] {
          "System.Drawing.dll",
          "System.Windows.Forms.dll",
          demoSaxDll
        };
        BuildAssembly("TreeviewDemo.exe", "winexe", projectDir, sources, resources, references, debug, null);
      }
    }
  }
}
