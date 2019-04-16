// NO WARRANTY!  This code is in the Public Domain.
// Written by Karl Waclawek (karl@waclawek.net).

/*                       DEMO OVERVIEW
 *
 * This simple demo shows how to load an XML document into a tree view
 * component using a SAX parser. If the parser implements the Suspend()
 * and Resume() methods then incremental parsing is enabled as well.
 *
 * The code of this demo is pretty much self-explanatory, and there
 * are comments throughout the code that hopefully enhance its clarity.
 *
 * The various controls have tool tips that appear when the mouse is
 * hovering over the control, explaining its purpose.
 *
 * Incremental parsing is implemented this way:
 * Parsing is suspended whenever the number of elements encountered
 * since processing was last started or resumed, reaches a specific
 * number specified by the user. Only elements at a user-set level are
 * considered for that purpose. As an example, consider we are processing
 * a database extract where each record is represented by one element
 * (subtree) at the level directly under the root element. If our goal
 * is to process 100 records at a time, we would set the "Chunk Size" to
 * 100 and the "Chunk Level" to 2 (the root element being at level 1).
 */

using System;
using System.Windows.Forms;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Org.System.Xml.Sax;
using Org.System.Xml.Sax.Helpers;
using System.Drawing;
using System.Threading;


namespace SaxTreeviewDemo
{
  using SaxConsts = Org.System.Xml.Sax.Constants;

  class MainForm : System.Windows.Forms.Form
  {
    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.Label sizeLbl;
    private System.Windows.Forms.Button parserBtn;
    private System.Windows.Forms.TabPage errorPage;
    private System.Windows.Forms.OpenFileDialog openDlg;
    private System.Windows.Forms.Button stopBtn;
    private System.Windows.Forms.NumericUpDown levelUpDown;
    private System.Windows.Forms.TabPage treePage;
    private System.Windows.Forms.TextBox errorBox;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.Button processBtn;
    private System.Windows.Forms.RadioButton allBtn;
    private System.Windows.Forms.Label levelLbl;
    private System.Windows.Forms.RadioButton incrementalBtn;
    private System.Windows.Forms.NumericUpDown sizeUpDown;
    private System.Windows.Forms.Label parserLbl;
    private System.Windows.Forms.Button browseBtn;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TreeView treeView;
    private System.Windows.Forms.ImageList imageList;
    private System.Windows.Forms.TextBox fileBox;

    public MainForm()
    {
      InitializeComponent();
    }

    [STAThread]
    public static void Main(string[] args)
    {
      Application.EnableVisualStyles();
      Application.ThreadException += new ThreadExceptionEventHandler(HandleException);
      Application.Run(new MainForm());
    }

