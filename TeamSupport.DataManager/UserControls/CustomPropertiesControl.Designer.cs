namespace TeamSupport.DataManager.UserControls
{
  partial class CustomPropertiesControl
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
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn1 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn2 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn3 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn4 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn5 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      Telerik.WinControls.UI.GridViewTextBoxColumn gridViewTextBoxColumn6 = new Telerik.WinControls.UI.GridViewTextBoxColumn();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnDelete = new System.Windows.Forms.Button();
      this.lblTicketType = new System.Windows.Forms.Label();
      this.cmbTicketType = new System.Windows.Forms.ComboBox();
      this.btnDown = new System.Windows.Forms.Button();
      this.btnUp = new System.Windows.Forms.Button();
      this.btnEdit = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.btnNew = new System.Windows.Forms.Button();
      this.cmbPropertyTypes = new System.Windows.Forms.ComboBox();
      this.gridProperties = new Telerik.WinControls.UI.RadGridView();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties.MasterGridViewTemplate)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnDelete);
      this.panel1.Controls.Add(this.lblTicketType);
      this.panel1.Controls.Add(this.cmbTicketType);
      this.panel1.Controls.Add(this.btnDown);
      this.panel1.Controls.Add(this.btnUp);
      this.panel1.Controls.Add(this.btnEdit);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.btnNew);
      this.panel1.Controls.Add(this.cmbPropertyTypes);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(800, 61);
      this.panel1.TabIndex = 3;
      // 
      // btnDelete
      // 
      this.btnDelete.Location = new System.Drawing.Point(259, 30);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(147, 23);
      this.btnDelete.TabIndex = 8;
      this.btnDelete.Text = "Delete Selected Property";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // lblTicketType
      // 
      this.lblTicketType.AutoSize = true;
      this.lblTicketType.Location = new System.Drawing.Point(228, 6);
      this.lblTicketType.Name = "lblTicketType";
      this.lblTicketType.Size = new System.Drawing.Size(67, 13);
      this.lblTicketType.TabIndex = 7;
      this.lblTicketType.Text = "Ticket Type:";
      // 
      // cmbTicketType
      // 
      this.cmbTicketType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbTicketType.FormattingEnabled = true;
      this.cmbTicketType.Location = new System.Drawing.Point(301, 3);
      this.cmbTicketType.Name = "cmbTicketType";
      this.cmbTicketType.Size = new System.Drawing.Size(121, 21);
      this.cmbTicketType.TabIndex = 6;
      this.cmbTicketType.SelectedIndexChanged += new System.EventHandler(this.cmbPropertyTypes_SelectedIndexChanged);
      // 
      // btnDown
      // 
      this.btnDown.Location = new System.Drawing.Point(518, 30);
      this.btnDown.Name = "btnDown";
      this.btnDown.Size = new System.Drawing.Size(75, 23);
      this.btnDown.TabIndex = 5;
      this.btnDown.Text = "Move Down";
      this.btnDown.UseVisualStyleBackColor = true;
      this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
      // 
      // btnUp
      // 
      this.btnUp.Location = new System.Drawing.Point(437, 30);
      this.btnUp.Name = "btnUp";
      this.btnUp.Size = new System.Drawing.Size(75, 23);
      this.btnUp.TabIndex = 4;
      this.btnUp.Text = "Move Up";
      this.btnUp.UseVisualStyleBackColor = true;
      this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
      // 
      // btnEdit
      // 
      this.btnEdit.Location = new System.Drawing.Point(129, 30);
      this.btnEdit.Name = "btnEdit";
      this.btnEdit.Size = new System.Drawing.Size(124, 23);
      this.btnEdit.TabIndex = 3;
      this.btnEdit.Text = "Edit Selected Property";
      this.btnEdit.UseVisualStyleBackColor = true;
      this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(76, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Property Type:";
      // 
      // btnNew
      // 
      this.btnNew.Location = new System.Drawing.Point(6, 30);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(117, 23);
      this.btnNew.TabIndex = 1;
      this.btnNew.Text = "New Custom Property";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // cmbPropertyTypes
      // 
      this.cmbPropertyTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbPropertyTypes.FormattingEnabled = true;
      this.cmbPropertyTypes.Location = new System.Drawing.Point(85, 3);
      this.cmbPropertyTypes.Name = "cmbPropertyTypes";
      this.cmbPropertyTypes.Size = new System.Drawing.Size(137, 21);
      this.cmbPropertyTypes.TabIndex = 0;
      this.cmbPropertyTypes.SelectedIndexChanged += new System.EventHandler(this.cmbPropertyTypes_SelectedIndexChanged);
      // 
      // gridProperties
      // 
      this.gridProperties.BackColor = System.Drawing.SystemColors.Control;
      this.gridProperties.Cursor = System.Windows.Forms.Cursors.Default;
      this.gridProperties.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.gridProperties.ForeColor = System.Drawing.SystemColors.ControlText;
      this.gridProperties.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gridProperties.Location = new System.Drawing.Point(0, 61);
      // 
      // 
      // 
      this.gridProperties.MasterGridViewTemplate.AllowAddNewRow = false;
      this.gridProperties.MasterGridViewTemplate.AllowColumnChooser = false;
      this.gridProperties.MasterGridViewTemplate.AllowDragToGroup = false;
      gridViewTextBoxColumn1.FieldAlias = "column1";
      gridViewTextBoxColumn1.FieldName = "ID";
      gridViewTextBoxColumn1.HeaderText = "ID";
      gridViewTextBoxColumn1.IsVisible = false;
      gridViewTextBoxColumn1.UniqueName = "ID";
      gridViewTextBoxColumn1.Width = 169;
      gridViewTextBoxColumn2.FieldAlias = "column1";
      gridViewTextBoxColumn2.FieldName = "Name";
      gridViewTextBoxColumn2.HeaderText = "Name";
      gridViewTextBoxColumn2.UniqueName = "Name";
      gridViewTextBoxColumn2.Width = 238;
      gridViewTextBoxColumn3.FieldAlias = "column2";
      gridViewTextBoxColumn3.FieldName = "Description";
      gridViewTextBoxColumn3.HeaderText = "Description";
      gridViewTextBoxColumn3.UniqueName = "Description";
      gridViewTextBoxColumn3.Width = 141;
      gridViewTextBoxColumn4.FieldAlias = "IsClosed";
      gridViewTextBoxColumn4.FieldName = "IsClosed";
      gridViewTextBoxColumn4.HeaderText = "Closed";
      gridViewTextBoxColumn4.UniqueName = "IsClosed";
      gridViewTextBoxColumn4.Width = 89;
      gridViewTextBoxColumn5.FieldAlias = "IsShipping";
      gridViewTextBoxColumn5.FieldName = "IsShipping";
      gridViewTextBoxColumn5.HeaderText = "Shipping";
      gridViewTextBoxColumn5.UniqueName = "IsShipping";
      gridViewTextBoxColumn5.Width = 100;
      gridViewTextBoxColumn6.FieldAlias = "IsDiscontinued";
      gridViewTextBoxColumn6.FieldName = "IsDiscontinued";
      gridViewTextBoxColumn6.HeaderText = "Discontinued";
      gridViewTextBoxColumn6.UniqueName = "IsDiscontinued";
      gridViewTextBoxColumn6.Width = 164;
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn3);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn4);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn5);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn6);
      this.gridProperties.MasterGridViewTemplate.EnableGrouping = false;
      this.gridProperties.MasterGridViewTemplate.EnableSorting = false;
      this.gridProperties.MasterGridViewTemplate.ShowRowHeaderColumn = false;
      this.gridProperties.Name = "gridProperties";
      this.gridProperties.ReadOnly = true;
      this.gridProperties.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.gridProperties.ShowGroupPanel = false;
      this.gridProperties.Size = new System.Drawing.Size(800, 527);
      this.gridProperties.TabIndex = 4;
      this.gridProperties.Text = "radGridViewPreview";
      this.gridProperties.DoubleClick += new System.EventHandler(this.gridProperties_DoubleClick);
      // 
      // CustomPropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gridProperties);
      this.Controls.Add(this.panel1);
      this.Name = "CustomPropertiesControl";
      this.Size = new System.Drawing.Size(800, 588);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties.MasterGridViewTemplate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.Label lblTicketType;
    private System.Windows.Forms.ComboBox cmbTicketType;
    private System.Windows.Forms.Button btnDown;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.Button btnEdit;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.ComboBox cmbPropertyTypes;
    private Telerik.WinControls.UI.RadGridView gridProperties;

  }
}
