namespace DataRecovery
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
      this.btnImportAll = new System.Windows.Forms.Button();
      this.btnImportOrgToReview = new System.Windows.Forms.Button();
      this.cmbOrg = new System.Windows.Forms.ComboBox();
      this.btnRollbackOrgFromReview = new System.Windows.Forms.Button();
      this.btnImportOrgToProduction = new System.Windows.Forms.Button();
      this.cbCompanies = new System.Windows.Forms.CheckBox();
      this.cbProducts = new System.Windows.Forms.CheckBox();
      this.cbOldActions = new System.Windows.Forms.CheckBox();
      this.cbTickets = new System.Windows.Forms.CheckBox();
      this.btnRollBackOrgFromProduction = new System.Windows.Forms.Button();
      this.btnRollBackAll = new System.Windows.Forms.Button();
      this.SuspendLayout();
      // 
      // btnImportAll
      // 
      this.btnImportAll.Enabled = false;
      this.btnImportAll.Location = new System.Drawing.Point(12, 12);
      this.btnImportAll.Name = "btnImportAll";
      this.btnImportAll.Size = new System.Drawing.Size(147, 23);
      this.btnImportAll.TabIndex = 0;
      this.btnImportAll.Text = "Import All Orgs to Review";
      this.btnImportAll.UseVisualStyleBackColor = true;
      this.btnImportAll.Click += new System.EventHandler(this.button1_Click);
      // 
      // btnImportOrgToReview
      // 
      this.btnImportOrgToReview.Location = new System.Drawing.Point(12, 97);
      this.btnImportOrgToReview.Name = "btnImportOrgToReview";
      this.btnImportOrgToReview.Size = new System.Drawing.Size(136, 23);
      this.btnImportOrgToReview.TabIndex = 1;
      this.btnImportOrgToReview.Text = "Import Org To Review";
      this.btnImportOrgToReview.UseVisualStyleBackColor = true;
      this.btnImportOrgToReview.Click += new System.EventHandler(this.btnImportOrgToReview_Click);
      // 
      // cmbOrg
      // 
      this.cmbOrg.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
      this.cmbOrg.FormattingEnabled = true;
      this.cmbOrg.Location = new System.Drawing.Point(12, 70);
      this.cmbOrg.Name = "cmbOrg";
      this.cmbOrg.Size = new System.Drawing.Size(260, 21);
      this.cmbOrg.TabIndex = 2;
      // 
      // btnRollbackOrgFromReview
      // 
      this.btnRollbackOrgFromReview.Location = new System.Drawing.Point(12, 126);
      this.btnRollbackOrgFromReview.Name = "btnRollbackOrgFromReview";
      this.btnRollbackOrgFromReview.Size = new System.Drawing.Size(136, 23);
      this.btnRollbackOrgFromReview.TabIndex = 3;
      this.btnRollbackOrgFromReview.Text = "Roll back Org Review";
      this.btnRollbackOrgFromReview.UseVisualStyleBackColor = true;
      this.btnRollbackOrgFromReview.Click += new System.EventHandler(this.btnRollbackOrgFromReview_Click);
      // 
      // btnImportOrgToProduction
      // 
      this.btnImportOrgToProduction.BackColor = System.Drawing.SystemColors.MenuHighlight;
      this.btnImportOrgToProduction.Location = new System.Drawing.Point(12, 198);
      this.btnImportOrgToProduction.Name = "btnImportOrgToProduction";
      this.btnImportOrgToProduction.Size = new System.Drawing.Size(245, 23);
      this.btnImportOrgToProduction.TabIndex = 4;
      this.btnImportOrgToProduction.Text = "Import Org to PRODUCTION";
      this.btnImportOrgToProduction.UseVisualStyleBackColor = false;
      this.btnImportOrgToProduction.Click += new System.EventHandler(this.btnImportOrgToProduction_Click);
      // 
      // cbCompanies
      // 
      this.cbCompanies.AutoSize = true;
      this.cbCompanies.Location = new System.Drawing.Point(179, 97);
      this.cbCompanies.Name = "cbCompanies";
      this.cbCompanies.Size = new System.Drawing.Size(78, 17);
      this.cbCompanies.TabIndex = 5;
      this.cbCompanies.Text = "Companies";
      this.cbCompanies.UseVisualStyleBackColor = true;
      // 
      // cbProducts
      // 
      this.cbProducts.AutoSize = true;
      this.cbProducts.Location = new System.Drawing.Point(179, 120);
      this.cbProducts.Name = "cbProducts";
      this.cbProducts.Size = new System.Drawing.Size(68, 17);
      this.cbProducts.TabIndex = 6;
      this.cbProducts.Text = "Products";
      this.cbProducts.UseVisualStyleBackColor = true;
      // 
      // cbOldActions
      // 
      this.cbOldActions.AutoSize = true;
      this.cbOldActions.Location = new System.Drawing.Point(179, 143);
      this.cbOldActions.Name = "cbOldActions";
      this.cbOldActions.Size = new System.Drawing.Size(80, 17);
      this.cbOldActions.TabIndex = 7;
      this.cbOldActions.Text = "Old Actions";
      this.cbOldActions.UseVisualStyleBackColor = true;
      // 
      // cbTickets
      // 
      this.cbTickets.AutoSize = true;
      this.cbTickets.Location = new System.Drawing.Point(179, 166);
      this.cbTickets.Name = "cbTickets";
      this.cbTickets.Size = new System.Drawing.Size(61, 17);
      this.cbTickets.TabIndex = 8;
      this.cbTickets.Text = "Tickets";
      this.cbTickets.UseVisualStyleBackColor = true;
      // 
      // btnRollBackOrgFromProduction
      // 
      this.btnRollBackOrgFromProduction.BackColor = System.Drawing.SystemColors.MenuHighlight;
      this.btnRollBackOrgFromProduction.Location = new System.Drawing.Point(12, 227);
      this.btnRollBackOrgFromProduction.Name = "btnRollBackOrgFromProduction";
      this.btnRollBackOrgFromProduction.Size = new System.Drawing.Size(245, 23);
      this.btnRollBackOrgFromProduction.TabIndex = 9;
      this.btnRollBackOrgFromProduction.Text = "Rollback Import from PRODUCTION";
      this.btnRollBackOrgFromProduction.UseVisualStyleBackColor = false;
      this.btnRollBackOrgFromProduction.Click += new System.EventHandler(this.btnRollBackOrgFromProduction_Click);
      // 
      // btnRollBackAll
      // 
      this.btnRollBackAll.Enabled = false;
      this.btnRollBackAll.Location = new System.Drawing.Point(12, 41);
      this.btnRollBackAll.Name = "btnRollBackAll";
      this.btnRollBackAll.Size = new System.Drawing.Size(147, 23);
      this.btnRollBackAll.TabIndex = 10;
      this.btnRollBackAll.Text = "Rollback All Orgs From Review";
      this.btnRollBackAll.UseVisualStyleBackColor = true;
      this.btnRollBackAll.Click += new System.EventHandler(this.btnRollBackAll_Click);
      // 
      // Form1
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(284, 262);
      this.Controls.Add(this.btnRollBackAll);
      this.Controls.Add(this.btnRollBackOrgFromProduction);
      this.Controls.Add(this.cbTickets);
      this.Controls.Add(this.cbOldActions);
      this.Controls.Add(this.cbProducts);
      this.Controls.Add(this.cbCompanies);
      this.Controls.Add(this.btnImportOrgToProduction);
      this.Controls.Add(this.btnRollbackOrgFromReview);
      this.Controls.Add(this.cmbOrg);
      this.Controls.Add(this.btnImportOrgToReview);
      this.Controls.Add(this.btnImportAll);
      this.Name = "Form1";
      this.Text = "Form1";
      this.ResumeLayout(false);
      this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnImportAll;
    private System.Windows.Forms.Button btnImportOrgToReview;
    private System.Windows.Forms.ComboBox cmbOrg;
    private System.Windows.Forms.Button btnRollbackOrgFromReview;
    private System.Windows.Forms.Button btnImportOrgToProduction;
    private System.Windows.Forms.CheckBox cbCompanies;
    private System.Windows.Forms.CheckBox cbProducts;
    private System.Windows.Forms.CheckBox cbOldActions;
    private System.Windows.Forms.CheckBox cbTickets;
    private System.Windows.Forms.Button btnRollBackOrgFromProduction;
    private System.Windows.Forms.Button btnRollBackAll;
	}
}

