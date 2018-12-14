namespace TeamSupport.DataManager
{
  partial class CustomPropertyForm
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CustomPropertyForm));
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.textDescription = new System.Windows.Forms.TextBox();
      this.textName = new System.Windows.Forms.TextBox();
      this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
      this.cbClosed = new System.Windows.Forms.CheckBox();
      this.cbShipping = new System.Windows.Forms.CheckBox();
      this.cbDiscontinued = new System.Windows.Forms.CheckBox();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.flowLayoutPanel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(9, 9);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(35, 13);
      this.label1.TabIndex = 0;
      this.label1.Text = "Name";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(9, 48);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(60, 13);
      this.label2.TabIndex = 1;
      this.label2.Text = "Description";
      // 
      // textDescription
      // 
      this.textDescription.Location = new System.Drawing.Point(12, 64);
      this.textDescription.Name = "textDescription";
      this.textDescription.Size = new System.Drawing.Size(287, 20);
      this.textDescription.TabIndex = 2;
      // 
      // textName
      // 
      this.textName.Location = new System.Drawing.Point(12, 25);
      this.textName.Name = "textName";
      this.textName.Size = new System.Drawing.Size(287, 20);
      this.textName.TabIndex = 3;
      // 
      // flowLayoutPanel1
      // 
      this.flowLayoutPanel1.Controls.Add(this.cbClosed);
      this.flowLayoutPanel1.Controls.Add(this.cbShipping);
      this.flowLayoutPanel1.Controls.Add(this.cbDiscontinued);
      this.flowLayoutPanel1.Location = new System.Drawing.Point(12, 90);
      this.flowLayoutPanel1.Name = "flowLayoutPanel1";
      this.flowLayoutPanel1.Size = new System.Drawing.Size(287, 29);
      this.flowLayoutPanel1.TabIndex = 4;
      // 
      // cbClosed
      // 
      this.cbClosed.AutoSize = true;
      this.cbClosed.Location = new System.Drawing.Point(3, 3);
      this.cbClosed.Name = "cbClosed";
      this.cbClosed.Size = new System.Drawing.Size(58, 17);
      this.cbClosed.TabIndex = 0;
      this.cbClosed.Text = "Closed";
      this.cbClosed.UseVisualStyleBackColor = true;
      this.cbClosed.Visible = false;
      // 
      // cbShipping
      // 
      this.cbShipping.AutoSize = true;
      this.cbShipping.Location = new System.Drawing.Point(67, 3);
      this.cbShipping.Name = "cbShipping";
      this.cbShipping.Size = new System.Drawing.Size(67, 17);
      this.cbShipping.TabIndex = 1;
      this.cbShipping.Text = "Shipping";
      this.cbShipping.UseVisualStyleBackColor = true;
      this.cbShipping.Visible = false;
      // 
      // cbDiscontinued
      // 
      this.cbDiscontinued.AutoSize = true;
      this.cbDiscontinued.Location = new System.Drawing.Point(140, 3);
      this.cbDiscontinued.Name = "cbDiscontinued";
      this.cbDiscontinued.Size = new System.Drawing.Size(88, 17);
      this.cbDiscontinued.TabIndex = 2;
      this.cbDiscontinued.Text = "Discontinued";
      this.cbDiscontinued.UseVisualStyleBackColor = true;
      this.cbDiscontinued.Visible = false;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(223, 143);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 5;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(142, 143);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 6;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // CustomPropertyForm
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(314, 183);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.btnCancel);
      this.Controls.Add(this.flowLayoutPanel1);
      this.Controls.Add(this.textName);
      this.Controls.Add(this.textDescription);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.label1);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "CustomPropertyForm";
      this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
      this.Text = "Custom Property";
      this.Load += new System.EventHandler(this.CustomPropertyForm_Load);
      this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.CustomPropertyForm_FormClosing);
      this.flowLayoutPanel1.ResumeLayout(false);
      this.flowLayoutPanel1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textDescription;
    private System.Windows.Forms.TextBox textName;
    private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
    private System.Windows.Forms.CheckBox cbClosed;
    private System.Windows.Forms.CheckBox cbShipping;
    private System.Windows.Forms.CheckBox cbDiscontinued;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOk;
  }
}