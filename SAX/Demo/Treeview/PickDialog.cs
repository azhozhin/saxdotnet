// NO WARRANTY!  This code is in the Public Domain.
// Written by Karl Waclawek (karl@waclawek.net).

using System;
using System.Windows.Forms;

namespace SaxTreeviewDemo
{
  /// <summary>
  /// Dialog to pick a SAX parser by assembly name and class name.
  /// </summary>
  public class PickDlg: System.Windows.Forms.Form
  {
    private System.ComponentModel.IContainer components;
    private System.Windows.Forms.Label asmLbl;
    private System.Windows.Forms.ToolTip toolTip;
    private System.Windows.Forms.TextBox assemBox;
    private System.Windows.Forms.OpenFileDialog fileDlg;
    private System.Windows.Forms.Button okBtn;
    private System.Windows.Forms.Button cancelBtn;
    private System.Windows.Forms.Button browseBtn;
    private System.Windows.Forms.Label classLbl;
    private System.Windows.Forms.TextBox classBox;
    public PickDlg()
    {
      //
      // The InitializeComponent() call is required for Windows Forms designer support.
      //
      InitializeComponent();
    }

    #region Windows Forms Designer generated code
    /// <summary>
    /// This method is required for Windows Forms designer support.
    /// Do not change the method contents inside the source code editor. The Forms designer might
    /// not be able to load this method if it was changed manually.
    /// </summary>
    private void InitializeComponent() {
        this.components = new System.ComponentModel.Container();
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PickDlg));
        this.classBox = new System.Windows.Forms.TextBox();
        this.toolTip = new System.Windows.Forms.ToolTip(this.components);
        this.classLbl = new System.Windows.Forms.Label();
        this.browseBtn = new System.Windows.Forms.Button();
        this.assemBox = new System.Windows.Forms.TextBox();
        this.asmLbl = new System.Windows.Forms.Label();
        this.cancelBtn = new System.Windows.Forms.Button();
        this.okBtn = new System.Windows.Forms.Button();
        this.fileDlg = new System.Windows.Forms.OpenFileDialog();
        this.SuspendLayout();
        // 
        // classBox
        // 
        this.classBox.Location = new System.Drawing.Point(64, 48);
        this.classBox.Name = "classBox";
        this.classBox.Size = new System.Drawing.Size(264, 20);
        this.classBox.TabIndex = 1;
        this.toolTip.SetToolTip(this.classBox, "Fully qualified class name (optional)");
        // 
        // classLbl
        // 
        this.classLbl.Location = new System.Drawing.Point(8, 51);
        this.classLbl.Name = "classLbl";
        this.classLbl.Size = new System.Drawing.Size(40, 16);
        this.classLbl.TabIndex = 3;
        this.classLbl.Text = "Class";
        this.toolTip.SetToolTip(this.classLbl, "Fully qualified class name (optional)");
        // 
        // browseBtn
        // 
        this.browseBtn.BackColor = System.Drawing.SystemColors.Control;
        this.browseBtn.Image = ((System.Drawing.Image)(resources.GetObject("browseBtn.Image")));
        this.browseBtn.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
        this.browseBtn.Location = new System.Drawing.Point(328, 13);
        this.browseBtn.Name = "browseBtn";
        this.browseBtn.Size = new System.Drawing.Size(25, 23);
        this.browseBtn.TabIndex = 4;
        this.browseBtn.TextAlign = System.Drawing.ContentAlignment.TopLeft;
        this.toolTip.SetToolTip(this.browseBtn, "Pick assembly file");
        this.browseBtn.UseVisualStyleBackColor = false;
        this.browseBtn.Click += new System.EventHandler(this.BrowseBtnClick);
        // 
        // assemBox
        // 
        this.assemBox.Location = new System.Drawing.Point(64, 14);
        this.assemBox.Name = "assemBox";
        this.assemBox.Size = new System.Drawing.Size(264, 20);
        this.assemBox.TabIndex = 0;
        this.toolTip.SetToolTip(this.assemBox, "Assembly name or full file path");
        // 
        // asmLbl
        // 
        this.asmLbl.Location = new System.Drawing.Point(8, 16);
        this.asmLbl.Name = "asmLbl";
        this.asmLbl.Size = new System.Drawing.Size(56, 16);
        this.asmLbl.TabIndex = 2;
        this.asmLbl.Text = "Assembly";
        this.toolTip.SetToolTip(this.asmLbl, "Assembly name or full file path");
        // 
        // cancelBtn
        // 
        this.cancelBtn.DialogResult = System.Windows.Forms.DialogResult.Cancel;
        this.cancelBtn.Location = new System.Drawing.Point(200, 88);
        this.cancelBtn.Name = "cancelBtn";
        this.cancelBtn.Size = new System.Drawing.Size(75, 23);
        this.cancelBtn.TabIndex = 3;
        this.cancelBtn.Text = "Cancel";
        // 
        // okBtn
        // 
        this.okBtn.DialogResult = System.Windows.Forms.DialogResult.OK;
        this.okBtn.Location = new System.Drawing.Point(88, 88);
        this.okBtn.Name = "okBtn";
        this.okBtn.Size = new System.Drawing.Size(75, 23);
        this.okBtn.TabIndex = 2;
        this.okBtn.Text = "OK";
        // 
        // PickDlg
        // 
        this.AcceptButton = this.okBtn;
        this.CancelButton = this.cancelBtn;
        this.ClientSize = new System.Drawing.Size(362, 127);
        this.Controls.Add(this.browseBtn);
        this.Controls.Add(this.classBox);
        this.Controls.Add(this.assemBox);
        this.Controls.Add(this.classLbl);
        this.Controls.Add(this.asmLbl);
        this.Controls.Add(this.cancelBtn);
        this.Controls.Add(this.okBtn);
        this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "PickDlg";
        this.ShowInTaskbar = false;
        this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        this.Text = "Pick Parser";
        this.ResumeLayout(false);
        this.PerformLayout();

    }
    #endregion

    void BrowseBtnClick(object sender, System.EventArgs e)
    {
      if (fileDlg.ShowDialog() == DialogResult.OK)
      assemBox.Text = fileDlg.FileName;
    }

    public string ParserAssembly
    {
      get { return assemBox.Text; }
      set { assemBox.Text = value; }
    }

    public string ParserClass
    {
      get { return classBox.Text; }
      set { classBox.Text = value; }
    }
  }
}
