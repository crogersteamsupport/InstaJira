namespace BusinessObjectGenerator.Forms
{
  partial class FormSettings
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
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.btnOk = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.label5 = new System.Windows.Forms.Label();
      this.label6 = new System.Windows.Forms.Label();
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.textOutputFolder = new System.Windows.Forms.TextBox();
      this.label7 = new System.Windows.Forms.Label();
      this.btnOutputFolder = new System.Windows.Forms.Button();
      this.textBox5 = new System.Windows.Forms.TextBox();
      this.textBox6 = new System.Windows.Forms.TextBox();
      this.textBox3 = new System.Windows.Forms.TextBox();
      this.textBox4 = new System.Windows.Forms.TextBox();
      this.textBox2 = new System.Windows.Forms.TextBox();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.btnDevelopmentPath = new System.Windows.Forms.Button();
      this.label8 = new System.Windows.Forms.Label();
      this.textDevelopmentPath = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(149, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(83, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Modifier ID Field";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(149, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(98, 13);
      this.label2.TabIndex = 2;
      this.label2.Text = "Modified Date Field";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(12, 48);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(95, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Created Date Field";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(12, 9);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(80, 13);
      this.label4.TabIndex = 4;
      this.label4.Text = "Creator ID Field";
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(119, 282);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 8;
      this.btnOk.Text = "Save";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(200, 282);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 9;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // label5
      // 
      this.label5.AutoSize = true;
      this.label5.Location = new System.Drawing.Point(12, 100);
      this.label5.Name = "label5";
      this.label5.Size = new System.Drawing.Size(82, 13);
      this.label5.TabIndex = 12;
      this.label5.Text = "Base Item Class";
      // 
      // label6
      // 
      this.label6.AutoSize = true;
      this.label6.Location = new System.Drawing.Point(151, 100);
      this.label6.Name = "label6";
      this.label6.Size = new System.Drawing.Size(108, 13);
      this.label6.TabIndex = 10;
      this.label6.Text = "Base Collection Class";
      // 
      // textOutputFolder
      // 
      this.textOutputFolder.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "OutputPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textOutputFolder.Location = new System.Drawing.Point(13, 165);
      this.textOutputFolder.Name = "textOutputFolder";
      this.textOutputFolder.Size = new System.Drawing.Size(234, 20);
      this.textOutputFolder.TabIndex = 14;
      this.textOutputFolder.Text = global::BusinessObjectGenerator.AppSettings.Default.OutputPath;
      // 
      // label7
      // 
      this.label7.AutoSize = true;
      this.label7.Location = new System.Drawing.Point(10, 149);
      this.label7.Name = "label7";
      this.label7.Size = new System.Drawing.Size(71, 13);
      this.label7.TabIndex = 15;
      this.label7.Text = "Output Folder";
      // 
      // btnOutputFolder
      // 
      this.btnOutputFolder.Location = new System.Drawing.Point(254, 163);
      this.btnOutputFolder.Name = "btnOutputFolder";
      this.btnOutputFolder.Size = new System.Drawing.Size(27, 23);
      this.btnOutputFolder.TabIndex = 16;
      this.btnOutputFolder.Text = "...";
      this.btnOutputFolder.UseVisualStyleBackColor = true;
      this.btnOutputFolder.Click += new System.EventHandler(this.btnOutputFolder_Click);
      // 
      // textBox5
      // 
      this.textBox5.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "BaseCollectionClass", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox5.Location = new System.Drawing.Point(150, 116);
      this.textBox5.Name = "textBox5";
      this.textBox5.Size = new System.Drawing.Size(131, 20);
      this.textBox5.TabIndex = 13;
      this.textBox5.Text = global::BusinessObjectGenerator.AppSettings.Default.BaseCollectionClass;
      // 
      // textBox6
      // 
      this.textBox6.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "BaseItemClass", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox6.Location = new System.Drawing.Point(13, 116);
      this.textBox6.Name = "textBox6";
      this.textBox6.Size = new System.Drawing.Size(131, 20);
      this.textBox6.TabIndex = 11;
      this.textBox6.Text = global::BusinessObjectGenerator.AppSettings.Default.BaseItemClass;
      // 
      // textBox3
      // 
      this.textBox3.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "ModifiedDateField", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox3.Location = new System.Drawing.Point(150, 64);
      this.textBox3.Name = "textBox3";
      this.textBox3.Size = new System.Drawing.Size(131, 20);
      this.textBox3.TabIndex = 7;
      this.textBox3.Text = global::BusinessObjectGenerator.AppSettings.Default.ModifiedDateField;
      // 
      // textBox4
      // 
      this.textBox4.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "ModifierIDField", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox4.Location = new System.Drawing.Point(150, 25);
      this.textBox4.Name = "textBox4";
      this.textBox4.Size = new System.Drawing.Size(131, 20);
      this.textBox4.TabIndex = 5;
      this.textBox4.Text = global::BusinessObjectGenerator.AppSettings.Default.ModifierIDField;
      // 
      // textBox2
      // 
      this.textBox2.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "CreatedDateField", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox2.Location = new System.Drawing.Point(13, 64);
      this.textBox2.Name = "textBox2";
      this.textBox2.Size = new System.Drawing.Size(131, 20);
      this.textBox2.TabIndex = 3;
      this.textBox2.Text = global::BusinessObjectGenerator.AppSettings.Default.CreatedDateField;
      // 
      // textBox1
      // 
      this.textBox1.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "CreatorIDField", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textBox1.Location = new System.Drawing.Point(13, 25);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(131, 20);
      this.textBox1.TabIndex = 1;
      this.textBox1.Text = global::BusinessObjectGenerator.AppSettings.Default.CreatorIDField;
      // 
      // btnDevelopmentPath
      // 
      this.btnDevelopmentPath.Location = new System.Drawing.Point(256, 212);
      this.btnDevelopmentPath.Name = "btnDevelopmentPath";
      this.btnDevelopmentPath.Size = new System.Drawing.Size(27, 23);
      this.btnDevelopmentPath.TabIndex = 19;
      this.btnDevelopmentPath.Text = "...";
      this.btnDevelopmentPath.UseVisualStyleBackColor = true;
      this.btnDevelopmentPath.Click += new System.EventHandler(this.btnDevelopmentPath_Click);
      // 
      // label8
      // 
      this.label8.AutoSize = true;
      this.label8.Location = new System.Drawing.Point(12, 198);
      this.label8.Name = "label8";
      this.label8.Size = new System.Drawing.Size(102, 13);
      this.label8.TabIndex = 18;
      this.label8.Text = "Development Folder";
      // 
      // textDevelopmentPath
      // 
      this.textDevelopmentPath.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::BusinessObjectGenerator.AppSettings.Default, "DevelopmentPath", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textDevelopmentPath.Location = new System.Drawing.Point(15, 214);
      this.textDevelopmentPath.Name = "textDevelopmentPath";
      this.textDevelopmentPath.Size = new System.Drawing.Size(234, 20);
      this.textDevelopmentPath.TabIndex = 17;
      this.textDevelopmentPath.Text = global::BusinessObjectGenerator.AppSettings.Default.DevelopmentPath;
      // 
      // FormSettings
      // 
      this.AcceptButton = this.btnCancel;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnOk;
      this.ClientSize = new System.Drawing.Size(298, 317);
      this.Controls.Add(this.btnDevelopmentPath);
      this.Controls.Add(this.label8);
      this.Controls.Add(this.textDevelopmentPath);
      this.Controls.Add(this.btnOutputFolder);
      this.Controls.Add(this.label7);
      this.Controls.Add(this.textOutputFolder);
      this.Controls.Add(this.textBox5);
      this.Controls.Add(this.label5);
      this.Controls.Add(this.textBox6);
      this.Controls.Add(this.label6);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.textBox3);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.textBox4);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.textBox2);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textBox1);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "FormSettings";
      this.Text = "Settings";
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormSettings_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.TextBox textBox2;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textBox3;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox textBox4;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.TextBox textBox5;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textBox6;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.TextBox textOutputFolder;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.Button btnOutputFolder;
    private System.Windows.Forms.Button btnDevelopmentPath;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.TextBox textDevelopmentPath;
  }
}