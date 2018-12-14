namespace TeamSupport.DataManager.UserControls
{
  partial class QueryExcelControl
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.label2 = new System.Windows.Forms.Label();
      this.btnBlob = new System.Windows.Forms.Button();
      this.button1 = new System.Windows.Forms.Button();
      this.btnExportSingleColumn = new System.Windows.Forms.Button();
      this.btnSaveQuery = new System.Windows.Forms.Button();
      this.btnOpenQuery = new System.Windows.Forms.Button();
      this.btnExport = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.lblSource = new System.Windows.Forms.Label();
      this.btnExecute = new System.Windows.Forms.Button();
      this.btnOpen = new System.Windows.Forms.Button();
      this.textQuery = new System.Windows.Forms.TextBox();
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.lblCount = new System.Windows.Forms.ToolStripStatusLabel();
      this.tabControl = new System.Windows.Forms.TabControl();
      this.tpResults = new System.Windows.Forms.TabPage();
      this.gridResults = new Telerik.WinControls.UI.RadGridView();
      this.tpMessages = new System.Windows.Forms.TabPage();
      this.textResults = new System.Windows.Forms.TextBox();
      this.panel1.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      this.tabControl.SuspendLayout();
      this.tpResults.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridResults)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridResults.MasterGridViewTemplate)).BeginInit();
      this.tpMessages.SuspendLayout();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.label2);
      this.panel1.Controls.Add(this.btnBlob);
      this.panel1.Controls.Add(this.button1);
      this.panel1.Controls.Add(this.btnExportSingleColumn);
      this.panel1.Controls.Add(this.btnSaveQuery);
      this.panel1.Controls.Add(this.btnOpenQuery);
      this.panel1.Controls.Add(this.btnExport);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Controls.Add(this.lblSource);
      this.panel1.Controls.Add(this.btnExecute);
      this.panel1.Controls.Add(this.btnOpen);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(918, 104);
      this.panel1.TabIndex = 4;
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(83, 60);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(253, 13);
      this.label2.TabIndex = 15;
      this.label2.Text = "Export Query: SELECT Root, Folder, FileName, Blob";
      // 
      // btnBlob
      // 
      this.btnBlob.Location = new System.Drawing.Point(2, 55);
      this.btnBlob.Name = "btnBlob";
      this.btnBlob.Size = new System.Drawing.Size(75, 23);
      this.btnBlob.TabIndex = 14;
      this.btnBlob.Text = "Export Blobs";
      this.btnBlob.UseVisualStyleBackColor = true;
      this.btnBlob.Click += new System.EventHandler(this.btnBlob_Click);
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(124, 26);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(75, 23);
      this.button1.TabIndex = 13;
      this.button1.Text = "Open MySql";
      this.button1.UseVisualStyleBackColor = true;
      this.button1.Click += new System.EventHandler(this.button1_Click);
      // 
      // btnExportSingleColumn
      // 
      this.btnExportSingleColumn.Location = new System.Drawing.Point(557, 26);
      this.btnExportSingleColumn.Name = "btnExportSingleColumn";
      this.btnExportSingleColumn.Size = new System.Drawing.Size(150, 23);
      this.btnExportSingleColumn.TabIndex = 12;
      this.btnExportSingleColumn.Text = "Export As Single Column";
      this.btnExportSingleColumn.UseVisualStyleBackColor = true;
      this.btnExportSingleColumn.Click += new System.EventHandler(this.btnExportSingleColumn_Click);
      // 
      // btnSaveQuery
      // 
      this.btnSaveQuery.Location = new System.Drawing.Point(286, 26);
      this.btnSaveQuery.Name = "btnSaveQuery";
      this.btnSaveQuery.Size = new System.Drawing.Size(75, 23);
      this.btnSaveQuery.TabIndex = 11;
      this.btnSaveQuery.Text = "Save Query";
      this.btnSaveQuery.UseVisualStyleBackColor = true;
      this.btnSaveQuery.Click += new System.EventHandler(this.btnSaveQuery_Click);
      // 
      // btnOpenQuery
      // 
      this.btnOpenQuery.Location = new System.Drawing.Point(205, 26);
      this.btnOpenQuery.Name = "btnOpenQuery";
      this.btnOpenQuery.Size = new System.Drawing.Size(75, 23);
      this.btnOpenQuery.TabIndex = 10;
      this.btnOpenQuery.Text = "Open Query";
      this.btnOpenQuery.UseVisualStyleBackColor = true;
      this.btnOpenQuery.Click += new System.EventHandler(this.btnOpenQuery_Click);
      // 
      // btnExport
      // 
      this.btnExport.Location = new System.Drawing.Point(464, 26);
      this.btnExport.Name = "btnExport";
      this.btnExport.Size = new System.Drawing.Size(86, 23);
      this.btnExport.TabIndex = 9;
      this.btnExport.Text = "Export Results";
      this.btnExport.UseVisualStyleBackColor = true;
      this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.label1.Location = new System.Drawing.Point(2, 8);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(51, 13);
      this.label1.TabIndex = 8;
      this.label1.Text = "Source:";
      // 
      // lblSource
      // 
      this.lblSource.AutoSize = true;
      this.lblSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.lblSource.Location = new System.Drawing.Point(59, 8);
      this.lblSource.Name = "lblSource";
      this.lblSource.Size = new System.Drawing.Size(129, 13);
      this.lblSource.TabIndex = 7;
      this.lblSource.Text = "[No Source Selected]";
      // 
      // btnExecute
      // 
      this.btnExecute.Location = new System.Drawing.Point(368, 26);
      this.btnExecute.Name = "btnExecute";
      this.btnExecute.Size = new System.Drawing.Size(90, 23);
      this.btnExecute.TabIndex = 6;
      this.btnExecute.Text = "Execute Query";
      this.btnExecute.UseVisualStyleBackColor = true;
      this.btnExecute.Click += new System.EventHandler(this.btnExecute_Click);
      // 
      // btnOpen
      // 
      this.btnOpen.Location = new System.Drawing.Point(2, 26);
      this.btnOpen.Name = "btnOpen";
      this.btnOpen.Size = new System.Drawing.Size(116, 23);
      this.btnOpen.TabIndex = 5;
      this.btnOpen.Text = "Open Spreadsheet";
      this.btnOpen.UseVisualStyleBackColor = true;
      this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
      // 
      // textQuery
      // 
      this.textQuery.Dock = System.Windows.Forms.DockStyle.Top;
      this.textQuery.Font = new System.Drawing.Font("Courier New", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
      this.textQuery.Location = new System.Drawing.Point(0, 104);
      this.textQuery.Multiline = true;
      this.textQuery.Name = "textQuery";
      this.textQuery.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.textQuery.Size = new System.Drawing.Size(918, 125);
      this.textQuery.TabIndex = 6;
      this.textQuery.TextChanged += new System.EventHandler(this.textQuery_TextChanged);
      // 
      // splitter1
      // 
      this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
      this.splitter1.Location = new System.Drawing.Point(0, 229);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(918, 10);
      this.splitter1.TabIndex = 9;
      this.splitter1.TabStop = false;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCount});
      this.statusStrip1.Location = new System.Drawing.Point(0, 539);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Size = new System.Drawing.Size(918, 22);
      this.statusStrip1.TabIndex = 11;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // lblCount
      // 
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(0, 17);
      // 
      // tabControl
      // 
      this.tabControl.Controls.Add(this.tpResults);
      this.tabControl.Controls.Add(this.tpMessages);
      this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl.Location = new System.Drawing.Point(0, 239);
      this.tabControl.Name = "tabControl";
      this.tabControl.SelectedIndex = 0;
      this.tabControl.Size = new System.Drawing.Size(918, 300);
      this.tabControl.TabIndex = 12;
      // 
      // tpResults
      // 
      this.tpResults.Controls.Add(this.gridResults);
      this.tpResults.Location = new System.Drawing.Point(4, 22);
      this.tpResults.Name = "tpResults";
      this.tpResults.Padding = new System.Windows.Forms.Padding(3);
      this.tpResults.Size = new System.Drawing.Size(910, 274);
      this.tpResults.TabIndex = 0;
      this.tpResults.Text = "Results";
      this.tpResults.UseVisualStyleBackColor = true;
      // 
      // gridResults
      // 
      this.gridResults.BackColor = System.Drawing.Color.Transparent;
      this.gridResults.Cursor = System.Windows.Forms.Cursors.Default;
      this.gridResults.Dock = System.Windows.Forms.DockStyle.Fill;
      this.gridResults.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
      this.gridResults.ForeColor = System.Drawing.SystemColors.ControlText;
      this.gridResults.ImeMode = System.Windows.Forms.ImeMode.NoControl;
      this.gridResults.Location = new System.Drawing.Point(3, 3);
      // 
      // 
      // 
      this.gridResults.MasterGridViewTemplate.AllowAddNewRow = false;
      this.gridResults.MasterGridViewTemplate.AllowDeleteRow = false;
      this.gridResults.MasterGridViewTemplate.AllowEditRow = false;
      this.gridResults.MasterGridViewTemplate.ShowRowHeaderColumn = false;
      this.gridResults.Name = "gridResults";
      this.gridResults.ReadOnly = true;
      this.gridResults.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.gridResults.Size = new System.Drawing.Size(904, 268);
      this.gridResults.TabIndex = 8;
      this.gridResults.Text = "radGridViewPreview";
      // 
      // tpMessages
      // 
      this.tpMessages.Controls.Add(this.textResults);
      this.tpMessages.Location = new System.Drawing.Point(4, 22);
      this.tpMessages.Name = "tpMessages";
      this.tpMessages.Padding = new System.Windows.Forms.Padding(3);
      this.tpMessages.Size = new System.Drawing.Size(910, 274);
      this.tpMessages.TabIndex = 1;
      this.tpMessages.Text = "Messages";
      this.tpMessages.UseVisualStyleBackColor = true;
      // 
      // textResults
      // 
      this.textResults.Dock = System.Windows.Forms.DockStyle.Fill;
      this.textResults.Location = new System.Drawing.Point(3, 3);
      this.textResults.Multiline = true;
      this.textResults.Name = "textResults";
      this.textResults.Size = new System.Drawing.Size(904, 268);
      this.textResults.TabIndex = 7;
      // 
      // QueryExcelControl
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabControl);
      this.Controls.Add(this.statusStrip1);
      this.Controls.Add(this.splitter1);
      this.Controls.Add(this.textQuery);
      this.Controls.Add(this.panel1);
      this.Name = "QueryExcelControl";
      this.Size = new System.Drawing.Size(918, 561);
      this.Load += new System.EventHandler(this.QueryExcelControl_Load);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      this.tabControl.ResumeLayout(false);
      this.tpResults.ResumeLayout(false);
      this.tpResults.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.gridResults.MasterGridViewTemplate)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.gridResults)).EndInit();
      this.tpMessages.ResumeLayout(false);
      this.tpMessages.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnOpen;
    private System.Windows.Forms.Button btnExecute;
    private System.Windows.Forms.Label lblSource;
    private System.Windows.Forms.TextBox textQuery;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.Button btnExport;
    private System.Windows.Forms.Button btnSaveQuery;
    private System.Windows.Forms.Button btnOpenQuery;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripStatusLabel lblCount;
    private System.Windows.Forms.TabControl tabControl;
    private System.Windows.Forms.TabPage tpResults;
    private Telerik.WinControls.UI.RadGridView gridResults;
    private System.Windows.Forms.TabPage tpMessages;
    private System.Windows.Forms.TextBox textResults;
    private System.Windows.Forms.Button btnExportSingleColumn;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btnBlob;
    private System.Windows.Forms.Label label2;

  }
}
