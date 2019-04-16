// NO WARRANTY!  This code is in the Public Domain.
// Written by Jeff Rafter (jeffrafter@users.sourceforge.net)
// and Karl Waclawek (kwaclaw@users.sourceforge.net).

/*                       DEMO OVERVIEW
 *
 * This console application will test a given parser against the
 * W3C XML test suite (http://w3.org/xml/test) and produce a report as
 * XML document that can be viewed with the included xmlconf-results.xsl
 * style sheet.
 *
 * To use the program, first place XmlConf.exe in the xmlconf root directory
 * of the XML test suite, together with the xmlconf-results.xsl style sheet.
 *
 * Then run it by passing the following parameters:
 * <script file name>             ...test script, included with the test suite (mandatory);
 *                                usually named xmlconf.xml
 * /o <output file name>          ...output file (optional);
 *                                if not provided, defaults to results.xml
 * /a <assembly name>             ...assembly name or file that contains parser (optional);
 *                                if not provided, uses system default parser
 * /c <class name>                ...class name that contains parser (optional);
 *                                if not provided, uses first parser found in assembly;
 *                                ignored if assembly was not specified
 * /g <Generation output path>    ...path for generated conformance results (optional);
 * /e <Expected conformance path> ...path for expected conformance results (optional);
 *                                generation path is required if running conformance check
 */

using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Text.RegularExpressions;
using Org.System.Xml.Sax;
using Org.System.Xml.Sax.Helpers;

namespace XmlConf
{
  using SaxConsts = Org.System.Xml.Sax.Constants;

  /// <summary>
  /// The TestSuite class allows you to run a SAX Parser against the xmlconf test suite
  /// found at http://www.w3.org/XML/Test/. The output is a stream, and will ultimately
  /// create a test report.
  /// </summary>
  public class TestSuite : IContentHandler, IErrorHandler
  {
    /// <summary>
    /// The main entry point for the application. It will create the output stream,
    /// the input stream, and the XmlReader. It will also inform the TestSuite class
    /// whether or not it should output console messages.
    /// </summary>
    [STAThread]
    static void Main(string[] args)
    {
      string inFile, outFile, testPattern, assemblyName, readerName, saxGenerate, saxExpected;
      InputSource<FileStream> input = null;
      FileStream output = null;
      try {
        ParseArgs(args, out inFile, out outFile, out testPattern, out assemblyName, out readerName, out saxGenerate, out saxExpected);
        // Create the input source for the XMLTest listing
        input = new InputSource<FileStream>(new FileStream(inFile, FileMode.Open, FileAccess.Read));
        input.SystemId = input.Source.Name;
        // Create the output
        output = new FileStream(outFile, FileMode.Create);
        // Create the XmlReader
        IXmlReader reader = LoadParser(assemblyName, readerName);
        // Create and run the test suite
        TestSuite ts = new TestSuite(input, output, testPattern, reader, saxGenerate, saxExpected, true);
        // Close the streams
      }
      catch (Exception e) {
        Console.Write(e.Message);
      }
      finally {
        if (input != null)
          input.Source.Close();
        if (output != null)
          output.Close();
        Console.WriteLine("\n\nProcess complete, press enter");
        Console.Read();
      }
    }

    private enum ArgState: byte {None, Output, Pattern, Assembly, Class, Error, SaxGenerate, SaxExpected};

