<?xml version="1.0" encoding="UTF-8"?>
<xsl:stylesheet xmlns:xsl="http://www.w3.org/1999/XSL/Transform" version="1.0">
  <xsl:param name="showOnlyFail" select="false()"/>
  <xsl:param name="showUri" select="false()"/>
  <xsl:output method="html" indent="yes" encoding="ISO-8859-1"/>
  <xsl:template match="/results">
    <html xmlns="http://www.w3.org/1999/xhtml">
      <head>
        <title>XML Conformance Tests</title>
        <meta http-equiv="Content-Type" content="text/html;charset=utf-8"/>
        <style type="text/css">tr.premiere { background-color: #BEDCE6; }
          th { text-align: left; vertical-align: top }
          .editor { color: red; }
          .countdown { color: white; background-color: red; }
          blockquote, q { font-family: Arial, Helvetica, sans-serif; }
          .quote     { font-family: Arial, Helvetica, sans-serif; }
          re.quote { margin-left: 2.5em; }
          ol.quote     { font-family: Arial, Helvetica, sans-serif; margin-left: 3em; }
          .diff-add { background-color: yellow; }
          .diff-chg { background-color: lime; }
          .diff-del { text-decoration: line-through; }</style>
      </head>
      <body bgcolor="#ffffff">
        <a name="listings"/>
        <h2>Test Case Descriptions</h2>
        <p>This section of this report contains descriptions of test
           cases, each of which fits into the categories noted above.
           Each test case includes a document of one of the types in the
           binary test matrix above (e.g. valid or invalid documents).</p>



        <p>In some cases, an <a href="#output">output file</a>, as
           described in Section 2.2,  will also be associated with
           a valid document, which is used for output testing.  If such
           a file exists, it will be noted at the end of the description
           of the input document.</p>





        <p>The description for each test case is presented as a two
           part table.  The right part describes what the test does.
           This description is intended to have enough detail to evaluate
           diagnostic messages.  The left part includes:
           <ul>
               <li>An entry describing the <em>Sections and/or Rules</em> from
                   the <a href="http://www.w3.org/TR/2000/REC-xml-20001006">XML 1.0
                   (Second Edition) Recommendation</a> which this case excercises.</li>
               <li>The unique <em>Test ID</em> within a given <em>Collection</em> for this test.</li>
               <li>The <em>Collection</em> from which this test originated.
                   Given the <em>Test ID</em> and the <em>Collection</em>, each
                   test can be uniquely identified.</li>
               <li>Some tests may have a field identifying the kinds of
                   external <em>Entities</em> a nonvalidating processor must
                   include (parameter, general, or both) to be able to
                   detect any errors in that test case.</li>
           </ul>
        </p>





        <a name="valid"/>
        <h3>3.1  Valid XML Documents</h3>
        <p>All conforming <em>XML 1.0 Processors</em> are <b>required</b> to accept
           valid documents, reporting no errors. In this section of this test report
           are found descriptions of test cases which fit into this category.</p>



        <xsl:apply-templates select="//TEST[@TYPE='valid']">
          <xsl:sort select="@SECTIONS"/>
        </xsl:apply-templates>
        <a name="invalid"/>
        <h3>3.2  Invalid XML Documents</h3>
        <p>All conforming XML 1.0 <em>Validating Processors</em> are <b>required</b>
           to report recoverable errors in the case
           of documents which are <em>Invalid</em>.  Such errors are
           violations of some <em>validity constraint (VC)</em>.</p>


        <p>If a validating processor does not report an error when
           given one of these test cases, or if the error reported is
           a fatal error, it is not conformant.  If the error reported
           does not correspond to the problem listed in this test
           description, that could also be a conformance problem; it
           might instead be a faulty diagnostic.</p>





        <p>All conforming XML 1.0 <em>Nonvalidating Processors</em>
           should accept these documents, reporting no errors.</p>
        <xsl:apply-templates select="//TEST[@TYPE='invalid']">
          <xsl:sort select="@SECTIONS"/>
        </xsl:apply-templates>
        <a name="not-wf"/>
        <h3>3.3  Documents that are Not Well Formed</h3>
        <p>All conforming XML 1.0 Processors are <b>required</b> to
           report fatal errors in the case of documents which are not
           <em>Well Formed</em>. Such errors are basically of two types:
           <em>(a)</em> the document violates the XML grammar; or else
           <em>(b)</em> it violates a <em>well formedness constraint (WFC)</em>.
           There is a single <em>exception to that requirement</em>:
           nonvalidating processors which do not read
           certain types of external entities are not required to detect
           (and hence report) these errors.</p>






        <p>If a processor does not report a fatal error when given
           one of these test cases, it is not conformant.  If the error
           reported does not correspond to the problem listed in this
           test description, that could also be a conformance problem;
           it might instead be a faulty diagnostic.</p>




        <xsl:apply-templates select="//TEST[@TYPE='not-wf']">
          <xsl:sort select="@SECTIONS"/>
        </xsl:apply-templates>
        <a name="error"/>
        <h3>3.4  XML Documents with Optional Errors</h3>
        <p>Conforming XML 1.0 Processors are permitted to ignore
           certain errors, or to report them at user option.  In this
           section of this test report are found descriptions of
           test cases which fit into this category.</p>



        <p>Processor behavior on such test cases does not affect
           conformance to the XML 1.0 (Second Edition) Recommendation, except as noted.</p>

        <xsl:apply-templates select="//TEST[@TYPE='error']">
          <xsl:sort select="@SECTIONS"/>
        </xsl:apply-templates>
        <h2>Failed tests</h2>
        <xsl:for-each select="//TEST[pass!='true']">
          <xsl:value-of select="@ID"/><br/>
        </xsl:for-each>
      </body>
    </html>
  </xsl:template>

  <xsl:template match="TEST">
    <xsl:if test="($showOnlyFail and string(pass)!='true') or ($showOnlyFail and string(conformance/pass)='false') or ($showOnlyFail = false())">
      <table width="100%">
        <xsl:if test="$showUri">
          <tr valign="top">
            <td colspan="2">
              <a target="null" href="{@URI}">
                <xsl:value-of select="@URI"/>
              </a>
            </td>
          </tr>
        </xsl:if>
        <tr valign="top">
          <td width="40%">
            <table bgcolor="#eeeeff" border="1" width="100%" height="100%">
              <tr>
                <td width="50%">
                  <b>Sections [Rules]:</b>
                </td>
                <td bgcolor="#ffffcc">
                  <xsl:value-of select="@SECTIONS"/>
                </td>
              </tr>
              <tr valign="top">
                <td width="50%">
                  <b>Test ID:</b>
                </td>
                <td bgcolor="#ffffcc">
                  <xsl:value-of select="@ID"/>
                </td>
              </tr>
              <tr valign="top">
                <td width="50%">
                  <b>RECOMMENDATION:</b>
                </td>
                <td bgcolor="#ffffcc">
                  <xsl:value-of select="@RECOMMENDATION"/>
                </td>
              </tr>
              <xsl:if test="not ( @ENTITIES = 'none')     and ( @TYPE = 'not-wf' )">
                <tr valign="top">
                  <td width="50%">
                    <b>Entities:</b>
                  </td>
                  <td bgcolor="#ffffcc">
                    <font color="blue">
                      <xsl:value-of select="@ENTITIES"/>
                    </font>
                  </td>
                </tr>
              </xsl:if>
              <xsl:if test="../@PROFILE">
                <tr valign="top">
                  <td width="50%">
                    <b>Collection:</b>
                  </td>
                  <td bgcolor="#ffffcc">
                    <xsl:value-of select="../@PROFILE"/>
                  </td>
                </tr>
              </xsl:if>
            </table>
          </td>
          <td>
            <xsl:attribute name="bgcolor">
              <xsl:choose>
                <xsl:when test="pass='true'">#AADDAA</xsl:when>
                <xsl:otherwise>#DDAAAA</xsl:otherwise>
              </xsl:choose>

            </xsl:attribute>
            <p>
              <i>Message</i> : <xsl:value-of select="message"/>
            </p>
            <xsl:if test="(exception) and (string(exception) != string(fatalError))">
              <p>
                <i>Exception</i> : <xsl:value-of select="exception"/> at Line
                  <xsl:value-of select="exception/@line"/>, column
                  <xsl:value-of select="exception/@column"/>
              </p>
            </xsl:if>
            <xsl:if test="fatalError">
              <p>
                <i>Fatal Error</i> : <xsl:value-of select="fatalError"/> at Line
                  <xsl:value-of select="fatalError/@line"/>, column
                  <xsl:value-of select="fatalError/@column"/>
              </p>
            </xsl:if>
            <xsl:if test="error">
              <p>
                <i>Error</i> : <xsl:value-of select="error"/> at Line
                  <xsl:value-of select="error/@line"/>, column
                  <xsl:value-of select="error/@column"/>
              </p>
            </xsl:if>
            <xsl:if test="warning">
              <p>
                <i>Warning</i> : <xsl:value-of select="warning"/> at Line
                  <xsl:value-of select="warning/@line"/>, column
                  <xsl:value-of select="warning/@column"/>
              </p>
            </xsl:if>
            <xsl:if test="@OUTPUT | @OUTPUT3">
              <p>There is an output test associated with this input file.</p>
            </xsl:if>
          </td>
        </tr>
        <xsl:if test="conformance">
          <tr valign="top">
            <td width="40%">
              <table bgcolor="#eeeeff" border="1" width="100%" height="100%">
                <tr>
                  <td width="50%">
                    <b>CONFORMANCE:</b>
                  </td>
                  <td bgcolor="#ffffcc">
                    <xsl:choose>
                      <xsl:when test="conformance/pass='true'"><i>Passed</i></xsl:when>
                      <xsl:otherwise><i>Failed</i></xsl:otherwise>
                    </xsl:choose>
                  </td>
                </tr>
              </table>
            </td>
            <td>
              <xsl:attribute name="bgcolor">
                <xsl:choose>
                  <xsl:when test="conformance/pass='true'">#AADDAA</xsl:when>
                  <xsl:otherwise>#DDAAAA</xsl:otherwise>
                </xsl:choose>
              </xsl:attribute>
              <a target="null" href="file:///{conformance/output}">
                <xsl:value-of select="conformance/output"/>
              </a>
            </td>
          </tr>
        </xsl:if>
      </table>
      <br/>
      <br/>
    </xsl:if>
  </xsl:template>
  <xsl:template match="EM">
    <em>
      <xsl:apply-templates/>
    </em>
  </xsl:template>
  <xsl:template match="B">
    <b>
      <xsl:apply-templates/>
    </b>
  </xsl:template>
</xsl:stylesheet>

