﻿namespace TeamSupport.ServiceTestApplication
{
  partial class Form1
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
			this.components = new System.ComponentModel.Container();
			this.timerEmails = new System.Windows.Forms.Timer(this.components);
			this.btnEmailProcessor = new System.Windows.Forms.Button();
			this.btnSlaProcessor = new System.Windows.Forms.Button();
			this.btnEmailSender = new System.Windows.Forms.Button();
			this.btnCrmPool = new System.Windows.Forms.Button();
			this.btnIndexer = new System.Windows.Forms.Button();
			this.btnReminders = new System.Windows.Forms.Button();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.btnTestEmail = new System.Windows.Forms.Button();
			this.btnFullContacts = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.btnReportSender = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnEmailProcessor
			// 
			this.btnEmailProcessor.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEmailProcessor.ForeColor = System.Drawing.Color.Green;
			this.btnEmailProcessor.Location = new System.Drawing.Point(12, 12);
			this.btnEmailProcessor.Name = "btnEmailProcessor";
			this.btnEmailProcessor.Size = new System.Drawing.Size(221, 31);
			this.btnEmailProcessor.TabIndex = 21;
			this.btnEmailProcessor.Text = "Start Email Processor";
			this.btnEmailProcessor.UseVisualStyleBackColor = true;
			this.btnEmailProcessor.Click += new System.EventHandler(this.btnEmailProcessor_Click);
			// 
			// btnSlaProcessor
			// 
			this.btnSlaProcessor.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSlaProcessor.ForeColor = System.Drawing.Color.Green;
			this.btnSlaProcessor.Location = new System.Drawing.Point(12, 159);
			this.btnSlaProcessor.Name = "btnSlaProcessor";
			this.btnSlaProcessor.Size = new System.Drawing.Size(221, 29);
			this.btnSlaProcessor.TabIndex = 22;
			this.btnSlaProcessor.Text = "Start SLA Processor";
			this.btnSlaProcessor.UseVisualStyleBackColor = true;
			this.btnSlaProcessor.Click += new System.EventHandler(this.btnSlaProcessor_Click);
			// 
			// btnEmailSender
			// 
			this.btnEmailSender.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnEmailSender.ForeColor = System.Drawing.Color.Green;
			this.btnEmailSender.Location = new System.Drawing.Point(12, 49);
			this.btnEmailSender.Name = "btnEmailSender";
			this.btnEmailSender.Size = new System.Drawing.Size(221, 31);
			this.btnEmailSender.TabIndex = 23;
			this.btnEmailSender.Text = "Start Email Sender";
			this.btnEmailSender.UseVisualStyleBackColor = true;
			this.btnEmailSender.Click += new System.EventHandler(this.btnEmailSender_Click);
			// 
			// btnCrmPool
			// 
			this.btnCrmPool.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnCrmPool.ForeColor = System.Drawing.Color.Green;
			this.btnCrmPool.Location = new System.Drawing.Point(12, 196);
			this.btnCrmPool.Name = "btnCrmPool";
			this.btnCrmPool.Size = new System.Drawing.Size(221, 29);
			this.btnCrmPool.TabIndex = 24;
			this.btnCrmPool.Text = "Start CRM Pool";
			this.btnCrmPool.UseVisualStyleBackColor = true;
			this.btnCrmPool.Click += new System.EventHandler(this.btnCrmPool_Click);
			// 
			// btnIndexer
			// 
			this.btnIndexer.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnIndexer.ForeColor = System.Drawing.Color.Green;
			this.btnIndexer.Location = new System.Drawing.Point(12, 124);
			this.btnIndexer.Name = "btnIndexer";
			this.btnIndexer.Size = new System.Drawing.Size(221, 29);
			this.btnIndexer.TabIndex = 25;
			this.btnIndexer.Text = "Start Indexer";
			this.btnIndexer.UseVisualStyleBackColor = true;
			this.btnIndexer.Click += new System.EventHandler(this.btnIndexer_Click);
			// 
			// btnReminders
			// 
			this.btnReminders.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnReminders.ForeColor = System.Drawing.Color.Green;
			this.btnReminders.Location = new System.Drawing.Point(12, 86);
			this.btnReminders.Name = "btnReminders";
			this.btnReminders.Size = new System.Drawing.Size(221, 31);
			this.btnReminders.TabIndex = 27;
			this.btnReminders.Text = "Start Reminder Processor";
			this.btnReminders.UseVisualStyleBackColor = true;
			this.btnReminders.Click += new System.EventHandler(this.btnReminders_Click);
			// 
			// button1
			// 
			this.button1.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button1.ForeColor = System.Drawing.Color.Green;
			this.button1.Location = new System.Drawing.Point(12, 231);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(221, 29);
			this.button1.TabIndex = 28;
			this.button1.Text = "Start Webhooks";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.button2.ForeColor = System.Drawing.Color.Green;
			this.button2.Location = new System.Drawing.Point(12, 266);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(221, 29);
			this.button2.TabIndex = 29;
			this.button2.Text = "Start Imports";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// btnTestEmail
			// 
			this.btnTestEmail.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnTestEmail.ForeColor = System.Drawing.Color.Green;
			this.btnTestEmail.Location = new System.Drawing.Point(12, 405);
			this.btnTestEmail.Name = "btnTestEmail";
			this.btnTestEmail.Size = new System.Drawing.Size(221, 29);
			this.btnTestEmail.TabIndex = 30;
			this.btnTestEmail.Text = "Send Test Email";
			this.btnTestEmail.UseVisualStyleBackColor = true;
			this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
			// 
			// btnFullContacts
			// 
			this.btnFullContacts.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnFullContacts.ForeColor = System.Drawing.Color.Green;
			this.btnFullContacts.Location = new System.Drawing.Point(12, 301);
			this.btnFullContacts.Name = "btnFullContacts";
			this.btnFullContacts.Size = new System.Drawing.Size(221, 29);
			this.btnFullContacts.TabIndex = 31;
			this.btnFullContacts.Text = "Start Full Contacts";
			this.btnFullContacts.UseVisualStyleBackColor = true;
			this.btnFullContacts.Click += new System.EventHandler(this.btnFullContacts_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(13, 376);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(220, 23);
			this.button3.TabIndex = 32;
			this.button3.Text = "Temp Test";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// btnReportSender
			// 
			this.btnReportSender.Font = new System.Drawing.Font("Trebuchet MS", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnReportSender.ForeColor = System.Drawing.Color.Green;
			this.btnReportSender.Location = new System.Drawing.Point(12, 336);
			this.btnReportSender.Name = "btnReportSender";
			this.btnReportSender.Size = new System.Drawing.Size(221, 29);
			this.btnReportSender.TabIndex = 33;
			this.btnReportSender.Text = "Start Report Sender";
			this.btnReportSender.UseVisualStyleBackColor = true;
			this.btnReportSender.Click += new System.EventHandler(this.btnReportSender_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(248, 446);
			this.Controls.Add(this.btnReportSender);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.btnFullContacts);
			this.Controls.Add(this.btnTestEmail);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.btnReminders);
			this.Controls.Add(this.btnIndexer);
			this.Controls.Add(this.btnCrmPool);
			this.Controls.Add(this.btnEmailSender);
			this.Controls.Add(this.btnSlaProcessor);
			this.Controls.Add(this.btnEmailProcessor);
			this.Name = "Form1";
			this.Text = "Service Test";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
			this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Timer timerEmails;
    private System.Windows.Forms.Button btnEmailProcessor;
    private System.Windows.Forms.Button btnSlaProcessor;
    private System.Windows.Forms.Button btnEmailSender;
    private System.Windows.Forms.Button btnCrmPool;
    private System.Windows.Forms.Button btnIndexer;
    private System.Windows.Forms.Button btnReminders;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button button2;
    private System.Windows.Forms.Button btnTestEmail;
    private System.Windows.Forms.Button btnFullContacts;
        private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button btnReportSender;
	}
}

