﻿namespace TeamSupport.Utility
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
      this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
      this.tabPage1 = new System.Windows.Forms.TabPage();
      this.label1 = new System.Windows.Forms.Label();
      this.btnAttachmentsClean = new System.Windows.Forms.Button();
      this.textBox1 = new System.Windows.Forms.TextBox();
      this.tabControlMain = new System.Windows.Forms.TabControl();
      this.button1 = new System.Windows.Forms.Button();
      this.richTextBox1 = new System.Windows.Forms.RichTextBox();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.fToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStrip1 = new System.Windows.Forms.ToolStrip();
      this.tabPage1.SuspendLayout();
      this.tabControlMain.SuspendLayout();
      this.menuStrip1.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabPage1
      // 
      this.tabPage1.Controls.Add(this.richTextBox1);
      this.tabPage1.Controls.Add(this.button1);
      this.tabPage1.Controls.Add(this.textBox1);
      this.tabPage1.Controls.Add(this.btnAttachmentsClean);
      this.tabPage1.Controls.Add(this.label1);
      this.tabPage1.Location = new System.Drawing.Point(4, 22);
      this.tabPage1.Name = "tabPage1";
      this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
      this.tabPage1.Size = new System.Drawing.Size(680, 375);
      this.tabPage1.TabIndex = 0;
      this.tabPage1.Text = "Attachments";
      this.tabPage1.UseVisualStyleBackColor = true;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Location = new System.Drawing.Point(8, 12);
      this.label1.Name = "label1";
      this.label1.Size = new System.Drawing.Size(93, 13);
      this.label1.TabIndex = 2;
      this.label1.Text = "Attachment Folder";
      // 
      // btnAttachmentsClean
      // 
      this.btnAttachmentsClean.Location = new System.Drawing.Point(409, 26);
      this.btnAttachmentsClean.Name = "btnAttachmentsClean";
      this.btnAttachmentsClean.Size = new System.Drawing.Size(111, 23);
      this.btnAttachmentsClean.TabIndex = 0;
      this.btnAttachmentsClean.Text = "Clean Attachments";
      this.btnAttachmentsClean.UseVisualStyleBackColor = true;
      this.btnAttachmentsClean.Click += new System.EventHandler(this.btnAttachmentsClean_Click);
      // 
      // textBox1
      // 
      this.textBox1.Location = new System.Drawing.Point(9, 28);
      this.textBox1.Name = "textBox1";
      this.textBox1.Size = new System.Drawing.Size(306, 20);
      this.textBox1.TabIndex = 1;
      // 
      // tabControlMain
      // 
      this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.tabControlMain.Controls.Add(this.tabPage1);
      this.tabControlMain.Location = new System.Drawing.Point(0, 52);
      this.tabControlMain.Name = "tabControlMain";
      this.tabControlMain.SelectedIndex = 0;
      this.tabControlMain.Size = new System.Drawing.Size(688, 401);
      this.tabControlMain.TabIndex = 3;
      // 
      // button1
      // 
      this.button1.Location = new System.Drawing.Point(321, 26);
      this.button1.Name = "button1";
      this.button1.Size = new System.Drawing.Size(82, 23);
      this.button1.TabIndex = 3;
      this.button1.Text = "Set Folder";
      this.button1.UseVisualStyleBackColor = true;
      // 
      // richTextBox1
      // 
      this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.richTextBox1.Location = new System.Drawing.Point(6, 55);
      this.richTextBox1.Name = "richTextBox1";
      this.richTextBox1.Size = new System.Drawing.Size(666, 312);
      this.richTextBox1.TabIndex = 4;
      this.richTextBox1.Text = "";
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fToolStripMenuItem});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(688, 24);
      this.menuStrip1.TabIndex = 4;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // fToolStripMenuItem
      // 
      this.fToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exitToolStripMenuItem});
      this.fToolStripMenuItem.Name = "fToolStripMenuItem";
      this.fToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
      this.fToolStripMenuItem.Text = "File";
      // 
      // exitToolStripMenuItem
      // 
      this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
      this.exitToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
      this.exitToolStripMenuItem.Text = "Exit";
      // 
      // toolStrip1
      // 
      this.toolStrip1.Location = new System.Drawing.Point(0, 24);
      this.toolStrip1.Name = "toolStrip1";
      this.toolStrip1.Size = new System.Drawing.Size(688, 25);
      this.toolStrip1.TabIndex = 5;
      this.toolStrip1.Text = "toolStrip1";
      // 
      // MainForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(688, 454);
      this.Controls.Add(this.toolStrip1);
      this.Controls.Add(this.tabControlMain);
      this.Controls.Add(this.menuStrip1);
      this.Name = "MainForm";
      this.Text = "TeamSupport Utility";
      this.tabPage1.ResumeLayout(false);
      this.tabPage1.PerformLayout();
      this.tabControlMain.ResumeLayout(false);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
    private System.Windows.Forms.TabPage tabPage1;
    private System.Windows.Forms.RichTextBox richTextBox1;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.TextBox textBox1;
    private System.Windows.Forms.Button btnAttachmentsClean;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TabControl tabControlMain;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem fToolStripMenuItem;
    private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
    private System.Windows.Forms.ToolStrip toolStrip1;

  }
}

