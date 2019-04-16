using System;
using System.Text;
using System.IO;
using System.Xml;
using System.Collections;
using Org.System.Xml.Sax;
using Org.System.Xml.Sax.Helpers;

namespace XmlConf
{
	/// <summary>
	/// ConformanceReportHandler is an implementation of Elliotte Rusty Harold's SAXTest conformance
	/// algorithms. The output of this class is intended to be compatabile with his
	/// Java classes which can be found at http://www.cafeconleche.org/SAXTest.
	/// Specific implementation choices for SAX for .NET will obviously render
	/// incompatible outputs. In such cases, the SAX for .NET reference implementation
	/// will be considered correct and not the Xerces reference tests.
	/// </summary>
  public class ConformanceReportHandler : IContentHandler, IErrorHandler, IDtdHandler, IEntityResolver
  {

    private bool inProlog;
    private bool seenEvent;
    private XmlTextWriter writer;
    private SortedList startPrefixMappings;
    private SortedList endPrefixMappings;
    private SortedList notations;
    private SortedList entities;
    private ILocator locator;
    private string uri;
    private TestSuite parentTestSuite;

    public ConformanceReportHandler (XmlTextWriter writer, TestSuite parentTestSuite)
    {
      this.seenEvent = false;
      this.inProlog = true;
      this.writer = writer;
      this.notations = new SortedList();
      this.entities = new SortedList();
      this.parentTestSuite = parentTestSuite;
    }

    public void Initialize()
    {
      writer.WriteStartDocument();
      writer.WriteStartElement("ConformanceResults");
    }

    public void Finish()
    {
      writer.WriteEndElement();
      writer.WriteEndDocument();
    }

    #region IContentHandler Members

    public void StartDocument()
    {
      seenEvent = true;
      writer.WriteStartElement("startDocument");
      writer.WriteEndElement();
    }

    public void SkippedEntity(string name)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      writer.WriteStartElement("skippedEntity");
      writer.WriteStartElement("name");
      writer.WriteString(name);
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    public void StartElement(string uri, string localName, string qName, IAttributes atts)
    {
      seenEvent = true;
      if (inProlog)
      {
        FlushProlog();
        inProlog = false;
      }

      FlushEndPrefixMappings();
      if (startPrefixMappings != null)
      {
        for (int i = 0; i < startPrefixMappings.Count; i++)
        {
          string[] mapping = (string[])startPrefixMappings.GetByIndex(i);
          writer.WriteStartElement("startPrefixMapping");
          writer.WriteStartElement("prefix");
          writer.WriteString(mapping[0]);
          writer.WriteEndElement();
          writer.WriteStartElement("data");
          writer.WriteString(mapping[1]);
          writer.WriteEndElement();
          writer.WriteEndElement();
        }
        startPrefixMappings = null;
      }

      writer.WriteStartElement("startElement");

      if (uri != null)
      {
        writer.WriteStartElement("namespaceURI");
        writer.WriteString(uri);
        writer.WriteEndElement();
      }
      if (localName != null)
      {
        writer.WriteStartElement("localName");
        writer.WriteString(localName);
        writer.WriteEndElement();
      }
      if (qName != null)
      {
        writer.WriteStartElement("qualifiedName");
        writer.WriteString(qName);
        writer.WriteEndElement();
      }

      writer.WriteStartElement("attributes");
      SortedList sortedAtts = new SortedList();
      for (int i = 0; i < atts.Length; i++)
      {
        string ln = atts.GetLocalName(i);
        string qn = atts.GetQName(i);
        string ns = atts.GetUri(i);
        string key = "";
        if (ln != null) key += ln;
        key += '\u0000';
        if (qn != null) key += qn;
        key += '\u0000';
        if (ns != null) key += ns;
        sortedAtts.Add(key, i);
      }

      for (int i = 0; i < sortedAtts.Count; i++)
      {
        int index = (int)sortedAtts.GetByIndex(i);
        string ln = atts.GetLocalName(i);
        string qn = atts.GetQName(i);
        string ns = atts.GetUri(i);
        string val = atts.GetValue(i);
        string typ = atts.GetType(i);
        writer.WriteStartElement("attribute");

        if (ns != null)
        {
          writer.WriteStartElement("namespaceURI");
          writer.WriteString(ns);
          writer.WriteEndElement();
        }
        if (ln != null)
        {
          writer.WriteStartElement("localName");
          writer.WriteString(ln);
          writer.WriteEndElement();
        }
        if (qn != null)
        {
          writer.WriteStartElement("qualifiedName");
          writer.WriteString(qn);
          writer.WriteEndElement();
        }
        writer.WriteStartElement("value");
        writer.WriteString(Escape(val));
        writer.WriteEndElement();
        writer.WriteStartElement("type");
        writer.WriteString(Escape(typ));
        writer.WriteEndElement();

        writer.WriteEndElement();
      }
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    public void EndPrefixMapping(string prefix)
    {
      seenEvent = true;
      if (endPrefixMappings == null)
        endPrefixMappings = new SortedList();
      endPrefixMappings.Add(prefix, null);
    }

    public void SetDocumentLocator(ILocator locator)
    {
      // This is optional; mostly for debugging;
      // However, if it is called it must be the first thing called;
      // even before startDocument
      this.locator = locator;
      this.uri = locator.SystemId;
      // Only note this if it's in the wrong place, so that the
      // comparison will fail.
      if (seenEvent)
      {
        writer.WriteStartElement("locator");
        writer.WriteEndElement();
      }
      // Do this at the end because we use it in this function
      seenEvent = true;
    }

    public void EndElement(string uri, string localName, string qName)
    {
      seenEvent = true;
      writer.WriteStartElement("endElement");

      if (uri != null)
      {
        writer.WriteStartElement("namespaceURI");
        writer.WriteString(uri);
        writer.WriteEndElement();
      }
      if (localName != null)
      {
        writer.WriteStartElement("localName");
        writer.WriteString(localName);
        writer.WriteEndElement();
      }
      if (qName != null)
      {
        writer.WriteStartElement("qualifiedName");
        writer.WriteString(qName);
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
    }

    public void EndDocument()
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      writer.WriteStartElement("endDocument");
      writer.WriteEndElement();
    }

    public void Characters(char[] ch, int start, int length)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      for (int i = start; i < start+length; i++)
      {
        writer.WriteStartElement("char");
        writer.WriteString(Escape(ch[i]));
        writer.WriteEndElement();
      }
    }

