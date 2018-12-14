namespace TeamSupport.DataManager
{
  partial class CustomFieldForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomFieldForm));
      this.cmbType = new System.Windows.Forms.ComboBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.lblList = new System.Windows.Forms.Label();
      this.textName = new System.Windows.Forms.TextBox();
      this.textDescription = new System.Windows.Forms.TextBox();
      this.textList = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // cmbType
      // 
      this.cmbType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbType.FormattingEnabled = true;
      this.cmbType.Location = new System.Drawing.Point(12, 103);
      this.cmbType.Name = "cmbType";
      this.cmbType.Size = new System.Drawing.Size(225, 21);
      this.cmbType.TabIndex = 0;
      this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(162, 179);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(81, 179);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 2;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 3;
      this.label1.Text = "Name";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 87);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(56, 13);
      this.label2.TabIndex = 4;
      this.label2.Text = "Field Type";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(9, 48);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(60, 13);
      this.label3.TabIndex = 5;
      this.label3.Text = "Description";
      // 
      // lblList
      // 
      this.lblList.AutoSize = true;
      this.lblList.Location = new System.Drawing.Point(9, 127);
      this.lblList.Name = "lblList";
      this.lblList.Size = new System.Drawing.Size(82, 13);
      this.lblList.TabIndex = 6;
      this.lblList.Text = "Pick List Values";
      this.lblList.Visible = false;
      // 
      // textName
      // 
      this.textName.Location = new System.Drawing.Point(12, 25);
      this.textName.Name = "textName";
      this.textName.Size = new System.Drawing.Size(225, 20);
      this.textName.TabIndex = 7;
      // 
      // textDescription
      // 
      this.textDescription.Location = new System.Drawing.Point(12, 64);
      this.textDescription.Name = "textDescription";
      this.textDescription.Size = new System.Drawing.Size(225, 20);
      this.textDescription.TabIndex = 8;
      // 
      // textList
      // 
      this.textList.Location = new System.Drawing.Point(12, 143);
      this.textList.Name = "textList";
      this.textList.Size = new System.Drawing.Size(225, 20);
      this.textList.TabIndex = 9;
      this.textList.Visible = false;
      // 
      // CustomFieldForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(248, 217);
      this.Controls.Add(this.textList);
      this.Controls.Add(this.textDescription);
      this.Controls.Add(this.textName);
      this.Controls.Add(this.lblList);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.cmbType);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "CustomFieldForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Custom Field";
      this.Load += new System.EventHandler(this.CustomFieldForm_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomFieldForm_FormClosing);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ComboBox cmbType;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label lblList;
    private System.Windows.Forms.TextBox textName;
    private System.Windows.Forms.TextBox textDescription;
    private System.Windows.Forms.TextBox textList;
  }
}