    private static void ParseArgs(string[]args,
                                  out string inFile,
                                  out string outFile,
                                  out string testPattern,
                                  out string assemblyName,
                                  out string readerName,
                                  out string saxGenerate,
                                  out string saxExpected)
    {
      const string usage = "Usage: XmlConf.exe Input [/o Output] [/t Pattern] [/a Assembly] [/c Class] [/g Output] [/e Expected]";
      string nl = Environment.NewLine;
      if (args.Length < 1)
        throw new ApplicationException("No parameters." + nl + usage);
      inFile = args[0];
      outFile = "results.xml";  // default
      testPattern = null;
      assemblyName = null;
      readerName = null;
      saxGenerate = null;
      saxExpected = null;

      ArgState state = ArgState.None;
      for (int indx = 1; indx < args.Length; indx++) {
        string arg = args[indx];
        switch (state) {
          case ArgState.None:
            if (arg == "/o" || arg == "/O")
            {
              state = ArgState.Output;
              continue;
            }
            if (arg == "/p" || arg == "/P")
            {
              state = ArgState.Pattern;
              continue;
            }
            if (arg == "/a" || arg == "/A")
            {
              state = ArgState.Assembly;
              continue;
            }
            if (arg == "/c" || arg == "/C")
            {
              state = ArgState.Class;
              continue;
            }
            if (arg == "/g" || arg == "/G")
            {
              state = ArgState.SaxGenerate;
              break;
            }
            if (arg == "/e" || arg == "/E")
            {
              state = ArgState.SaxExpected;
              break;
            }
            state = ArgState.Error;
            break;
          case ArgState.Output:
            outFile = arg;
            state = ArgState.None;
            break;
          case ArgState.Pattern:
            testPattern = arg;
            state = ArgState.None;
            break;
          case ArgState.Assembly:
            assemblyName = arg;
            state = ArgState.None;
            break;
          case ArgState.Class:
            readerName = arg;
            state = ArgState.None;
            break;
          case ArgState.SaxGenerate:
            saxGenerate = arg;
            state = ArgState.None;
            break;
          case ArgState.SaxExpected:
            saxExpected = arg;
            state = ArgState.None;
            break;
        }
        if (state == ArgState.Error)
          break;
      }
      if (state != ArgState.None)
        throw new ApplicationException("Parameter error." + nl + usage);
    }

    private static IXmlReader LoadParser(string assemblyName, string readerName) {
      IXmlReader reader = null;
      if (assemblyName == null || assemblyName == String.Empty)
        reader = SaxReaderFactory.CreateReader(null);
      else {
        Assembly assem = null;
        if (File.Exists(assemblyName))
          assem = Assembly.LoadFrom(assemblyName);
        else
          assem = Assembly.Load(assemblyName);
        if (assem == null)
          throw new ApplicationException(
                     String.Format("Assembly not found: {0}.", assemblyName));
        if (readerName == null || readerName == String.Empty)
          // we have only an assembly, so load whatever IXmlReader we can find
          reader = SaxReaderFactory.CreateReader(assem, null);
        else
          // class and assembly specified, we know exactly what we want
          reader = SaxReaderFactory.CreateReader(assem, readerName, null);
      }
      return reader;
    }

    private IXmlReader reader;
    private InputSource input = null;
    private Stream output = null;
    private StreamWriter writer = null;
    private string saxGenerate = null;
    private string saxExpected = null;
    private string testPattern = null;
    private bool console = false;
    private ILocator testLoc = null;

    private bool supportsValidation = false;
    private bool supportsXml11 = false;
    private bool supportsNamespaces = false;
    private bool supportsParameterEntityResolution = false;
    private bool supportsGeneralEntityResolution = false;

    private bool inTest = false;
    private string currProfile = null;
    private string currId = null;
    private string currType = null;
    private string currEntities = null;
    private string currUri = null;
    private string currSections = null;
    private string currRecommendation = null;
    private string currVersion = null;
    private string currNamespaces = null;
    private StringBuilder currContent = new StringBuilder();
    private StringBuilder escaper = new StringBuilder();

    private DateTime startTime;
    private TimeSpan totalDur;
    private int passCount = 0;
    private int failCount = 0;
    private int conformanceFailCount = 0;
    private int unsupportedCount = 0;
    private int fatalCount = 0;
    private int errorCount = 0;
    private int warningCount = 0;

    public TestSuite(InputSource input, Stream output, string testPattern, IXmlReader reader, string saxGenerate, string saxExpected, bool console)
    {
      this.reader = reader;
      this.saxGenerate = saxGenerate;
      this.saxExpected = saxExpected;
      this.console = console;
      this.input = input;
      this.output = output;
      this.testPattern = testPattern;
      this.writer = new StreamWriter(output);
      writer.AutoFlush = true;
      // Test features and properties
      PrepareTest(reader);
      // Set the reader for the test document and parse
      reader.ContentHandler = this;
      reader.Parse(input);
      // Write the end
      FinalizeTest();
    }