    public void IgnorableWhitespace(char[] ch, int start, int length)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      for (int i = start; i < start+length; i++)
      {
        writer.WriteStartElement("ignorable");
        writer.WriteString(Escape(ch[i]));
        writer.WriteEndElement();
      }
    }

    public void StartPrefixMapping(string prefix, string uri)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      string[] mapping = new string[2];
      mapping[0] = prefix;
      mapping[1] = uri;
      if (startPrefixMappings == null)
        startPrefixMappings = new SortedList();
      startPrefixMappings.Add(prefix + '\u0000' + uri, mapping);
    }

    public void ProcessingInstruction(string target, string data)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      writer.WriteStartElement("processingInstruction");
      writer.WriteStartElement("target");
      writer.WriteString(target);
      writer.WriteEndElement();
      writer.WriteStartElement("data");
      writer.WriteString(Escape(data));
      writer.WriteEndElement();
      writer.WriteEndElement();
    }

    #endregion

    #region IErrorHandler Members

    public void FatalError(ParseError error)
    {
      if (parentTestSuite != null)
        parentTestSuite.FatalError(error);

      seenEvent = true;
      FlushEndPrefixMappings();
      writer.WriteStartElement("fatalError");
      writer.WriteEndElement();
    }

    public void Warning(ParseError error)
    {
      // warnings are optional
      if (parentTestSuite != null)
        parentTestSuite.Warning(error);
    }

    public void Error(ParseError error)
    {
      // reporting errors is optional
      if (parentTestSuite != null)
        parentTestSuite.Error(error);
    }

    #endregion

    #region IDtdHandler Members

    public void UnparsedEntityDecl(string name, string publicId, string systemId, string notationName)
    {
      seenEvent = true;
      string[] unparsed = new string[4];
      unparsed[0] = name;
      unparsed[1] = publicId;
      unparsed[2] = systemId;
      unparsed[3] = notationName;
      if (inProlog)
      {
        entities.Add(name, unparsed);
      }
      else
      {
        writer.WriteStartElement("bug");
        writer.WriteAttributeString("reason", "unparsed entity reported after first startElement");
        writer.WriteStartElement("unparsedEntity");
        if (unparsed[0] != null)
        {
          writer.WriteStartElement("name");
          writer.WriteString(unparsed[0]);
          writer.WriteEndElement();
        }
        if (unparsed[1] != null)
        {
          writer.WriteStartElement("publicID");
          writer.WriteString(unparsed[1]);
          writer.WriteEndElement();
        }
        if (unparsed[2] != null)
        {
          writer.WriteStartElement("systemID");
          writer.WriteString(unparsed[2]);
          writer.WriteEndElement();
        }
        if (unparsed[3] != null)
        {
          writer.WriteStartElement("notation");
          writer.WriteString(unparsed[3]);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    public void NotationDecl(string name, string publicId, string systemId)
    {
      seenEvent = true;
      string[] notation = new string[3];
      notation[0] = name;
      notation[1] = publicId;
      notation[2] = systemId;
      if (inProlog)
      {
        notations.Add(name, notation);
      }
      else
      {
        writer.WriteStartElement("bug");
        writer.WriteAttributeString("reason", "notation reported after first startElement");
        writer.WriteStartElement("notation");
        if (notation[0] != null)
        {
          writer.WriteStartElement("name");
          writer.WriteString(notation[0]);
          writer.WriteEndElement();
        }
        if (notation[1] != null)
        {
          writer.WriteStartElement("publicID");
          writer.WriteString(notation[1]);
          writer.WriteEndElement();
        }
        if (notation[2] != null)
        {
          writer.WriteStartElement("systemID");
          writer.WriteString(notation[2]);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
        writer.WriteEndElement();
      }
    }

    #endregion

    #region IEntityResolver Members

    public InputSource GetExternalSubset(string name, string baseUri)
    {
      return null;
    }

    public InputSource ResolveEntity(string name, string publicId, string baseUri, string systemId)
    {
      seenEvent = true;
      FlushEndPrefixMappings();
      writer.WriteStartElement("resolveEntity");
      if (publicId != null)
      {
        writer.WriteStartElement("publicID");
        writer.WriteString(Escape(publicId));
        writer.WriteEndElement();
      }
      if (systemId != null)
      {
        string sysId = systemId;
        if (baseUri != null)
          try {
            Uri bsUri = new Uri(baseUri);
            Uri sysUri = new Uri(bsUri, systemId);
            sysId = sysUri.AbsoluteUri;
          }
          catch (Exception e) {
            if (parentTestSuite != null)
              parentTestSuite.Error(new ParseErrorImpl("Entity '" + name + "': External id error", e));
          }
        writer.WriteStartElement("systemID");
        writer.WriteString(Escape(sysId));
        writer.WriteEndElement();
      }
      writer.WriteEndElement();
      return null;
    }

    #endregion


    private void FlushEndPrefixMappings()
    {
      if (endPrefixMappings != null)
      {
        for (int i = 0; i < endPrefixMappings.Count; i++)
        {
          string prefix = (string)endPrefixMappings.GetKey(i);
          writer.WriteStartElement("endPrefixMapping");
          writer.WriteStartElement("prefix");
          writer.WriteString(prefix);
          writer.WriteEndElement();
          writer.WriteEndElement();
        }
        endPrefixMappings = null;
      }
    }

    private void FlushProlog()
    {

      for (int i = 0; i < notations.Count; i++)
      {
        string[] notation = (string[])notations.GetByIndex(i);
        writer.WriteStartElement("notation");
        if (notation[0] != null)
        {
          writer.WriteStartElement("name");
          writer.WriteString(Escape(notation[0]));
          writer.WriteEndElement();
        }
        if (notation[1] != null)
        {
          writer.WriteStartElement("publicID");
          writer.WriteString(Escape(notation[1]));
          writer.WriteEndElement();
        }
        if (notation[2] != null)
        {
          writer.WriteStartElement("systemID");
          writer.WriteString(Escape(notation[2]));
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }

      for (int i = 0; i < entities.Count; i++)
      {
        string[] unparsed = (string[])entities.GetByIndex(i);
        writer.WriteStartElement("unparsedEntity");
        if (unparsed[0] != null)
        {
          writer.WriteStartElement("name");
          writer.WriteString(unparsed[0]);
          writer.WriteEndElement();
        }
        if (unparsed[1] != null)
        {
          writer.WriteStartElement("publicID");
          writer.WriteString(unparsed[1]);
          writer.WriteEndElement();
        }
        if (unparsed[2] != null)
        {
          writer.WriteStartElement("systemID");
          writer.WriteString(unparsed[2]);
          writer.WriteEndElement();
        }
        if (unparsed[3] != null)
        {
          writer.WriteStartElement("notation");
          writer.WriteString(unparsed[3]);
          writer.WriteEndElement();
        }
        writer.WriteEndElement();
      }
    }

    private string Escape(string text)
    {
      StringBuilder sb = new StringBuilder();
      for (int i = 0; i < text.Length; i++)
      {
        sb.Append(Escape(text[i]));
      }
      return sb.ToString();
    }

    private string Escape(char c)
    {
      try
      {
        StringBuilder sb = new StringBuilder(1, 10);
        switch (c)
        {
          case ' ':
            sb.Append("\\s");
            break;
          case '\n':
            sb.Append("\\n");
            break;
          case '\r':
            sb.Append("\\r");
            break;
          case '\t':
            sb.Append("\\t");
            break;
          case '\\':
            sb.Append("\\\\");
            break;
            /*
                      case 0xFFFE:
                        sb.Append("\\uFFFE");
                        break;
                      case 0xFFFF:
                        sb.Append("\\uFFFF");
                        break;
            */
          default:
            if (c < ' ')
            {
              sb.Append("\\u" + ((uint)c).ToString("X4").ToUpper());
            }
            else if (c >= 127 && c <= 160 || c >= 0xD800 && c < 0xE000)
            {
              // surrogate or private use character
              // not legal in XML but they are tested for
              sb.Append("\\u" + ((uint)c).ToString("X4").ToUpper());
            }
            else sb.Append(c);
            break;
        }
        return sb.ToString();
      }
      catch (Exception)
      {
        return c.ToString();
      }
    }
  }

  /// <summary>
  /// <p>ResultComparer is an implementation of Elliotte Rusty Harold's SAXTest conformance
  /// algorithms. The output of this class is intended to be compatabile with his
  /// Java classes which can be found at http://www.cafeconleche.org/SAXTest.
  /// Specific implementation choices for SAX for .NET will obviously render
  /// incompatible results. In such cases, the SAX for .NET reference implementation
  /// will be considered correct and not the Xerces reference tests.</p>
  /// <p>
  /// Another important difference is that this class has been designed with the
  /// existing XmlConf structure in mind. For this reason this implementation will
  /// only compare one set of tests. It is intended that this class be created once and called
  /// repeatedly.
  /// </p>
  ///
  /// </summary>
  public class ResultComparer
  {

    public ResultComparer ()
    {

    }

    public bool compare(string expectedUri, string actualUri, out string errorMessage)
    {
      errorMessage = "";

      // .NET only, check if the output files were created, if not the test was skipped
      // in which case this should not have been called anyway
      if (!File.Exists(expectedUri) && !File.Exists(actualUri))
        return true;

      XmlDocument expected = null;
      XmlDocument actual = null;
      try
      {
        expected = new XmlDocument();
        expected.Load(expectedUri);
        actual = new XmlDocument();
        actual.Load(actualUri);
        return compare(expected, actual);
      }
      catch(ApplicationException e)
      {
        //!!Console.Error.WriteLine("\n[Conformance Error] " + e.Message);
        errorMessage = e.Message;
        expected = null;
        actual = null;
        return false;
      }
    }

    private bool compare(XmlDocument expectedDoc, XmlDocument actualDoc)
    {
      XmlElement actualRoot = actualDoc.DocumentElement;
      XmlElement expectedRoot = expectedDoc.DocumentElement;
      XmlNodeList actualChildren = actualRoot.SelectNodes("*");
      XmlNodeList expectedChildren = expectedRoot.SelectNodes("*");

      // She's actual size but she seems much bigger to me...
      int actualSize = actualChildren.Count;
      int expectedSize = expectedChildren.Count;

      if (actualSize == 0)
        throw new ApplicationException("Actual document and expected document do not contain the same number of children");

      if (!"startDocument".Equals(actualChildren[0].Name))
        throw new ApplicationException("Actual document first element must be \"startDocument\"");

      // .NET specific check
      if (!"endDocument".Equals(actualChildren[actualSize-1].Name))
        throw new ApplicationException("Actual document last element must be \"endDocument\"");

      // Compare notations
      XmlNodeList actualNotations = actualRoot.SelectNodes("notation");
      XmlNodeList expectedNotations = expectedRoot.SelectNodes("notation");
      if (actualNotations.Count != expectedNotations.Count)
      {
        if (isDoubleFatality(expectedChildren, actualChildren))
          throw new ApplicationException("Actual document and expected document do not contain the same number of notations (double fatality)");
        else
          throw new ApplicationException("Actual document and expected document do not contain the same number of notations");
      }
      for (int i = 0; i < actualNotations.Count; i++)
      {
        if (!containsNotation(expectedNotations, (XmlElement)actualNotations[i]))
        {
          XmlNode actualNotationNameNode = ((XmlElement)actualNotations[i]).SelectSingleNode("name");
          string actualNotationName = (actualNotationNameNode == null) ? "" : actualNotationNameNode.InnerText;
          if (isDoubleFatality(expectedChildren, actualChildren))
            throw new ApplicationException("Actual document notation \"" + actualNotationName + "\" not contained within expected notations (double fatality)");
          else
            throw new ApplicationException("Actual document notation \"" + actualNotationName + "\" not contained within expected notations");
        }
      }

      // Compare entities
      XmlNodeList actualEntities = actualRoot.SelectNodes("unparsedEntity");
      XmlNodeList expectedEntities = expectedRoot.SelectNodes("unparsedEntity");
      if (actualEntities.Count != expectedEntities.Count)
      {
        if (isDoubleFatality(expectedChildren, actualChildren))
          throw new ApplicationException("Actual document and expected document do not contain the same number of unparsed entities (double fatality)");
        else
          throw new ApplicationException("Actual document and expected document do not contain the same number of unparsed entities");
      }
      for (int i = 0; i < actualEntities.Count; i++)
      {
        if (!containsEntity(expectedEntities, (XmlElement)actualEntities[i]))
        {
          XmlNode actualEntityNameNode = ((XmlElement)actualEntities[i]).SelectSingleNode("name");
          string actualEntityName = (actualEntityNameNode == null) ? "" : actualEntityNameNode.InnerText;
          throw new ApplicationException("Actual document unparsed entity \"" + actualEntityName + "\" not contained within expected unparsed entities");
        }
      }



      int expected = 1; // 0 = startDocument

      // Skip over the entities and notations
      while (true)
      {
        XmlElement e = (XmlElement)expectedChildren[expected];
        string eName = e.LocalName;
        if (eName.Equals("notation") || eName.Equals("unparsedEntitiy"))
          expected++;
        else
          break;
      }

      int actual = expected;

      while (expected < expectedSize && actual < actualSize)
      {
        XmlElement nextActual = (XmlElement)actualChildren[actual++];
        XmlElement nextExpected = (XmlElement)expectedChildren[expected++];
        if (nextExpected.LocalName.Equals("ignorable"))
        {
          if ((nextActual.LocalName.Equals("char") || nextActual.LocalName.Equals("ignorable") &&
            nextActual.InnerText == nextExpected.InnerText))
            continue;
          else
            throw new ApplicationException("Actual document value \"" + nextActual.InnerText + "\" in the element \"" + nextActual.LocalName +
              "\" does not match the expected value \"" + nextExpected.InnerText + "\" in the element \"" + nextExpected.LocalName);
        }

        // At this point Elliotte uses XOMTestCase to assertEquals...
        // not sure what that is-- we test what seems important I guess...
        // attributes are only used on <bug> elements, so not important
        if (!nextExpected.LocalName.Equals("systemID"))
        {
          if (!nextExpected.LocalName.Equals(nextActual.LocalName) ||
            !nextExpected.NamespaceURI.Equals(nextActual.NamespaceURI) ||
            (!nextExpected.HasChildNodes && nextExpected.InnerText != nextActual.InnerText))
          {
            if (nextActual.LocalName.Equals("fatalError"))
            {
              // The last element should always be endDocument
              XmlElement expectedPenultimate = (XmlElement)expectedChildren[expectedSize-2];
              if ("fatalError".Equals(expectedPenultimate.LocalName))
                return true;
            }

            if (!nextExpected.LocalName.Equals(nextActual.LocalName))
              throw new ApplicationException("Actual document element \"" + nextActual.LocalName + "\" does not match the expected document element \"" + nextExpected.LocalName + "\"");
            else if (!nextExpected.NamespaceURI.Equals(nextActual.NamespaceURI))
              throw new ApplicationException("Actual document element \"{" + nextActual.NamespaceURI + "}" + nextActual.LocalName + "\" does not match the expected document element \"{" + nextExpected.NamespaceURI + "}" + nextExpected.LocalName + "\"");
            else if (!nextExpected.HasChildNodes && nextExpected.InnerText != nextActual.InnerText)
              throw new ApplicationException("Actual document element \"" + nextActual.LocalName + "\" has content \"" + nextActual.InnerText + "\" that does not match the expected content \"" + nextExpected.InnerText + "\"");
          }
        }
      }

      if (expectedSize != actualSize)
      {
        if (isDoubleFatality(expectedChildren, actualChildren))
          throw new ApplicationException("Actual document size does not match the expected document size (double fatality)");
        else
          throw new ApplicationException("Actual document size does not match the expected document size");
      }

      return true;
    }

    private bool isDoubleFatality(XmlNodeList expected, XmlNodeList actual)
    {
      try
      {
        // We only have to check the penultimate-- SAX for .NET requires endDocument
        return expected[expected.Count-2].LocalName.Equals("fatalError") &&
               actual[actual.Count-2].LocalName.Equals("fatalError");
      }
      catch
      {
        return false;
      }
    }

    private bool containsNotation(XmlNodeList expectedNotations, XmlElement actual)
    {
      for (int i = 0; i < expectedNotations.Count; i++)
      {
        if (compareByValue(expectedNotations[i].SelectSingleNode("name"), actual.SelectSingleNode("name")))
        {
          if (compareByValue(expectedNotations[i].SelectSingleNode("publicID"), actual.SelectSingleNode("publicID")))
          {
            XmlNode systemExpected = expectedNotations[i].SelectSingleNode("systemID");
            XmlNode systemActual = actual.SelectSingleNode("systemID");

            // Can't use compareByValue because we need to manipulate the URL value
            if (systemExpected == systemActual)
              return true;
            else if (systemExpected == null || systemActual == null)
              return false;

            string expectedSystemId = ((XmlElement)systemExpected).InnerText;
            string actualSystemId = ((XmlElement)systemActual).InnerText;

            // Handle incompatible file:/// and file:/
            expectedSystemId = expectedSystemId.Replace("file:///", "file:/");
            expectedSystemId = expectedSystemId.Replace("file://", "file:/");
            actualSystemId = actualSystemId.Replace("file:///", "file:/");
            actualSystemId = actualSystemId.Replace("file://", "file:/");

            //.NET Try to delete trailing slashes
            if (expectedSystemId.EndsWith("/") || expectedSystemId.EndsWith("\\"))
              expectedSystemId = expectedSystemId.Substring(0, expectedSystemId.Length-1);
            if (actualSystemId.EndsWith("/") || actualSystemId.EndsWith("\\"))
              actualSystemId = actualSystemId.Substring(0, actualSystemId.Length-1);

            if (expectedSystemId.Equals(actualSystemId))
              return true;

            //.NET test for relative part modified
            int index = expectedSystemId.IndexOf("xmlconf/");
            if (index > 0)
            {
              string relPart = expectedSystemId.Substring(index);
              if (actualSystemId.EndsWith(relPart))
                return true;
            }
          }
        }
      }
      return false;
    }

    private bool containsEntity(XmlNodeList expectedEntities, XmlElement actual)
    {
      for (int i = 0; i < expectedEntities.Count; i++)
      {
        if (compareByValue(expectedEntities[i].SelectSingleNode("name"), actual.SelectSingleNode("name")))
        {
          if (compareByValue(expectedEntities[i].SelectSingleNode("publicID"), actual.SelectSingleNode("publicID")))
          {
            XmlNode systemExpected = expectedEntities[i].SelectSingleNode("systemID");
            XmlNode systemActual = actual.SelectSingleNode("systemID");

            if (systemActual == null)
              return false;

            string expectedSystemId = ((XmlElement)systemExpected).InnerText;
            string actualSystemId = ((XmlElement)systemActual).InnerText;

            // Handle incompatible file:/// and file:/
            expectedSystemId = expectedSystemId.Replace("file:///", "file:/");
            expectedSystemId = expectedSystemId.Replace("file://", "file:/");
            actualSystemId = actualSystemId.Replace("file:///", "file:/");
            actualSystemId = actualSystemId.Replace("file://", "file:/");

            //.NET Try to delete trailing slashes
            if (expectedSystemId.EndsWith("/") || expectedSystemId.EndsWith("\\"))
              expectedSystemId = expectedSystemId.Substring(0, expectedSystemId.Length-1);
            if (actualSystemId.EndsWith("/") || actualSystemId.EndsWith("\\"))
              actualSystemId = actualSystemId.Substring(0, actualSystemId.Length-1);

            if (expectedSystemId.Equals(actualSystemId))
              return true;

            //.NET test for relative part modified
            int index = expectedSystemId.IndexOf("xmlconf/");
            if (index > 0)
            {
              string relPart = expectedSystemId.Substring(index);
              if (actualSystemId.EndsWith(relPart))
              {
                if (compareByValue(expectedEntities[i].SelectSingleNode("notation"), actual.SelectSingleNode("notation")))
                  return true;
              }
            }
          }
        }
      }
      return false;
    }

    private bool compareByValue(XmlNode expected, XmlNode actual)
    {
      if (expected == actual)
        return true;
      else if (expected == null || actual == null)
        return true;
      else
        return ((XmlElement)expected).InnerText == ((XmlElement)actual).InnerText;
    }


    public void makeResultDoc(string testUri, string expectedUri, string actualUri,
      string message, string id, bool passed, string errorMessage)
    {
      string outputFileName = Path.ChangeExtension(actualUri, ".html");
      string title = "Test Case " + id + (passed ? ": Passed" : ": Failed");

      StreamWriter sw = new StreamWriter(outputFileName);
      sw.AutoFlush = true;
      XmlTextWriter wri = new XmlTextWriter(sw);
      wri.Formatting = Formatting.Indented;

      wri.WriteStartElement("html");
      wri.WriteStartElement("head");
      wri.WriteStartElement("title");
      wri.WriteString(title);
      wri.WriteEndElement();
      wri.WriteEndElement();
      wri.WriteStartElement("body");
      wri.WriteStartElement("h1");
      wri.WriteString(title);
      wri.WriteEndElement();
      if (!passed)
      {
        wri.WriteStartElement("blockquote");
        wri.WriteAttributeString("style", "color:red;");
        wri.WriteString(errorMessage);
        wri.WriteEndElement();
      }
      wri.WriteStartElement("p");
      wri.WriteString(message);
      wri.WriteEndElement();

      try
      {
        StreamReader testText = new StreamReader(testUri);
        wri.WriteStartElement("pre");
        wri.WriteString(testText.ReadToEnd());
        wri.WriteEndElement();
      }
      catch (Exception e)
      {
        wri.WriteStartElement("blockquote");
        wri.WriteString("Unable to display orginal test : " + e.Message);
        wri.WriteEndElement();
      }

      wri.WriteStartElement("table");
      wri.WriteAttributeString("border", "1px");

      wri.WriteStartElement("tr");

      wri.WriteStartElement("th");
      wri.WriteString("Expected Result");
      wri.WriteEndElement();

      wri.WriteStartElement("th");
      wri.WriteString("Actual Result");
      wri.WriteEndElement();

      wri.WriteEndElement();

      wri.WriteStartElement("tr");

      wri.WriteStartElement("td");
      wri.WriteAttributeString("valign", "top");
      wri.WriteStartElement("pre");
      wri.WriteAttributeString("style", "width:400px;overflow:scroll");
      StreamReader expectedText = new StreamReader(expectedUri);
      wri.WriteString(expectedText.ReadToEnd());
      wri.WriteEndElement();
      wri.WriteEndElement();

      wri.WriteStartElement("td");
      wri.WriteAttributeString("valign", "top");
      wri.WriteStartElement("pre");
      wri.WriteAttributeString("style", "width:400px;overflow:scroll;");
      StreamReader actualText = new StreamReader(actualUri);
      wri.WriteString(actualText.ReadToEnd());
      wri.WriteEndElement();
      wri.WriteEndElement();

      wri.WriteEndElement();
      wri.WriteEndElement();

      wri.WriteEndElement();
      wri.WriteEndElement();
    }

  }

}
