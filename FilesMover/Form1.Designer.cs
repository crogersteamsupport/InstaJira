namespace FilesMover
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
            this.run = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.stepAttachment = new System.Windows.Forms.Button();
            this.stepImport = new System.Windows.Forms.Button();
            this.stepScheduledReport = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // run
            // 
            this.run.Location = new System.Drawing.Point(490, 410);
            this.run.Name = "run";
            this.run.Size = new System.Drawing.Size(75, 23);
            this.run.TabIndex = 0;
            this.run.Text = "Run";
            this.run.UseVisualStyleBackColor = true;
            this.run.Click += new System.EventHandler(this.run_Click);
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.Color.Black;
            this.textBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.ForeColor = System.Drawing.Color.Lime;
            this.textBox1.Location = new System.Drawing.Point(13, 13);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox1.Size = new System.Drawing.Size(977, 391);
            this.textBox1.TabIndex = 1;
            this.textBox1.WordWrap = false;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged_1);
            // 
            // stepAttachment
            // 
            this.stepAttachment.Location = new System.Drawing.Point(43, 410);
            this.stepAttachment.Name = "stepAttachment";
            this.stepAttachment.Size = new System.Drawing.Size(143, 23);
            this.stepAttachment.TabIndex = 2;
            this.stepAttachment.Text = "Step Attachment";
            this.stepAttachment.UseVisualStyleBackColor = true;
            this.stepAttachment.Click += new System.EventHandler(this.stepAttachment_Click);
            // 
            // stepImport
            // 
            this.stepImport.Location = new System.Drawing.Point(192, 410);
            this.stepImport.Name = "stepImport";
            this.stepImport.Size = new System.Drawing.Size(143, 23);
            this.stepImport.TabIndex = 3;
            this.stepImport.Text = "Step Import";
            this.stepImport.UseVisualStyleBackColor = true;
            this.stepImport.Click += new System.EventHandler(this.stepImport_Click);
            // 
            // stepScheduledReport
            // 
            this.stepScheduledReport.Location = new System.Drawing.Point(341, 410);
            this.stepScheduledReport.Name = "stepScheduledReport";
            this.stepScheduledReport.Size = new System.Drawing.Size(143, 23);
            this.stepScheduledReport.TabIndex = 4;
            this.stepScheduledReport.Text = "Step Scheduled Report";
            this.stepScheduledReport.UseVisualStyleBackColor = true;
            this.stepScheduledReport.Click += new System.EventHandler(this.stepScheduledReport_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1002, 445);
            this.Controls.Add(this.stepScheduledReport);
            this.Controls.Add(this.stepImport);
            this.Controls.Add(this.stepAttachment);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.run);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button run;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button stepAttachment;
        private System.Windows.Forms.Button stepImport;
        private System.Windows.Forms.Button stepScheduledReport;
    }
}