    static void HandleException(object sender, ThreadExceptionEventArgs e)
    {
      MessageBox.Show(e.Exception.Message,
                      "Application Error",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
    }

    #region Windows Forms Designer generated code
    // THIS METHOD IS MAINTAINED BY THE FORM DESIGNER
    // DO NOT EDIT IT MANUALLY! YOUR CHANGES ARE LIKELY TO BE LOST
    void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
        this.fileBox = new System.Windows.Forms.TextBox();
        this.imageList = new System.Windows.Forms.ImageList(this.components);
        this.treeView = new System.Windows.Forms.TreeView();
        this.tabControl = new System.Windows.Forms.TabControl();
        this.treePage = new System.Windows.Forms.TabPage();
        this.errorPage = new System.Windows.Forms.TabPage();
        this.errorBox = new System.Windows.Forms.TextBox();
        this.browseBtn = new System.Windows.Forms.Button();
        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
        this.parserLbl = new System.Windows.Forms.Label();
        this.sizeUpDown = new System.Windows.Forms.NumericUpDown();
        this.incrementalBtn = new System.Windows.Forms.RadioButton();
        this.levelLbl = new System.Windows.Forms.Label();
        this.allBtn = new System.Windows.Forms.RadioButton();
        this.processBtn = new System.Windows.Forms.Button();
        this.levelUpDown = new System.Windows.Forms.NumericUpDown();
        this.stopBtn = new System.Windows.Forms.Button();
        this.parserBtn = new System.Windows.Forms.Button();
        this.sizeLbl = new System.Windows.Forms.Label();
        this.openDlg = new System.Windows.Forms.OpenFileDialog();
        this.tabControl.SuspendLayout();
        this.treePage.SuspendLayout();
        this.errorPage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.levelUpDown)).BeginInit();
        this.SuspendLayout();
        // 
        // fileBox
        // 
        this.fileBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.fileBox.Location = new System.Drawing.Point(72, 8);
        this.fileBox.Name = "fileBox";
        this.fileBox.Size = new System.Drawing.Size(368, 20);
        this.fileBox.TabIndex = 1;
        // 
        // imageList
        // 
        this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
        this.imageList.Images.SetKeyName(0, "");
        this.imageList.Images.SetKeyName(1, "");
        this.imageList.Images.SetKeyName(2, "");
        this.imageList.Images.SetKeyName(3, "");
        this.imageList.Images.SetKeyName(4, "");
        this.imageList.Images.SetKeyName(5, "");
        // 
        // treeView
        // 
        this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
        this.treeView.ImageIndex = 0;
        this.treeView.ImageList = this.imageList;
        this.treeView.Location = new System.Drawing.Point(0, 0);
        this.treeView.Name = "treeView";
        this.treeView.SelectedImageIndex = 0;
        this.treeView.Size = new System.Drawing.Size(424, 350);
        this.treeView.TabIndex = 0;
        // 
        // tabControl
        // 
        this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                    | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.tabControl.Controls.Add(this.treePage);
        this.tabControl.Controls.Add(this.errorPage);
        this.tabControl.Location = new System.Drawing.Point(8, 88);
        this.tabControl.Name = "tabControl";
        this.tabControl.SelectedIndex = 0;
        this.tabControl.Size = new System.Drawing.Size(432, 376);
        this.tabControl.TabIndex = 9;
        // 
        // treePage
        // 
        this.treePage.Controls.Add(this.treeView);
        this.treePage.Location = new System.Drawing.Point(4, 22);
        this.treePage.Name = "treePage";
        this.treePage.Size = new System.Drawing.Size(424, 350);
        this.treePage.TabIndex = 0;
        this.treePage.Text = "Tree View";
        // 
        // errorPage
        // 
        this.errorPage.Controls.Add(this.errorBox);
        this.errorPage.Location = new System.Drawing.Point(4, 22);
        this.errorPage.Name = "errorPage";
        this.errorPage.Size = new System.Drawing.Size(424, 350);
        this.errorPage.TabIndex = 1;
        this.errorPage.Text = "Errors";
        // 
        // errorBox
        // 
        this.errorBox.AutoSize = false;
        this.errorBox.Dock = System.Windows.Forms.DockStyle.Fill;
        this.errorBox.Location = new System.Drawing.Point(0, 0);
        this.errorBox.Multiline = true;
        this.errorBox.Name = "errorBox";
        this.errorBox.Size = new System.Drawing.Size(424, 350);
        this.errorBox.TabIndex = 0;
        // 
        // browseBtn
        // 
        this.browseBtn.Location = new System.Drawing.Point(8, 7);
        this.browseBtn.Name = "browseBtn";
        this.browseBtn.Size = new System.Drawing.Size(56, 23);
        this.browseBtn.TabIndex = 0;
        this.browseBtn.Text = "Browse";
        this.toolTip.SetToolTip(this.browseBtn, "Select an XML document");
        this.browseBtn.Click += new System.EventHandler(this.BrowseBtnClick);
        // 
        // parserLbl
        // 
        this.parserLbl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.parserLbl.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
        this.parserLbl.Location = new System.Drawing.Point(287, 37);
        this.parserLbl.Name = "parserLbl";
        this.parserLbl.Size = new System.Drawing.Size(152, 19);
        this.parserLbl.TabIndex = 14;
        this.parserLbl.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.toolTip.SetToolTip(this.parserLbl, "Name of SAX parser");
        // 
        // sizeUpDown
        // 
        this.sizeUpDown.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
        this.sizeUpDown.Location = new System.Drawing.Point(224, 35);
        this.sizeUpDown.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
        this.sizeUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
        this.sizeUpDown.Name = "sizeUpDown";
        this.sizeUpDown.Size = new System.Drawing.Size(48, 20);
        this.sizeUpDown.TabIndex = 6;
        this.sizeUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        this.toolTip.SetToolTip(this.sizeUpDown, "Number of elements to process on each increment");
        this.sizeUpDown.Value = new decimal(new int[] {
            10,
            0,
            0,
            0});
        // 
        // incrementalBtn
        // 
        this.incrementalBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.incrementalBtn.Location = new System.Drawing.Point(72, 64);
        this.incrementalBtn.Name = "incrementalBtn";
        this.incrementalBtn.Size = new System.Drawing.Size(56, 16);
        this.incrementalBtn.TabIndex = 5;
        this.incrementalBtn.Text = "Chunk";
        this.incrementalBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.toolTip.SetToolTip(this.incrementalBtn, "Process document in chunks, as specified");
        this.incrementalBtn.CheckedChanged += new System.EventHandler(this.ProcessingModeChanged);
        // 
        // levelLbl
        // 
        this.levelLbl.Location = new System.Drawing.Point(144, 66);
        this.levelLbl.Name = "levelLbl";
        this.levelLbl.Size = new System.Drawing.Size(72, 16);
        this.levelLbl.TabIndex = 11;
        this.levelLbl.Text = "Chunk Level";
        this.toolTip.SetToolTip(this.levelLbl, "Document level at which to count the elements");
        // 
        // allBtn
        // 
        this.allBtn.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
        this.allBtn.Checked = true;
        this.allBtn.Location = new System.Drawing.Point(88, 40);
        this.allBtn.Name = "allBtn";
        this.allBtn.Size = new System.Drawing.Size(40, 14);
        this.allBtn.TabIndex = 4;
        this.allBtn.Text = "All";
        this.allBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        this.toolTip.SetToolTip(this.allBtn, "Process document all at once");
        this.allBtn.CheckedChanged += new System.EventHandler(this.ProcessingModeChanged);
        // 
        // processBtn
        // 
        this.processBtn.Location = new System.Drawing.Point(8, 35);
        this.processBtn.Name = "processBtn";
        this.processBtn.Size = new System.Drawing.Size(56, 23);
        this.processBtn.TabIndex = 2;
        this.processBtn.Text = "Process";
        this.toolTip.SetToolTip(this.processBtn, "Start or continue processing");
        this.processBtn.Click += new System.EventHandler(this.ProcessBtnClick);
        // 
        // levelUpDown
        // 
        this.levelUpDown.Location = new System.Drawing.Point(224, 62);
        this.levelUpDown.Minimum = new decimal(new int[] {
            2,
            0,
            0,
            0});
        this.levelUpDown.Name = "levelUpDown";
        this.levelUpDown.Size = new System.Drawing.Size(48, 20);
        this.levelUpDown.TabIndex = 7;
        this.levelUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
        this.toolTip.SetToolTip(this.levelUpDown, "Document level at which to count the elements");
        this.levelUpDown.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
        // 
        // stopBtn
        // 
        this.stopBtn.Location = new System.Drawing.Point(8, 60);
        this.stopBtn.Name = "stopBtn";
        this.stopBtn.Size = new System.Drawing.Size(56, 24);
        this.stopBtn.TabIndex = 3;
        this.stopBtn.Text = "Stop";
        this.toolTip.SetToolTip(this.stopBtn, "Stop incremental processing");
        this.stopBtn.Click += new System.EventHandler(this.StopBtnClick);
        // 
        // parserBtn
        // 
        this.parserBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                    | System.Windows.Forms.AnchorStyles.Right)));
        this.parserBtn.Location = new System.Drawing.Point(288, 60);
        this.parserBtn.Name = "parserBtn";
        this.parserBtn.Size = new System.Drawing.Size(152, 23);
        this.parserBtn.TabIndex = 8;
        this.parserBtn.Text = "Change Parser";
        this.toolTip.SetToolTip(this.parserBtn, "Load SAX Parser");
        this.parserBtn.Click += new System.EventHandler(this.ParserBtnClick);
        // 
        // sizeLbl
        // 
        this.sizeLbl.Location = new System.Drawing.Point(152, 40);
        this.sizeLbl.Name = "sizeLbl";
        this.sizeLbl.Size = new System.Drawing.Size(64, 14);
        this.sizeLbl.TabIndex = 8;
        this.sizeLbl.Text = "Chunk Size";
        this.toolTip.SetToolTip(this.sizeLbl, "Number of elements to process on each increment");
        // 
        // openDlg
        // 
        this.openDlg.Filter = "(XML files)|*.xml";
        // 
        // MainForm
        // 
        this.ClientSize = new System.Drawing.Size(448, 469);
        this.Controls.Add(this.tabControl);
        this.Controls.Add(this.parserLbl);
        this.Controls.Add(this.parserBtn);
        this.Controls.Add(this.levelLbl);
        this.Controls.Add(this.levelUpDown);
        this.Controls.Add(this.sizeUpDown);
        this.Controls.Add(this.sizeLbl);
        this.Controls.Add(this.incrementalBtn);
        this.Controls.Add(this.allBtn);
        this.Controls.Add(this.stopBtn);
        this.Controls.Add(this.browseBtn);
        this.Controls.Add(this.fileBox);
        this.Controls.Add(this.processBtn);
        this.Name = "MainForm";
        this.Text = "Treeview Demo";
        this.Load += new System.EventHandler(this.MainFormLoad);
        this.tabControl.ResumeLayout(false);
        this.treePage.ResumeLayout(false);
        this.errorPage.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.sizeUpDown)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.levelUpDown)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();

    }
    #endregion

    private const string noParserLoaded = "<no parser loaded>";
    private bool incrementalMode = false;
    private IXmlReader reader = null;
    private ErrHandler errorHandler = new ErrHandler();

    /* Error handler for SAX parser. */
    class ErrHandler: IErrorHandler
    {
      private ArrayList msgList;

      public ErrHandler()
      {
        this.msgList = new ArrayList();
      }

      public string[] Lines
      {
        get {
          string[] lines = new String[msgList.Count];
          for (int indx = 0; indx < msgList.Count; indx++)
            lines[indx] = (string)msgList[indx];
          return lines;
        }
      }

      public void Reset()
      {
        msgList.Clear();
      }

      public void AddMessage(string msg)
      {
        msgList.Add(msg);
      }

      /* IErrorHandler */

      public void Warning(ParseError error)
      {
        string msg = "Warning: " + error.Message;
        if (error.BaseException != null)
          msg = msg + Environment.NewLine + error.BaseException.Message;
        msgList.Add(msg);
      }

      public void Error(ParseError error)
      {
        string msg = "Error: " + error.Message;
        if (error.BaseException != null)
          msg = msg + Environment.NewLine + error.BaseException.Message;
        msgList.Add(msg);
      }

      public void FatalError(ParseError error)
      {
        error.Throw();
      }

    }

    /* Content, lexical and declaration handler for SAX parser. */
    class CntHandler: IContentHandler, ILexicalHandler, IDeclHandler
    {
      private TreeView tv;
      private StringBuilder builder;
      private TreeNode currentNode = null;

      public CntHandler(TreeView tv) {
        this.tv = tv;
        this.builder = new StringBuilder();
      }

      /* IContentHandler */

      public void SetDocumentLocator(ILocator locator)
      {
        // ignore
      }

      public void StartDocument()
      {
        // initialize tree view and string builder
        currentNode = null;
        tv.Nodes.Clear();
        builder.Length = 0;
      }

      public void EndDocument()
      {
        // ignore
      }

      public void StartPrefixMapping(string prefix, string uri)
      {
        // ignore
      }

      public void EndPrefixMapping(string prefix)
      {
        // ignore
      }

      private TreeNodeCollection CurrentNodes()
      {
        TreeNodeCollection nodes;
        if (currentNode == null)
          nodes = tv.Nodes;
        else
          nodes = currentNode.Nodes;
        return nodes;
      }

      /* Whenever a call-back might have been preceded by a
       * Characters() call we must check if any character data
       * were reported, and create a text node if necessary.
       * As a side effect (for efficiency purposes) this returns
       * the current node's Nodes collection.
       * Note: Text nodes consisting of whitespace only are ignored.
       */
      private TreeNodeCollection CheckTextNode()
      {
        TreeNodeCollection nodes = CurrentNodes();
        string nodeText = (builder.ToString()).Trim();
        if (nodeText.Length > 0) {
          nodes.Add(new TreeNode(nodeText, 4, 4));
          builder.Length = 0;
        }
        return nodes;
      }

      public virtual void StartElement(
                 string uri, string localName, string qName, IAttributes atts)
      {
        TreeNodeCollection nodes = CheckTextNode();
        currentNode = new TreeNode(qName, 0, 0);
        for (int indx = 0; indx < atts.Length; indx ++) {
          string attStr = atts.GetQName(indx) + "=\"" + atts.GetValue(indx) + "\"";
          currentNode.Nodes.Add(new TreeNode(attStr, 1, 1));
        }
        nodes.Add(currentNode);
      }

      public virtual void EndElement(string uri, string localName, string qName)
      {
        CheckTextNode();
        currentNode = currentNode.Parent;
      }

      public void Characters(char[] ch, int start, int length)
      {
        builder.Append(ch, start, length);
      }

      public void IgnorableWhitespace(char[] ch, int start, int length)
      {
        // ignore
      }

      public void ProcessingInstruction(string target, string data)
      {
        TreeNodeCollection nodes = CheckTextNode();
        string piStr = String.Format("{0}={1}", target, data);
        TreeNode node = new TreeNode(piStr, 3, 3);
        Font fnt = new Font(tv.Font, FontStyle.Underline);
        node.NodeFont = fnt;
        nodes.Add(node);
      }

      public void SkippedEntity(string name)
      {
        // ignore
      }

      /* ILexicalhandler */

      public void StartDtd(string name, string publicId, string systemId)
      {
        // ignore
      }

      public void EndDtd()
      {
        // ignore
      }

      public void StartEntity(string name)
      {
        // ignore
      }

      public void EndEntity(string name)
      {
        // ignore
      }

      public void StartCData()
      {
        // ignore
      }

      public void EndCData()
      {
        // ignore
      }

      public void Comment(char[] ch, int start, int length)
      {
        TreeNodeCollection nodes = CheckTextNode();
        TreeNode node = new TreeNode(new string(ch, start, length), 2, 2);
        Font fnt = new Font(tv.Font, FontStyle.Italic);
        node.NodeFont = fnt;
        nodes.Add(node);
      }

      /* IDeclHandler */

      public void ElementDecl(string name, string model)
      {
        string declStr = String.Format("<!ELEMENT {0} {1}>", name, model);
        TreeNode node = new TreeNode(declStr, 5, 5);
        CurrentNodes().Add(node);
      }

      public void AttributeDecl(string eName, string aName, string aType,
                                string mode, string aValue)
      {
        const string attStr = "<!ATTRIBUTE({0}) ";
        const string modeStr = attStr + "{1} {2} {3}>";
        const string valueStr = attStr + "{1} {2} \"{3}\">";
        const string allStr = attStr + "{1} {2} {3} \"{4}\">";

        string declStr;
        if (mode == null || mode == String.Empty)
          declStr = String.Format(valueStr, eName, aName, aType, aValue);
        else if (aValue == null)
          declStr = String.Format(modeStr, eName, aName, aType, mode);
        else
          declStr = String.Format(allStr, eName, aName, aType, mode, aValue);
        TreeNode node = new TreeNode(declStr, 5, 5);
        CurrentNodes().Add(node);
      }

      public void InternalEntityDecl(string name, string value)
      {
        string declStr = String.Format("<!ENTITY {0} \"{1}\">", name, value);
        TreeNode node = new TreeNode(declStr, 5, 5);
        CurrentNodes().Add(node);
      }

      public void ExternalEntityDecl(string name, string publicId, string systemId)
      {
        const string pubIdStr = "<!ENTITY {0} PUBLIC \"{1}\" SYSTEM \"{2}\">";
        const string sysIdStr = "<!ENTITY {0} SYSTEM \"{1}\">";
        string declStr;
        if (publicId != String.Empty)
          declStr = String.Format(pubIdStr, name, publicId, systemId);
        else
          declStr = String.Format(sysIdStr, name, systemId);
        TreeNode node = new TreeNode(declStr, 5, 5);
        CurrentNodes().Add(node);
      }
    }

    /* Content, lexical and declaration handler for incremental SAX parsing. */
    class ChunkHandler: CntHandler
    {
      private IXmlReader reader;
      private int elmCount;
      private int level;
      private readonly int activeLevel;
      private readonly int chunkSize;

      public ChunkHandler(IXmlReader reader,
                          TreeView tv,
                          int chunkSize,
                          int activeLevel): base(tv)
      {
        this.reader = reader;
        this.chunkSize = chunkSize;
        this.activeLevel = activeLevel;
        elmCount = chunkSize;
      }

      /* We only count elements at a specific level/depth. */
      public override void StartElement(
                 string uri, string localName, string qName, IAttributes atts)
      {
        level++;
        if (level == activeLevel) elmCount--;
        base.StartElement(uri, localName, qName, atts);
        if (elmCount == 0) {
          elmCount = chunkSize;
          reader.Suspend();
        }
      }

      public override void EndElement(string uri, string localName, string qName)
      {
        level--;
        base.EndElement(uri, localName, qName);
      }
    }

    /* Get or set the current processing mode. */
    private bool IncrementalMode
    {
      get { return incrementalMode; }
      set {
        if (Suspendable)
          incrementalMode = value;
        else
          throw new ApplicationException("Parser is not suspendable.");
        UpdateUI();
      }
    }

    /* Is this IXmlReader capable of incremental processing? */
    private bool Suspendable
    {
      get {
        bool result = reader != null;
        if (result) {
          try {
            result = reader.GetFeature(Constants.ReaderControlFeature);
          }
          catch (ArgumentException) {
            result = false;
          }
          catch (NotSupportedException) {
            result = false;
          }
        }
        return result;
      }
    }

    /* Are we in the middle of incremental processing? */
    private bool ParsingSuspended
    {
      get {
        if (!Suspendable)
          return false;
        else
          return reader.Status == XmlReaderStatus.Suspended;
      }
    }

    /* Update user interface - call when form's state has changed. */
    private void UpdateUI()
    {
      bool suspended = ParsingSuspended;
      bool resizable = IncrementalMode && !suspended;

      sizeLbl.Enabled = resizable;
      sizeUpDown.Enabled = resizable;
      levelLbl.Enabled = resizable;
      levelUpDown.Enabled = resizable;
      parserLbl.Enabled = !suspended;
      parserBtn.Enabled = !suspended;
      stopBtn.Enabled = suspended;
      processBtn.Enabled = reader != null;
      if (!Suspendable)
        allBtn.Checked = true;
      allBtn.Enabled = !suspended;
      incrementalBtn.Enabled = Suspendable && !suspended;
      browseBtn.Enabled = !suspended;
    }

    /* Initialize class - try to load the default SAX parser if there is one. */
    private void MainFormLoad(object sender, System.EventArgs e)
    {
      try {
        reader = SaxReaderFactory.CreateReader(null);
      }
      catch
      {
        // ignore exception
        reader = null;
      }
      if (reader != null) {
        Type readerType = reader.GetType();
        parserLbl.Text = readerType.FullName;
      }
      else
        parserLbl.Text = noParserLoaded;
      UpdateUI();
    }

    private void CheckNotSuspended()
    {
      if (ParsingSuspended)
        throw new ApplicationException("Parsing suspended.");
    }

    /* Pick XML document to process. */
    private void BrowseBtnClick(object sender, System.EventArgs e)
    {
      CheckNotSuspended();
      if (openDlg.ShowDialog() == DialogResult.OK)
        fileBox.Text = openDlg.FileName;
    }

    private FileStream fileStm;

    /* Start or continue processing the XML document. */
    private void DoParse()
    {
      if (ParsingSuspended)
        reader.Resume();
      else {
        fileStm = new FileStream(fileBox.Text, FileMode.Open, FileAccess.Read);
        errorHandler.Reset();
        reader.ErrorHandler = errorHandler;
        // turn on namespaces and tell parser to process external entities
        try {
          reader.SetFeature(SaxConsts.NamespacesFeature, true);
          reader.SetFeature(SaxConsts.ExternalGeneralFeature, true);
          reader.SetFeature(SaxConsts.ExternalParameterFeature, true);
        }
        catch (Exception e) {
          MessageBox.Show(e.Message);
        }
        CntHandler handler;
        // create content/lexical handler depending on processing mode
        if (IncrementalMode) {
          int chunkSize = (int)sizeUpDown.Value;
          int activeLevel = (int)levelUpDown.Value;
          handler = new ChunkHandler(reader, treeView, chunkSize, activeLevel);
        }
        else {
          handler = new CntHandler(treeView);
        }
        // set content handler - this is straightforward
        reader.ContentHandler = handler;
        // set lexical handler, if possible
        try {
          reader.LexicalHandler = handler;
        }
        catch (Exception e) {
          errorHandler.AddMessage("Cannot set lexical handler: " + e.Message);
        }
        // set declaration handler, if possible
        try {
          reader.DeclHandler = handler;
        }
        catch (Exception e) {
          errorHandler.AddMessage("Cannot set declaration handler: " + e.Message);
        }
        // finally, process the document
        reader.Parse(new InputSource<Stream>(fileStm));
      }
    }

    private bool isActive = false;

    /* Process the XML document - all of it, or in increments. */
    private void ProcessBtnClick(object sender, System.EventArgs e)
    {
      // avoid restarting an active parser
      if (isActive)
        return;
      bool mustCloseStream = false;
      Cursor oldCursor = Cursor.Current;
      Cursor.Current = Cursors.WaitCursor;
      treeView.BeginUpdate();
      isActive = true;
      try {
        DoParse();
        mustCloseStream = true;
        // if we are finished, close the stream
        if (!ParsingSuspended) {
          mustCloseStream = false;
          fileStm.Close();
        }
      }
      catch {
        // close the stream if it is not closed yet
        if (mustCloseStream)
          fileStm.Close();
        throw;
      }
      finally {
        isActive = false;
        treeView.EndUpdate();
        Cursor.Current = oldCursor;
        UpdateUI();
        errorBox.Lines = errorHandler.Lines;
      }
    }

    /* Change between incremental or all-at-once modes. */
    private void ProcessingModeChanged(object sender, System.EventArgs e)
    {
      CheckNotSuspended();
      IncrementalMode = incrementalBtn.Checked;
    }

    /* Abort incremental parsing before resuming again. */
    private void StopBtnClick(object sender, System.EventArgs e)
    {
      if (ParsingSuspended) {
        reader.Abort();
        UpdateUI();
      }
      else
        throw new ApplicationException("Parsing not suspended.");
    }

    /* Load an IXmlReader instance. */
    private void ParserBtnClick(object sender, System.EventArgs e)
    {
      CheckNotSuspended();
      Assembly assem = null;
      Type readerType = null;
      // get type and assembly for existing reader and display it
      if (reader != null) {
        readerType = reader.GetType();
        assem = Assembly.GetAssembly(readerType);
      }
      PickDlg pickDlg = new PickDlg();
      if (assem != null) {
        pickDlg.ParserAssembly = assem.Location;
        pickDlg.ParserClass = readerType.FullName;
      }
      if (pickDlg.ShowDialog() != DialogResult.OK)
        return;
      // if we picked an assembly and (optionally) a class, let's load them
      string assemName = pickDlg.ParserAssembly;
      assem = null;
      if (File.Exists(assemName))
        assem = Assembly.LoadFrom(assemName);
      else
        assem = Assembly.Load(assemName);
      if (assem == null)
        throw new ApplicationException(
                   String.Format("Assembly not found: {0}.", assemName));
      string readerClass = pickDlg.ParserClass;
      IXmlReader newReader = null;
      if (readerClass == null || readerClass == String.Empty)
        // we have only an assembly, so load whatever IXmlReader we can find
        newReader = SaxReaderFactory.CreateReader(assem, null);
      else
        // class and assembly specified, we know exactly what we want
        newReader = SaxReaderFactory.CreateReader(assem, readerClass, null);
      if (newReader != null) {
        reader = newReader;
        readerType = newReader.GetType();
        parserLbl.Text = readerType.FullName;
        UpdateUI();
      }
      else
        throw new ApplicationException(
                String.Format("Cannot create parser {0}.", readerClass));
    }

  }
}