    /// <summary>
    /// This creates the parser and attempts to set features that will be needed throughout the
    /// tests. The results are stored in booleans so that the features don't need to be rechecked
    /// for each test.
    /// </summary>
    public void PrepareTest(IXmlReader parser) {
      startTime = DateTime.Now;
      passCount = 0;
      failCount = 0;
      conformanceFailCount = 0;
      unsupportedCount = 0;
      writer.WriteLine("<?xml version=\"1.0\"?>");
      writer.WriteLine("<?xml-stylesheet href=\"xmlconf-results.xsl\" type=\"text/xsl\"?>");
      writer.WriteLine("<results>");
      writer.WriteLine("  <features>");
      // Try to get/set the required features
      supportsNamespaces = TryFeature(SaxConsts.NamespacesFeature, parser);
      supportsValidation = TryFeature(SaxConsts.ValidationFeature, parser);
      supportsXml11 = TryFeature(SaxConsts.Xml11Feature, parser);
      supportsParameterEntityResolution =
        TryFeature(SaxConsts.ExternalParameterFeature, parser);
      supportsGeneralEntityResolution =
        TryFeature(SaxConsts.ExternalGeneralFeature, parser);
      // Close out the features Xml
      writer.WriteLine("  </features>");
    }

    /// <summary>
    /// Handles the output of the results of trying to set the feature. Returns true if the
    /// feature can be set without exception.
    /// </summary>
    public bool TryFeature(string feature, IXmlReader parser) {
      int slashPos = feature.LastIndexOf('/');
      string featureName = feature.Substring(slashPos + 1);
      try {
        if (!parser.GetFeature(feature))
          parser.SetFeature(feature, true);
        writer.WriteLine("    <" + featureName + ">true</" + featureName + ">");
        return true;
      } catch {
        writer.WriteLine("    <" + featureName + ">false</" + featureName + ">");
        return false;
      }
    }

    /// <summary>
    /// Handles setting the feature.
    /// </summary>
    public void SetFeature(string feature, IXmlReader parser, bool flag) {
      if (parser.GetFeature(feature) != flag)
        parser.SetFeature(feature, flag);
    }

