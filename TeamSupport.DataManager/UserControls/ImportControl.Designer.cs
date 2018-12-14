namespace TeamSupport.DataManager.UserControls
{
  partial class ImportControl
  {
    /// <summary> 
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary> 
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.btnOpen = new System.Windows.Forms.Button();
      this.btnImport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.lblFileName = new System.Windows.Forms.Label();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnClone = new System.Windows.Forms.Button();
      this.cbCustomFieldsOnly = new System.Windows.Forms.CheckBox();
      this.cbNewOrg = new System.Windows.Forms.CheckBox();
      this.label2 = new System.Windows.Forms.Label();
      this.textOrganizationName = new System.Windows.Forms.TextBox();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tpSummary = new System.Windows.Forms.TabPage();
      this.lbSummary = new System.Windows.Forms.ListBox();
      this.tpMessages = new System.Windows.Forms.TabPage();
      this.lbMessages = new System.Windows.Forms.ListBox();
      this.btnUnkownDups = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tpSummary.SuspendLayout();
      this.tpMessages.SuspendLayout();
      this.SuspendLayout();
      // 
      // btnOpen
      // 
      this.btnOpen.Location = new System.Drawing.Point(6, 20);
      this.btnOpen.Name = "btnOpen";
      this.btnOpen.Size = new System.Drawing.Size(107, 23);
      this.btnOpen.TabIndex = 0;
      this.btnOpen.Text = "Open Spreadsheet";
      this.btnOpen.UseVisualStyleBackColor = true;
      this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
      // 
      // btnImport
      // 
      this.btnImport.Enabled = false;
      this.btnImport.Location = new System.Drawing.Point(119, 20);
      this.btnImport.Name = "btnImport";
      this.btnImport.Size = new System.Drawing.Size(75, 23);
      this.btnImport.TabIndex = 1;
      this.btnImport.Text = "Import Data";
      this.btnImport.UseVisualStyleBackColor = true;
      this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 0);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(54, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "FileName:";
      // 
      // lblFileName
      // 
      this.lblFileName.AutoSize = true;
      this.lblFileName.Location = new System.Drawing.Point(63, 0);
      this.lblFileName.Name = "lblFileName";
      this.lblFileName.Size = new System.Drawing.Size(91, 13);
      this.lblFileName.TabIndex = 3;
      this.lblFileName.Text = "[No File Selected]";
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnUnkownDups);
      this.panel1.Controls.Add(this.btnClone);
      this.panel1.Controls.Add(this.cbCustomFieldsOnly);
      this.panel1.Controls.Add(this.cbNewOrg);
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.textOrganizationName);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.lblFileName);
      this.panel1.Controls.Add(this.btnOpen);
      this.panel1.Controls.Add(this.btnImport);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(743, 84);
      this.panel1.TabIndex = 4;
      // 
      // btnClone
      // 
      this.btnClone.Location = new System.Drawing.Point(616, 20);
      this.btnClone.Name = "btnClone";
      this.btnClone.Size = new System.Drawing.Size(75, 23);
      this.btnClone.TabIndex = 8;
      this.btnClone.Text = "Clone";
      this.btnClone.UseVisualStyleBackColor = true;
      this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
      // 
      // cbCustomFieldsOnly
      // 
      this.cbCustomFieldsOnly.AutoSize = true;
      this.cbCustomFieldsOnly.Location = new System.Drawing.Point(7, 49);
      this.cbCustomFieldsOnly.Name = "cbCustomFieldsOnly";
      this.cbCustomFieldsOnly.Size = new System.Drawing.Size(115, 17);
      this.cbCustomFieldsOnly.TabIndex = 7;
      this.cbCustomFieldsOnly.Text = "Custom Fields Only";
      this.cbCustomFieldsOnly.UseVisualStyleBackColor = true;
      // 
      // cbNewOrg
      // 
      this.cbNewOrg.AutoSize = true;
      this.cbNewOrg.Checked = true;
      this.cbNewOrg.CheckState = System.Windows.Forms.CheckState.Checked;
      this.cbNewOrg.Location = new System.Drawing.Point(200, 24);
      this.cbNewOrg.Name = "cbNewOrg";
      this.cbNewOrg.Size = new System.Drawing.Size(110, 17);
      this.cbNewOrg.TabIndex = 6;
      this.cbNewOrg.Text = "New Organization";
      this.cbNewOrg.UseVisualStyleBackColor = true;
      this.cbNewOrg.CheckedChanged += new System.EventHandler(this.cbNewOrg_CheckedChanged);
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(316, 25);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(100, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "Organization Name:";
      // 
      // textOrganizationName
      // 
      this.textOrganizationName.Location = new System.Drawing.Point(422, 22);
      this.textOrganizationName.Name = "textOrganizationName";
      this.textOrganizationName.Size = new System.Drawing.Size(188, 20);
      this.textOrganizationName.TabIndex = 4;
      this.textOrganizationName.Text = "__NEW_ORGANIZATION_IMPORT";
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.tpSummary);
      this.tabControl.Controls.Add(this.tpMessages);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 84);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(743, 507);
      this.tabControl.TabIndex = 5;
      // 
      // tpSummary
      // 
      this.tpSummary.Controls.Add(this.lbSummary);
      this.tpSummary.Location = new System.Drawing.Point(4, 22);
      this.tpSummary.Name = "tpSummary";
      this.tpSummary.Padding = new System.Windows.Forms.Padding(3);
      this.tpSummary.Size = new System.Drawing.Size(735, 481);
      this.tpSummary.TabIndex = 0;
      this.tpSummary.Text = "Summary";
      this.tpSummary.UseVisualStyleBackColor = true;
      // 
      // lbSummary
      // 
      this.lbSummary.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lbSummary.FormattingEnabled = true;
      this.lbSummary.Location = new System.Drawing.Point(3, 3);
      this.lbSummary.Name = "lbSummary";
      this.lbSummary.ScrollAlwaysVisible = true;
      this.lbSummary.Size = new System.Drawing.Size(729, 475);
      this.lbSummary.TabIndex = 0;
      // 
      // tpMessages
      // 
      this.tpMessages.Controls.Add(this.lbMessages);
      this.tpMessages.Location = new System.Drawing.Point(4, 22);
      this.tpMessages.Name = "tpMessages";
      this.tpMessages.Padding = new System.Windows.Forms.Padding(3);
      this.tpMessages.Size = new System.Drawing.Size(735, 481);
      this.tpMessages.TabIndex = 1;
      this.tpMessages.Text = "Messages";
      this.tpMessages.UseVisualStyleBackColor = true;
      // 
      // lbMessages
      // 
      this.lbMessages.Dock = System.Windows.Forms.DockStyle.Fill;
      this.lbMessages.FormattingEnabled = true;
      this.lbMessages.Location = new System.Drawing.Point(3, 3);
      this.lbMessages.Name = "lbMessages";
      this.lbMessages.Size = new System.Drawing.Size(729, 475);
      this.lbMessages.TabIndex = 1;
      // 
      // btnUnkownDups
      // 
      this.btnUnkownDups.Location = new System.Drawing.Point(128, 47);
      this.btnUnkownDups.Name = "btnUnkownDups";
      this.btnUnkownDups.Size = new System.Drawing.Size(163, 23);
      this.btnUnkownDups.TabIndex = 9;
      this.btnUnkownDups.Text = "Fix Unknown Dupes";
      this.btnUnkownDups.UseVisualStyleBackColor = true;
      this.btnUnkownDups.Click += new System.EventHandler(this.btnUnkownDups_Click);
      // 
      // ImportControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.panel1);
      this.Name = "ImportControl";
      this.Size = new System.Drawing.Size(743, 591);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.tpSummary.ResumeLayout(false);
      this.tpMessages.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Button btnOpen;
    private System.Windows.Forms.Button btnImport;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label lblFileName;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tpSummary;
    private System.Windows.Forms.TabPage tpMessages;
    private System.Windows.Forms.ListBox lbSummary;
    private System.Windows.Forms.ListBox lbMessages;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textOrganizationName;
    private System.Windows.Forms.CheckBox cbNewOrg;
    private System.Windows.Forms.CheckBox cbCustomFieldsOnly;
    private System.Windows.Forms.Button btnClone;
    private System.Windows.Forms.Button btnUnkownDups;
  }
}
