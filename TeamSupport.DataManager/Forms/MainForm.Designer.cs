namespace TeamSupport.DataManager
{
  partial class MainForm
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

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnEditOrganization = new System.Windows.Forms.Button();
      this.btnTestButton = new System.Windows.Forms.Button();
      this.btnDeleteOrganization = new System.Windows.Forms.Button();
      this.btnNewOrganization = new System.Windows.Forms.Button();
      this.cmbOrganization = new System.Windows.Forms.ComboBox();
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.tpProperties = new System.Windows.Forms.TabPage();
      this.propertiesControl1 = new TeamSupport.DataManager.UserControls.PropertiesControl();
      this.tpImport = new System.Windows.Forms.TabPage();
      this.importControl1 = new TeamSupport.DataManager.UserControls.ImportControl();
      this.tpFields = new System.Windows.Forms.TabPage();
      this.customFieldsControl1 = new TeamSupport.DataManager.UserControls.CustomFieldsControl();
      this.tpTypes = new System.Windows.Forms.TabPage();
      this.customPropertiesControl1 = new TeamSupport.DataManager.UserControls.CustomPropertiesControl();
      this.tpQuery = new System.Windows.Forms.TabPage();
      this.queryExcelControl1 = new TeamSupport.DataManager.UserControls.QueryExcelControl();
      this.panel1.SuspendLayout();
      this.tabControl1.SuspendLayout();
      this.tpProperties.SuspendLayout();
      this.tpImport.SuspendLayout();
      this.tpFields.SuspendLayout();
      this.tpTypes.SuspendLayout();
      this.tpQuery.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnEditOrganization);
      this.panel1.Controls.Add(this.btnTestButton);
      this.panel1.Controls.Add(this.btnDeleteOrganization);
      this.panel1.Controls.Add(this.btnNewOrganization);
      this.panel1.Controls.Add(this.cmbOrganization);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(849, 30);
      this.panel1.TabIndex = 7;
      // 
      // btnEditOrganization
      // 
      this.btnEditOrganization.Location = new System.Drawing.Point(312, 2);
      this.btnEditOrganization.Name = "btnEditOrganization";
      this.btnEditOrganization.Size = new System.Drawing.Size(97, 23);
      this.btnEditOrganization.TabIndex = 11;
      this.btnEditOrganization.Text = "Edit Organization";
      this.btnEditOrganization.UseVisualStyleBackColor = true;
      this.btnEditOrganization.Click += new System.EventHandler(this.btnEditOrganization_Click);
      // 
      // btnTestButton
      // 
      this.btnTestButton.Location = new System.Drawing.Point(530, 2);
      this.btnTestButton.Name = "btnTestButton";
      this.btnTestButton.Size = new System.Drawing.Size(116, 23);
      this.btnTestButton.TabIndex = 10;
      this.btnTestButton.Text = "TEST BUTTON";
      this.btnTestButton.UseVisualStyleBackColor = true;
      this.btnTestButton.Click += new System.EventHandler(this.btnTestButton_Click);
      // 
      // btnDeleteOrganization
      // 
      this.btnDeleteOrganization.Location = new System.Drawing.Point(415, 2);
      this.btnDeleteOrganization.Name = "btnDeleteOrganization";
      this.btnDeleteOrganization.Size = new System.Drawing.Size(109, 23);
      this.btnDeleteOrganization.TabIndex = 9;
      this.btnDeleteOrganization.Text = "Delete Organization";
      this.btnDeleteOrganization.UseVisualStyleBackColor = true;
      this.btnDeleteOrganization.Click += new System.EventHandler(this.btnDeleteOrganization_Click);
      // 
      // btnNewOrganization
      // 
      this.btnNewOrganization.Location = new System.Drawing.Point(205, 2);
      this.btnNewOrganization.Name = "btnNewOrganization";
      this.btnNewOrganization.Size = new System.Drawing.Size(101, 23);
      this.btnNewOrganization.TabIndex = 8;
      this.btnNewOrganization.Text = "New Organization";
      this.btnNewOrganization.UseVisualStyleBackColor = true;
      this.btnNewOrganization.Click += new System.EventHandler(this.btnNewOrganization_Click);
      // 
      // cmbOrganization
      // 
      this.cmbOrganization.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbOrganization.FormattingEnabled = true;
      this.cmbOrganization.Location = new System.Drawing.Point(12, 3);
      this.cmbOrganization.Name = "cmbOrganization";
      this.cmbOrganization.Size = new System.Drawing.Size(187, 21);
      this.cmbOrganization.TabIndex = 7;
      this.cmbOrganization.SelectedIndexChanged += new System.EventHandler(this.cmbOrganization_SelectedIndexChanged);
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.tpProperties);
      this.tabControl1.Controls.Add(this.tpImport);
      this.tabControl1.Controls.Add(this.tpFields);
      this.tabControl1.Controls.Add(this.tpTypes);
      this.tabControl1.Controls.Add(this.tpQuery);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 30);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(849, 514);
      this.tabControl1.TabIndex = 8;
      // 
      // tpProperties
      // 
      this.tpProperties.Controls.Add(this.propertiesControl1);
      this.tpProperties.Location = new System.Drawing.Point(4, 22);
      this.tpProperties.Name = "tpProperties";
      this.tpProperties.Padding = new System.Windows.Forms.Padding(3);
      this.tpProperties.Size = new System.Drawing.Size(841, 488);
      this.tpProperties.TabIndex = 0;
      this.tpProperties.Text = "Properties";
      this.tpProperties.UseVisualStyleBackColor = true;
      // 
      // propertiesControl1
      // 
      this.propertiesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.propertiesControl1.Location = new System.Drawing.Point(3, 3);
      this.propertiesControl1.Name = "propertiesControl1";
      this.propertiesControl1.OrganizationID = 0;
      this.propertiesControl1.Size = new System.Drawing.Size(835, 482);
      this.propertiesControl1.TabIndex = 0;
      // 
      // tpImport
      // 
      this.tpImport.Controls.Add(this.importControl1);
      this.tpImport.Location = new System.Drawing.Point(4, 22);
      this.tpImport.Name = "tpImport";
      this.tpImport.Padding = new System.Windows.Forms.Padding(3);
      this.tpImport.Size = new System.Drawing.Size(841, 488);
      this.tpImport.TabIndex = 8;
      this.tpImport.Text = "Import";
      this.tpImport.UseVisualStyleBackColor = true;
      // 
      // importControl1
      // 
      this.importControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.importControl1.Location = new System.Drawing.Point(3, 3);
      this.importControl1.Name = "importControl1";
      this.importControl1.OrganizationID = 0;
      this.importControl1.Size = new System.Drawing.Size(835, 482);
      this.importControl1.TabIndex = 0;
      // 
      // tpFields
      // 
      this.tpFields.Controls.Add(this.customFieldsControl1);
      this.tpFields.Location = new System.Drawing.Point(4, 22);
      this.tpFields.Name = "tpFields";
      this.tpFields.Padding = new System.Windows.Forms.Padding(3);
      this.tpFields.Size = new System.Drawing.Size(841, 488);
      this.tpFields.TabIndex = 1;
      this.tpFields.Text = "Custom Fields";
      this.tpFields.UseVisualStyleBackColor = true;
      // 
      // customFieldsControl1
      // 
      this.customFieldsControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.customFieldsControl1.Location = new System.Drawing.Point(3, 3);
      this.customFieldsControl1.Name = "customFieldsControl1";
      this.customFieldsControl1.OrganizationID = 0;
      this.customFieldsControl1.Size = new System.Drawing.Size(835, 482);
      this.customFieldsControl1.TabIndex = 0;
      // 
      // tpTypes
      // 
      this.tpTypes.Controls.Add(this.customPropertiesControl1);
      this.tpTypes.Location = new System.Drawing.Point(4, 22);
      this.tpTypes.Name = "tpTypes";
      this.tpTypes.Padding = new System.Windows.Forms.Padding(3);
      this.tpTypes.Size = new System.Drawing.Size(841, 488);
      this.tpTypes.TabIndex = 2;
      this.tpTypes.Text = "Custom Properties";
      this.tpTypes.UseVisualStyleBackColor = true;
      // 
      // customPropertiesControl1
      // 
      this.customPropertiesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.customPropertiesControl1.Location = new System.Drawing.Point(3, 3);
      this.customPropertiesControl1.Name = "customPropertiesControl1";
      this.customPropertiesControl1.OrganizationID = 0;
      this.customPropertiesControl1.Size = new System.Drawing.Size(835, 482);
      this.customPropertiesControl1.TabIndex = 0;
      // 
      // tpQuery
      // 
      this.tpQuery.Controls.Add(this.queryExcelControl1);
      this.tpQuery.Location = new System.Drawing.Point(4, 22);
      this.tpQuery.Name = "tpQuery";
      this.tpQuery.Padding = new System.Windows.Forms.Padding(3);
      this.tpQuery.Size = new System.Drawing.Size(841, 488);
      this.tpQuery.TabIndex = 9;
      this.tpQuery.Text = "Spreadsheet Query";
      this.tpQuery.UseVisualStyleBackColor = true;
      // 
      // queryExcelControl1
      // 
      this.queryExcelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.queryExcelControl1.Location = new System.Drawing.Point(3, 3);
      this.queryExcelControl1.Name = "queryExcelControl1";
      this.queryExcelControl1.Size = new System.Drawing.Size(835, 482);
      this.queryExcelControl1.TabIndex = 0;
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = global::TeamSupport.DataManager.Properties.Settings.Default.MainFormClientSize;
      this.Controls.Add(this.tabControl1);
      this.Controls.Add(this.panel1);
      this.DataBindings.Add(new System.Windows.Forms.Binding("Location", global::TeamSupport.DataManager.Properties.Settings.Default, "MainFormLocation", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.DataBindings.Add(new System.Windows.Forms.Binding("ClientSize", global::TeamSupport.DataManager.Properties.Settings.Default, "MainFormClientSize", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.KeyPreview = true;
      this.Location = global::TeamSupport.DataManager.Properties.Settings.Default.MainFormLocation;
      this.Name = "MainForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Team Support Data Manager";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
      this.Load += new System.EventHandler(this.MainForm_Load);
      this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
      this.panel1.ResumeLayout(false);
      this.tabControl1.ResumeLayout(false);
      this.tpProperties.ResumeLayout(false);
      this.tpImport.ResumeLayout(false);
      this.tpFields.ResumeLayout(false);
      this.tpTypes.ResumeLayout(false);
      this.tpQuery.ResumeLayout(false);
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnEditOrganization;
    private System.Windows.Forms.Button btnTestButton;
    private System.Windows.Forms.Button btnDeleteOrganization;
    private System.Windows.Forms.Button btnNewOrganization;
    private System.Windows.Forms.ComboBox cmbOrganization;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tpProperties;
    private System.Windows.Forms.TabPage tpImport;
    private System.Windows.Forms.TabPage tpFields;
    private TeamSupport.DataManager.UserControls.CustomFieldsControl customFieldsControl1;
    private System.Windows.Forms.TabPage tpTypes;
    private System.Windows.Forms.TabPage tpQuery;
    private TeamSupport.DataManager.UserControls.QueryExcelControl queryExcelControl1;
    private TeamSupport.DataManager.UserControls.ImportControl importControl1;
    private TeamSupport.DataManager.UserControls.PropertiesControl propertiesControl1;
    private TeamSupport.DataManager.UserControls.CustomPropertiesControl customPropertiesControl1;

  }
}