    /// <summary>
    /// This takes the collected test information, creates the parser and attempts to run the
    /// test. In general, it is a safe process and output will be written to the output stream.
    /// The RunTest() function attempts to configure the parser based on the requirements of
    /// the test. It also increments the testCount, unsupportedCount and failCount (if applicable).
    /// </summary>
    public void RunTest(IXmlReader parser) {
      bool expectFatalError = true;
      bool expectError = true;
      bool optionalFatalError = false;
      bool optionalError = false;

      // Check if this matches the testPattern
      if (testPattern != null)
        if (!Regex.IsMatch(currId, testPattern))
          return;

      try
      {
        // All wrapped in a try
        try {
          Uri f = new Uri(new Uri(testLoc.SystemId), currUri);

          writer.WriteLine("<TEST ENTITIES='" + currEntities + "' " +
            "ID='" + currId + "' "+
            "NAMESPACES='" + currNamespaces + "' "+
            "RECOMMENDATION='" + currRecommendation + "' "+
            "SECTIONS='" + currSections + "' "+
            "TYPE='" + currType + "' "+
            "URI='" + f.AbsoluteUri + "' "+
            "VERSION='" + currVersion + "'>");
          writer.WriteLine("<message>" + Escape(currContent.ToString()) + "</message>");

          // Check whether or not NAMESPACES are required
          if ("yes".Equals(currNamespaces)) {
            if (!supportsNamespaces) {
              writer.WriteLine("<unsupported>Namespace processing</unsupported>");
              unsupportedCount++;
              expectFatalError = false;
              expectError = false;
              fatalCount = 0;
              errorCount = 0;
              warningCount = 0;
              return;
            } else {
              SetFeature(SaxConsts.NamespacesFeature, parser, true);
              SetFeature(SaxConsts.NamespacePrefixesFeature, parser, false);
            }
          } else {
            SetFeature(SaxConsts.NamespacesFeature, parser, false);
            SetFeature(SaxConsts.NamespacePrefixesFeature, parser, true);
          }

          // Check whether or not ENTITIES are required
          if ("both".Equals(currEntities)) {
            if (!supportsGeneralEntityResolution || !supportsParameterEntityResolution) {
              writer.WriteLine("<unsupported>Entity resolution</unsupported>");
              unsupportedCount++;
              expectFatalError = false;
              expectError = false;
              fatalCount = 0;
              errorCount = 0;
              warningCount = 0;
              return;
            } else {
              SetFeature(SaxConsts.ExternalGeneralFeature, parser, true);
              SetFeature(SaxConsts.ExternalParameterFeature, parser, true);
            }
          } else if ("parameter".Equals(currEntities)) {
            if (!supportsParameterEntityResolution) {
              writer.WriteLine("<unsupported>Parameter entity resolution</unsupported>");
              unsupportedCount++;
              expectFatalError = false;
              expectError = false;
              fatalCount = 0;
              errorCount = 0;
              warningCount = 0;
              return;
            } else {
              SetFeature(SaxConsts.ExternalParameterFeature, parser, true);
            }
          } else if ("general".Equals(currEntities)) {
            if (!supportsGeneralEntityResolution) {
              writer.WriteLine("<unsupported>General entity resolution</unsupported>");
              unsupportedCount++;
              expectFatalError = false;
              expectError = false;
              fatalCount = 0;
              errorCount = 0;
              warningCount = 0;
              return;
            } else {
              SetFeature(SaxConsts.ExternalGeneralFeature, parser, true);
            }
          }

          // Check for the RECOMMENDATION
          if ("1.1".Equals(currVersion) || "XML1.1".Equals(currRecommendation) || "NS1.1".Equals(currRecommendation)) {
            if (!supportsXml11) {
              writer.WriteLine("<unsupported>XML 1.1</unsupported>");
              unsupportedCount++;
              expectFatalError = false;
              expectError = false;
              fatalCount = 0;
              errorCount = 0;
              warningCount = 0;
              return;
            } else {
              SetFeature(SaxConsts.Xml11Feature, parser, true);
            }
          }

          // Check the TYPE
          if ("valid".Equals(currType))
          {
            expectFatalError = false;
            expectError = false;
            if (supportsValidation)
              SetFeature(SaxConsts.ValidationFeature, parser, true);
          }
          else if ("invalid".Equals(currType))
          {
            expectFatalError = false;
            if (supportsValidation)
            {
              expectError = true;
              SetFeature(SaxConsts.ValidationFeature, parser, true);
            }
            else
            {
              expectError = false;
              // This is debateable, a non-validating parser may or may not raise an Error
              optionalError = true;
            }
          }
          else if ("not-wf".Equals(currType))
          {
            // no validation
            expectFatalError = true;
            expectError = false;
            optionalError = true;
          }
          else if ("error".Equals(currType))
          {
            // not required
            expectFatalError = false;
            expectError = false;
            optionalFatalError = true;
            optionalError = true;
          }

          // reset error count
          fatalCount = 0;
          errorCount = 0;
          warningCount = 0;


          // Try to parse
          if (saxGenerate == null)
          {
            parser.ErrorHandler = this;
            parser.Parse(f.AbsoluteUri);
          }
          else
          {
            DirectoryInfo confDir = new DirectoryInfo(saxGenerate);
            Uri baseUri = new Uri(testLoc.SystemId);
            if (!baseUri.IsFile)
              throw new ApplicationException("Not a file URI");
            DirectoryInfo baseDir = new DirectoryInfo(baseUri.LocalPath);
            string basePath = "";
            while (baseDir != null && baseDir.Name != "xmlconf") {
              basePath = Path.Combine(baseDir.Name, basePath);
              baseDir = baseDir.Parent;
            }
            string confPath = Path.Combine(confDir.FullName, basePath);
            confPath = Path.GetDirectoryName(confPath);
            FileInfo confFile = new FileInfo(Path.Combine(confPath, currUri));
            Directory.CreateDirectory(confFile.DirectoryName);
            StreamWriter sw = new StreamWriter(confFile.FullName);
            sw.NewLine = "\n";
            XmlTextWriter confWriter = new XmlTextWriter(sw);
            confWriter.Formatting = Formatting.Indented;
            confWriter.Indentation = 4;
            ConformanceReportHandler handler = new ConformanceReportHandler(confWriter, this);
            handler.Initialize();
            parser.ContentHandler = handler;
            parser.DtdHandler = handler;
            parser.ErrorHandler = handler;
            parser.EntityResolver = handler;
            try
            {
              parser.Parse(f.AbsoluteUri);
            }
            catch (SaxException)
            {
            }
            catch (IOException)
            {
            }
            catch (Exception ex)
            {
              confWriter.WriteStartElement("bug");
              confWriter.WriteAttributeString("reason", "Parser should only throw SAXExceptions");
              confWriter.WriteAttributeString("type", ex.GetType().Name);
              confWriter.WriteString(Escape(ex.Message));
              confWriter.WriteEndElement();
            }
            handler.Finish();

            // Close the writer
            confWriter.Close();

            // Now, let's just go ahead and make the comparisson while we are at it
            if (saxExpected != null)
            {
              DirectoryInfo expectedDir = new DirectoryInfo(saxExpected);
              string expectedPath = Path.Combine(expectedDir.FullName, basePath);
              expectedPath = Path.GetDirectoryName(expectedPath);
              FileInfo expectedFile = new FileInfo(Path.Combine(expectedPath, currUri));
              ResultComparer rc = new ResultComparer();
              string confError = "";
              bool confPassed = rc.compare(expectedFile.FullName, confFile.FullName, out confError);
              rc.makeResultDoc(f.LocalPath, expectedFile.FullName, confFile.FullName,
                Escape(currContent.ToString()), currId, confPassed, confError);

              writer.WriteLine("<conformance>");
              if (confPassed)
                writer.WriteLine("<pass>true</pass>");
              else
              {
                writer.WriteLine("<pass>false</pass>");
                conformanceFailCount++;
              }
              writer.WriteLine("<actual>" + confFile.FullName + "</actual>");
              writer.WriteLine("<expected>" + expectedFile.FullName + "</expected>");
              writer.WriteLine("<output>" + Path.ChangeExtension(confFile.FullName, ".html") + "</output>");
              writer.WriteLine("</conformance>");

            }
          }


        } catch (Exception e) {
          writer.WriteLine("<exception>" + Escape(e.Message) + "</exception>");
          fatalCount++;
        }
      } finally {
        if ((((expectError || optionalError) && errorCount > 0) ||
             (!expectError && errorCount == 0)) &&
            (((expectFatalError || optionalFatalError) && fatalCount > 0) ||
             (!expectFatalError && fatalCount == 0))) {
          writer.WriteLine("<pass>true</pass>");
          passCount++;
        } else {
          writer.WriteLine("<pass>false</pass>");
          failCount++;
        }
        writer.WriteLine("</TEST>");
      }
    }

