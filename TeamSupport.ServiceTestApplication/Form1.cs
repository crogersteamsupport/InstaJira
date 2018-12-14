using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TeamSupport.Data;
using TeamSupport.ServiceLibrary;
using TeamSupport.CrmIntegration;
using Microsoft.Win32;
using System.Net.Mail;
using System.IO;
using Quiksoft.EasyMail.SMTP;

namespace TeamSupport.ServiceTestApplication
{
  public partial class Form1 : Form
  {
    ServiceThreadPool<EmailProcessor> _emailProcessor;
    ServiceThreadPool<EmailSender> _emailSender;
    SlaProcessor _slaProcessor;
    ServiceThreadPool<Indexer> _indexer;
    CrmPool _crmPool;
    ReminderProcessor _reminderProcessor;
    ImportProcessor _importProcessor;
    WebHooks _webhooks;
    CustomerInsightsProcessor _cip;
	ServiceThreadPool<ReportSender> _reportSender;
        ServiceThreadPool<TokTranscoder> _tokTranscoder;
    ServiceThreadPool<TaskProcessor> _taskProcessor;
	WebHooksPool _webHooksProcessor;
    
    public Form1()
    {
      InitializeComponent();
      System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
      /*
      Settings settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailSender");
      settings.WriteBool("Debug", true);
      settings = new Settings(ServiceThread.GetLoginUser("Service Test App"), "EmailProcessor");
      settings.WriteBool("Debug", true);
      */
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e)
    {
      Properties.Settings.Default.Save();
      StopAll();
    }

    private void StopAll()
    {
      ServiceThread.ServiceStopped = true;
      if (_emailProcessor != null) _emailProcessor.Stop();
      if (_emailSender != null) _emailSender.Stop();
      if (_slaProcessor != null) _slaProcessor.Stop();
      if (_indexer != null) _indexer.Stop();
      if (_crmPool != null) _crmPool.Stop();
      if (_reminderProcessor != null) _reminderProcessor.Stop();
      if (_importProcessor != null) _importProcessor.Stop();
      if (_webhooks != null) _webhooks.Stop();
      if (_cip != null) _cip.Stop();
	  if (_reportSender != null) _reportSender.Stop();
            if (_tokTranscoder != null) _tokTranscoder.Stop();
      if (_taskProcessor != null) _taskProcessor.Stop();
		if (_webHooksProcessor != null) _webHooksProcessor.Stop();
        }

    private void StartProcess(ServiceThread thread, Button button)
    {
      button.Text = button.Text.Replace("Start ", "Stop ");
      button.ForeColor = Color.Red;
      try
      {
        button.Enabled = false;
        thread.Start();
      }
      finally
      {
        button.Enabled = true;
      }
    
    }

    private void StopProcess(ServiceThread thread, Button button)
    {
      try
      {
        button.Enabled = false;
        thread.Stop();
        thread = null;
      }
      finally
      {
        button.Enabled = true;
        button.Text = button.Text.Replace("Stop ", "Start ");
        button.ForeColor = Color.Green;
      }
    }

    private void btnEmailProcessor_Click(object sender, EventArgs e)
    {
      if (_emailProcessor == null || _emailProcessor.IsStopped) StartProcess(_emailProcessor = new ServiceThreadPool<EmailProcessor>("EmailProcessor"), sender as Button); else StopProcess(_emailProcessor, sender as Button);
    }

    private void btnEmailSender_Click(object sender, EventArgs e)
    {
      if (_emailSender == null || _emailSender.IsStopped) StartProcess(_emailSender = new ServiceThreadPool<EmailSender>("EmailSender"), sender as Button); else StopProcess(_emailSender, sender as Button);
    }

    private void btnSlaProcessor_Click(object sender, EventArgs e)
    {
      if (_slaProcessor == null || _slaProcessor.IsStopped) StartProcess(_slaProcessor = new SlaProcessor(), sender as Button); else StopProcess(_slaProcessor, sender as Button);
    }

    private void btnIndexer_Click(object sender, EventArgs e)
    {
      if (_indexer == null || _indexer.IsStopped) StartProcess(_indexer = new ServiceThreadPool<Indexer>("Indexer"), sender as Button); else StopProcess(_indexer, sender as Button);
    }

    private void btnCrmPool_Click(object sender, EventArgs e)
    {
      if (_crmPool == null || _crmPool.IsStopped) StartProcess(_crmPool = new CrmPool(), sender as Button); else StopProcess(_crmPool, sender as Button);
    }

