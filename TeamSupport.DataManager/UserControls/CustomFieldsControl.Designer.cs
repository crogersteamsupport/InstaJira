namespace TeamSupport.DataManager.UserControls
{
  partial class CustomFieldsControl
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.lblTicketType = new System.Windows.Forms.Label();
      this.cmbTicketType = new System.Windows.Forms.ComboBox();
      this.btnDown = new System.Windows.Forms.Button();
      this.btnUp = new System.Windows.Forms.Button();
      this.btnEdit = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.btnNew = new System.Windows.Forms.Button();
      this.cmbFieldTypes = new System.Windows.Forms.ComboBox();
      this.gridFields = new Telerik.WinControls.UI.RadGridView();
      this.btnDelete = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridFields)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridFields.MasterGridViewTemplate)).BeginInit();
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
      this.panel1.Controls.Add(this.cmbFieldTypes);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(574, 61);
      this.panel1.TabIndex = 1;
      // 
      // lblTicketType
      // 
      this.lblTicketType.AutoSize = true;
      this.lblTicketType.Location = new System.Drawing.Point(209, 6);
      this.lblTicketType.Name = "lblTicketType";
      this.lblTicketType.Size = new System.Drawing.Size(67, 13);
      this.lblTicketType.TabIndex = 7;
      this.lblTicketType.Text = "Ticket Type:";
      // 
      // cmbTicketType
      // 
      this.cmbTicketType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbTicketType.FormattingEnabled = true;
      this.cmbTicketType.Location = new System.Drawing.Point(282, 3);
      this.cmbTicketType.Name = "cmbTicketType";
      this.cmbTicketType.Size = new System.Drawing.Size(121, 21);
      this.cmbTicketType.TabIndex = 6;
      this.cmbTicketType.SelectedIndexChanged += new System.EventHandler(this.cmbTypes_SelectedIndexChanged);
      // 
      // btnDown
      // 
      this.btnDown.Location = new System.Drawing.Point(451, 30);
      this.btnDown.Name = "btnDown";
      this.btnDown.Size = new System.Drawing.Size(75, 23);
      this.btnDown.TabIndex = 5;
      this.btnDown.Text = "Move Down";
      this.btnDown.UseVisualStyleBackColor = true;
      this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
      // 
      // btnUp
      // 
      this.btnUp.Location = new System.Drawing.Point(370, 30);
      this.btnUp.Name = "btnUp";
      this.btnUp.Size = new System.Drawing.Size(75, 23);
      this.btnUp.TabIndex = 4;
      this.btnUp.Text = "Move Up";
      this.btnUp.UseVisualStyleBackColor = true;
      this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
      // 
      // btnEdit
      // 
      this.btnEdit.Location = new System.Drawing.Point(115, 30);
      this.btnEdit.Name = "btnEdit";
      this.btnEdit.Size = new System.Drawing.Size(110, 23);
      this.btnEdit.TabIndex = 3;
      this.btnEdit.Text = "Edit Selected Field";
      this.btnEdit.UseVisualStyleBackColor = true;
      this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(3, 6);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(59, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Field Type:";
      // 
      // btnNew
      // 
      this.btnNew.Location = new System.Drawing.Point(6, 30);
      this.btnNew.Name = "btnNew";
      this.btnNew.Size = new System.Drawing.Size(103, 23);
      this.btnNew.TabIndex = 1;
      this.btnNew.Text = "New Custom Field";
      this.btnNew.UseVisualStyleBackColor = true;
      this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
      // 
      // cmbFieldTypes
      // 
      this.cmbFieldTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbFieldTypes.FormattingEnabled = true;
      this.cmbFieldTypes.Location = new System.Drawing.Point(68, 3);
      this.cmbFieldTypes.Name = "cmbFieldTypes";
      this.cmbFieldTypes.Size = new System.Drawing.Size(121, 21);
      this.cmbFieldTypes.TabIndex = 0;
      this.cmbFieldTypes.SelectedIndexChanged += new System.EventHandler(this.cmbTypes_SelectedIndexChanged);
      // 
      // gridFields
      // 
      this.gridFields.BackColor = System.Drawing.SystemColors.Control;
      this.gridFields.Cursor = System.Windows.Forms.Cursors.Arrow;
      this.gridFields.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridFields.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.gridFields.ForeColor = System.Drawing.SystemColors.ControlText;
      this.gridFields.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gridFields.Location = new System.Drawing.Point(0, 61);
      // 
      // 
      // 
      this.gridFields.MasterGridViewTemplate.AllowAddNewRow = false;
      this.gridFields.MasterGridViewTemplate.AllowColumnChooser = false;
      this.gridFields.MasterGridViewTemplate.AllowColumnReorder = false;
      this.gridFields.MasterGridViewTemplate.AllowDeleteRow = false;
      this.gridFields.MasterGridViewTemplate.AllowDragToGroup = false;
      this.gridFields.MasterGridViewTemplate.AllowEditRow = false;
      this.gridFields.MasterGridViewTemplate.AllowRowResize = false;
      this.gridFields.MasterGridViewTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
      gridViewTextBoxColumn1.FieldAlias = "CustomFieldID";
      gridViewTextBoxColumn1.FieldName = "CustomFieldID";
      gridViewTextBoxColumn1.HeaderText = "CustomFieldID";
      gridViewTextBoxColumn1.IsVisible = false;
      gridViewTextBoxColumn1.UniqueName = "CustomFieldID";
      gridViewTextBoxColumn1.Width = 48;
      gridViewTextBoxColumn2.FieldAlias = "column1";
      gridViewTextBoxColumn2.FieldName = "Name";
      gridViewTextBoxColumn2.HeaderText = "Name";
      gridViewTextBoxColumn2.UniqueName = "Name";
      gridViewTextBoxColumn2.Width = 284;
      gridViewTextBoxColumn3.AllowGroup = false;
      gridViewTextBoxColumn3.FieldAlias = "column2";
      gridViewTextBoxColumn3.FieldName = "DataType";
      gridViewTextBoxColumn3.HeaderText = "Data Type";
      gridViewTextBoxColumn3.UniqueName = "DataType";
      gridViewTextBoxColumn3.Width = 284;
      this.gridFields.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
      this.gridFields.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
      this.gridFields.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn3);
      this.gridFields.MasterGridViewTemplate.EnableGrouping = false;
      this.gridFields.MasterGridViewTemplate.EnableSorting = false;
      this.gridFields.MasterGridViewTemplate.ShowRowHeaderColumn = false;
      this.gridFields.Name = "gridFields";
      this.gridFields.ReadOnly = true;
      this.gridFields.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.gridFields.ShowGroupPanel = false;
      this.gridFields.Size = new System.Drawing.Size(574, 361);
      this.gridFields.TabIndex = 2;
      this.gridFields.Text = "radGridViewPreview";
      this.gridFields.ThemeName = "ControlDefault";
      this.gridFields.DoubleClick += new System.EventHandler(this.gridFields_DoubleClick);
      // 
      // btnDelete
      // 
      this.btnDelete.Location = new System.Drawing.Point(231, 30);
      this.btnDelete.Name = "btnDelete";
      this.btnDelete.Size = new System.Drawing.Size(120, 23);
      this.btnDelete.TabIndex = 8;
      this.btnDelete.Text = "Delete Selected Field";
      this.btnDelete.UseVisualStyleBackColor = true;
      this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
      // 
      // CustomFieldsControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gridFields);
      this.Controls.Add(this.panel1);
      this.Name = "CustomFieldsControl";
      this.Size = new System.Drawing.Size(574, 422);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridFields.MasterGridViewTemplate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridFields)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private Telerik.WinControls.UI.RadGridView gridFields;
    private System.Windows.Forms.Button btnDown;
    private System.Windows.Forms.Button btnUp;
    private System.Windows.Forms.Button btnEdit;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Button btnNew;
    private System.Windows.Forms.ComboBox cmbFieldTypes;
    private System.Windows.Forms.Label lblTicketType;
    private System.Windows.Forms.ComboBox cmbTicketType;
    private System.Windows.Forms.Button btnDelete;

  }
}