    /// <summary>
    /// Writes the end tag for the test and does any cleanup
    /// </summary>
    public void FinalizeTest() {
      writer.WriteLine("<duration type=\"seconds\">" + totalDur.TotalSeconds + "</duration>");
      writer.WriteLine("</results>");
    }

    /// <summary>
    /// Desinged to take an input string and escape any reserved XML Characters such as &amp;
    /// &lt; &gt; etc. Clearly this function is not designed for speed.
    /// </summary>
    /// <param name="text">The input text to be escaped</param>
    /// <returns>The escaped string, ready to be inserted into the output XML</returns>
    public string Escape(string text) {
      escaper.Length = 0;
      for (int i = 0; i < text.Length; i++) {
        char ch = text[i];
        switch (ch) {
          case '&':
            escaper.Append("&amp;");
            break;
          case '<':
            escaper.Append("&lt;");
            break;
          case '>':
            escaper.Append("&gt;");
            break;
          default:
            if (((int)ch < 32 || (int)ch > 127) && (int)ch != 10 && (int)ch != 13)
              escaper.Append('?');
            else
              escaper.Append(ch);
            break;
        }
      }
      return escaper.ToString();
    }

    #region IContentHandler Members

    public void SetDocumentLocator(ILocator locator) {
      this.testLoc = locator;
    }