    private void btnReminders_Click(object sender, EventArgs e)
    {
      if (_reminderProcessor == null || _reminderProcessor.IsStopped) StartProcess(_reminderProcessor = new ReminderProcessor(), sender as Button); else StopProcess(_reminderProcessor, sender as Button);

    }

    private void button1_Click(object sender, EventArgs e)
    {
      if (_webhooks == null || _webhooks.IsStopped) StartProcess(_webhooks = new WebHooks(), sender as Button); else StopProcess(_webhooks, sender as Button);
    }

    private void button2_Click(object sender, EventArgs e)
    {
      if (_importProcessor == null || _importProcessor.IsStopped) StartProcess(_importProcessor = new ImportProcessor(), sender as Button); else StopProcess(_importProcessor, sender as Button);
    }

    private void btnTestEmail_Click(object sender, EventArgs e)
    {
      Quiksoft.EasyMail.SMTP.License.Key = "Muroc Systems, Inc. (Single Developer)/9983782F406978487783FBAA248A#E86A";
      Quiksoft.EasyMail.SSL.License.Key = "Muroc Systems, Inc. (Single Developer)/9984652F406991896501FC33B3#02AE4B";

      const string smtpServerHostName = "smtp.socketlabs.com";
      const string smtpUserName = "MurocSystems";
      const string smtpPassword = "k3C5Wtb8ZYs7";

      var ssl = new Quiksoft.EasyMail.SSL.SSL();
      var smtp = new Quiksoft.EasyMail.SMTP.SMTP();

 
      EmailMessage msg = new EmailMessage();
      msg.Recipients.Add("kevin4885@gmail.com", "Kevin Tst Jones", RecipientType.To);
      msg.Subject = "KEVIN'S TEST EMAIL";
      msg.From.Email = "blah@blah.com";
      msg.From.Name = "Mr. blah";
      msg.BodyParts.Add(new Quiksoft.EasyMail.SMTP.BodyPart("三菱電機 増田です。", BodyPartFormat.HTML));
      msg.CharsetEncoding = System.Text.Encoding.UTF8;
      //msg.Attachments.Add("c:\\tesxt.png");
      msg.CustomHeaders.Add("X-xsMessageId", "");
      msg.CustomHeaders.Add("X-xsMailingId", "");
      //message.Headers.Add("X-xsMessageId", email.OrganizationID.ToString());
      //message.Headers.Add("X-xsMailingId", email.EmailID.ToString());

      //Set the SMTP server and secure port.
      var smtpServer = new SMTPServer
      {
          Name = smtpServerHostName,
          Port = 465, //Secure port
          Account = smtpUserName,
          Password = smtpPassword,
          AuthMode = SMTPAuthMode.AuthLogin
      };

      smtp.SMTPServers.Add(smtpServer);

      try
      {
          smtp.Connect(ssl.GetInterface());
          //For performance loop here to send multiple message on the same connection.
          smtp.Send(msg);
          //Disconnect when done.
          smtp.Disconnect();
          MessageBox.Show("Message Sent");
      }
      catch (Exception ex)
      {
        MessageBox.Show("There was an error sending mail\n\n" + ex.Message);
      }
    }

    private void btnFullContacts_Click(object sender, EventArgs e)
    {
      if (_cip == null || _cip.IsStopped) StartProcess(_cip = new CustomerInsightsProcessor(), sender as Button); else StopProcess(_cip, sender as Button);

    }

        private void button3_Click(object sender, EventArgs e)
        {
            MessageBox.Show(LoginUser.GetConnectionString());
        }

		private void btnReportSender_Click(object sender, EventArgs e)
		{
			if (_reportSender == null || _reportSender.IsStopped) StartProcess(_reportSender = new ServiceThreadPool<ReportSender>("ReportSender"), sender as Button); else StopProcess(_reportSender, sender as Button);
		}

        private void button3_Click_1(object sender, EventArgs e)
        {
            if (_tokTranscoder == null || _tokTranscoder.IsStopped) StartProcess(_tokTranscoder = new ServiceThreadPool<TokTranscoder>("TokTranscoder"), sender as Button); else StopProcess(_tokTranscoder, sender as Button);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (_taskProcessor == null || _taskProcessor.IsStopped) StartProcess(_taskProcessor = new ServiceThreadPool<TaskProcessor>("TaskProcessor"), sender as Button); else StopProcess(_taskProcessor, sender as Button);
        }

		private void btnWebHooks_Click(object sender, EventArgs e)
		{
			if (_webHooksProcessor == null || _webHooksProcessor.IsStopped) StartProcess(_webHooksProcessor = new WebHooksPool(SystemUser.CRM), sender as Button); else StopProcess(_webHooksProcessor, sender as Button);
		}
    }
}
