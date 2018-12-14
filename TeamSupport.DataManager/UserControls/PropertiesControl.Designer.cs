namespace TeamSupport.DataManager.UserControls
{
  partial class PropertiesControl
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
      this.gridProperties = new Telerik.WinControls.UI.RadGridView();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties.MasterGridViewTemplate)).BeginInit();
      this.SuspendLayout();
      // 
      // gridProperties
      // 
      this.gridProperties.BackColor = System.Drawing.SystemColors.Control;
      this.gridProperties.Cursor = System.Windows.Forms.Cursors.Default;
      this.gridProperties.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridProperties.EnableAlternatingRowColor = true;
      this.gridProperties.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.gridProperties.ForeColor = System.Drawing.SystemColors.ControlText;
      this.gridProperties.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gridProperties.Location = new System.Drawing.Point(0, 0);
      // 
      // 
      // 
      this.gridProperties.MasterGridViewTemplate.AllowAddNewRow = false;
      this.gridProperties.MasterGridViewTemplate.AllowColumnChooser = false;
      this.gridProperties.MasterGridViewTemplate.AllowColumnReorder = false;
      this.gridProperties.MasterGridViewTemplate.AllowDeleteRow = false;
      this.gridProperties.MasterGridViewTemplate.AllowDragToGroup = false;
      this.gridProperties.MasterGridViewTemplate.AllowEditRow = false;
      this.gridProperties.MasterGridViewTemplate.AllowRowResize = false;
      this.gridProperties.MasterGridViewTemplate.AutoSizeColumnsMode = Telerik.WinControls.UI.GridViewAutoSizeColumnsMode.Fill;
      gridViewTextBoxColumn1.AllowGroup = false;
      gridViewTextBoxColumn1.FieldAlias = "column1";
      gridViewTextBoxColumn1.FieldName = "Property";
      gridViewTextBoxColumn1.HeaderText = "Property";
      gridViewTextBoxColumn1.SortOrder = Telerik.WinControls.UI.RadSortOrder.Ascending;
      gridViewTextBoxColumn1.UniqueName = "Property";
      gridViewTextBoxColumn1.Width = 415;
      gridViewTextBoxColumn2.FieldAlias = "column2";
      gridViewTextBoxColumn2.FieldName = "Value";
      gridViewTextBoxColumn2.HeaderText = "Value";
      gridViewTextBoxColumn2.UniqueName = "Value";
      gridViewTextBoxColumn2.Width = 285;
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn1);
      this.gridProperties.MasterGridViewTemplate.Columns.Add(gridViewTextBoxColumn2);
      this.gridProperties.MasterGridViewTemplate.EnableGrouping = false;
      this.gridProperties.MasterGridViewTemplate.ShowRowHeaderColumn = false;
      this.gridProperties.Name = "gridProperties";
      this.gridProperties.ReadOnly = true;
      this.gridProperties.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.gridProperties.ShowGroupPanel = false;
      this.gridProperties.Size = new System.Drawing.Size(705, 523);
      this.gridProperties.TabIndex = 0;
      this.gridProperties.Text = "radGridViewPreview";
      // 
      // PropertiesControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.gridProperties);
      this.Name = "PropertiesControl";
      this.Size = new System.Drawing.Size(705, 523);
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties.MasterGridViewTemplate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridProperties)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private Telerik.WinControls.UI.RadGridView gridProperties;
  }
}