    public void StartDocument() {
      if (console)
        Console.WriteLine("Running tests in file " + testLoc.SystemId);
    }

    private static string GetAttValue(IAttributes atts, string name)
    {
      string value;
      int indx = atts.GetIndex(name);
      if (indx >= 0)
        value = atts.GetValue(indx);
      else
        value = null;
      return value;
    }

    public void StartElement(string uri, string localName, string qName, IAttributes atts) {
      if (localName.Equals("TEST")) {
        inTest = true;
        currId = atts.GetValue("ID");
        currSections = atts.GetValue("SECTIONS");
        currType = atts.GetValue("TYPE");
        currUri = atts.GetValue("URI");
        currEntities = GetAttValue(atts, "ENTITIES");
        currRecommendation = GetAttValue(atts, "RECOMMENDATION");
        currVersion = GetAttValue(atts, "VERSION");
        currNamespaces = GetAttValue(atts, "NAMESPACE");
        currContent.Length = 0;
        return;
      }
      if (localName.Equals("TESTCASES")) {
        currProfile = atts.GetValue("PROFILE");
        // string baseUriStr = GetAttValue(atts, "xml:base");
        if (console)
          Console.WriteLine("\n=======================================================================\n" +
            currProfile +
            "\n=======================================================================\n");
        return;
      }
      if (localName.Equals("TESTSUITE")) {
        if (console)
          Console.WriteLine("\nTest suite for " + atts.GetValue("PROFILE"));
        return;
      }
      if (inTest)
        currContent.Append("<" + qName + ">");
    }

    public void Characters(char[] ch, int start, int length) {
      if (inTest)
        currContent.Append(ch, start, length);
    }

    public void EndElement(string uri, string localName, string qName) {
      if (localName.Equals("TEST")) {
        inTest = false;
        // create new instance of same parser type
        Type readerType = reader.GetType();
        BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
        ConstructorInfo ci = readerType.GetConstructor(flags, null, Type.EmptyTypes, null);
        IXmlReader parser = (IXmlReader)ci.Invoke(null);
        RunTest(parser);
        return;
      }
      if (localName.Equals("TESTCASES")) {
        return;
      }
      if (localName.Equals("TESTSUITE"))
        return;
      if (inTest)
        currContent.Append("</" + qName + ">");
    }

    public void EndDocument() {
      totalDur = DateTime.Now - startTime;
      if (console)
      {
        Console.WriteLine("\nFiles Passed: " + passCount);
        Console.WriteLine("\nFiles Failed: " + failCount);
        if (saxExpected != null)
          Console.WriteLine("\nFiles Failing Conformance: " + conformanceFailCount);
        Console.WriteLine("\nFiles Not Supported: " + unsupportedCount);
        Console.WriteLine("\nExecution Time: " + totalDur.TotalSeconds + " seconds");
      }
    }

    public void SkippedEntity(string name) {}
    public void IgnorableWhitespace(char[] ch, int start, int length) { }
    public void StartPrefixMapping(string prefix, string uri) { }
    public void EndPrefixMapping(string prefix) { }
    public void ProcessingInstruction(string target, string data) {}

    #endregion

    #region IErrorHandler Members

    public void FatalError(ParseError error) {
      writer.WriteLine("<fatalError line='" + error.LineNumber + "' column='" + error.ColumnNumber + "'>"+
         Escape(error.Message) + "</fatalError>");
      fatalCount++;
    }

    public void Error(ParseError error) {
      writer.WriteLine("<error line='" + error.LineNumber + "' column='" + error.ColumnNumber + "'>"+
        Escape(error.Message) + "</error>");
      errorCount++;
    }

    public void Warning(ParseError error)
    {
      writer.WriteLine("<warning line='" + error.LineNumber + "' column='" + error.ColumnNumber + "'>"+
        Escape(error.Message) + "</warning>");
      warningCount++;
    }

    #endregion
  }
}
