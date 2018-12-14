namespace TeamSupport.DataManager.Forms
{
  partial class MySqlConnectionDialog
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
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOk = new System.Windows.Forms.Button();
      this.label1 = new System.Windows.Forms.Label();
      this.label2 = new System.Windows.Forms.Label();
      this.label3 = new System.Windows.Forms.Label();
      this.label4 = new System.Windows.Forms.Label();
      this.btnTest = new System.Windows.Forms.Button();
      this.textPassword = new System.Windows.Forms.TextBox();
      this.textUser = new System.Windows.Forms.TextBox();
      this.textServer = new System.Windows.Forms.TextBox();
      this.textDbName = new System.Windows.Forms.TextBox();
      this.SuspendLayout();
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(233, 130);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 0;
      this.btnCancel.Text = "Cancel";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnOk
      // 
      this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOk.Location = new System.Drawing.Point(152, 130);
      this.btnOk.Name = "btnOk";
      this.btnOk.Size = new System.Drawing.Size(75, 23);
      this.btnOk.TabIndex = 1;
      this.btnOk.Text = "OK";
      this.btnOk.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(34, 15);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(41, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Server:";
      // 
      // label2
      // 
      this.label2.AutoSize = true;
      this.label2.Location = new System.Drawing.Point(12, 41);
      this.label2.Name = "label2";
      this.label2.Size = new System.Drawing.Size(63, 13);
      this.label2.TabIndex = 5;
      this.label2.Text = "User Name:";
      // 
      // label3
      // 
      this.label3.AutoSize = true;
      this.label3.Location = new System.Drawing.Point(19, 67);
      this.label3.Name = "label3";
      this.label3.Size = new System.Drawing.Size(56, 13);
      this.label3.TabIndex = 6;
      this.label3.Text = "Password:";
      // 
      // label4
      // 
      this.label4.AutoSize = true;
      this.label4.Location = new System.Drawing.Point(19, 93);
      this.label4.Name = "label4";
      this.label4.Size = new System.Drawing.Size(56, 13);
      this.label4.TabIndex = 7;
      this.label4.Text = "Database:";
      // 
      // btnTest
      // 
      this.btnTest.Location = new System.Drawing.Point(12, 130);
      this.btnTest.Name = "btnTest";
      this.btnTest.Size = new System.Drawing.Size(75, 23);
      this.btnTest.TabIndex = 10;
      this.btnTest.Text = "Test";
      this.btnTest.UseVisualStyleBackColor = true;
      this.btnTest.Click += new System.EventHandler(this.btnTest_Click);
      // 
      // textPassword
      // 
      this.textPassword.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TeamSupport.DataManager.Properties.Settings.Default, "MySqlPassword", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textPassword.Location = new System.Drawing.Point(101, 64);
      this.textPassword.Name = "textPassword";
      this.textPassword.Size = new System.Drawing.Size(207, 20);
      this.textPassword.TabIndex = 8;
      this.textPassword.Text = global::TeamSupport.DataManager.Properties.Settings.Default.MySqlPassword;
      // 
      // textUser
      // 
      this.textUser.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TeamSupport.DataManager.Properties.Settings.Default, "MySqlUser", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textUser.Location = new System.Drawing.Point(101, 38);
      this.textUser.Name = "textUser";
      this.textUser.Size = new System.Drawing.Size(207, 20);
      this.textUser.TabIndex = 4;
      this.textUser.Text = global::TeamSupport.DataManager.Properties.Settings.Default.MySqlUser;
      // 
      // textServer
      // 
      this.textServer.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TeamSupport.DataManager.Properties.Settings.Default, "MySqlServer", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textServer.Location = new System.Drawing.Point(101, 12);
      this.textServer.Name = "textServer";
      this.textServer.Size = new System.Drawing.Size(207, 20);
      this.textServer.TabIndex = 3;
      this.textServer.Text = global::TeamSupport.DataManager.Properties.Settings.Default.MySqlServer;
      // 
      // textDbName
      // 
      this.textDbName.DataBindings.Add(new System.Windows.Forms.Binding("Text", global::TeamSupport.DataManager.Properties.Settings.Default, "MySqlDB", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));
      this.textDbName.Location = new System.Drawing.Point(101, 90);
      this.textDbName.Name = "textDbName";
      this.textDbName.Size = new System.Drawing.Size(207, 20);
      this.textDbName.TabIndex = 11;
      this.textDbName.Text = global::TeamSupport.DataManager.Properties.Settings.Default.MySqlDB;
      // 
      // MySqlConnectionDialog
      // 
      this.AcceptButton = this.btnOk;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.CancelButton = this.btnCancel;
      this.ClientSize = new System.Drawing.Size(330, 173);
      this.Controls.Add(this.textDbName);
      this.Controls.Add(this.btnTest);
      this.Controls.Add(this.textPassword);
      this.Controls.Add(this.label4);
      this.Controls.Add(this.label3);
      this.Controls.Add(this.label2);
      this.Controls.Add(this.textUser);
      this.Controls.Add(this.textServer);
      this.Controls.Add(this.label1);
      this.Controls.Add(this.btnOk);
      this.Controls.Add(this.btnCancel);
      this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
      this.Name = "MySqlConnectionDialog";
      this.ShowInTaskbar = false;
      this.Text = "MySql Connection";
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOk;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textServer;
    private System.Windows.Forms.TextBox textUser;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.TextBox textPassword;
    private System.Windows.Forms.Button btnTest;
    private System.Windows.Forms.TextBox textDbName;
  }
}