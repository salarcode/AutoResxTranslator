namespace AutoResxTranslator
{
	partial class frmAbout
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmAbout));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.btnClose = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.lblVersion = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.lnkUpdate = new System.Windows.Forms.LinkLabel();
			this.lnk = new System.Windows.Forms.Label();
			this.lnkWebSite = new System.Windows.Forms.LinkLabel();
			this.label7 = new System.Windows.Forms.Label();
			this.lblLicense = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.lnkEmail = new System.Windows.Forms.LinkLabel();
			this.label4 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(12, 12);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(128, 128);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// btnClose
			// 
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnClose.Location = new System.Drawing.Point(351, 194);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(75, 23);
			this.btnClose.TabIndex = 1;
			this.btnClose.Text = "Close";
			this.btnClose.UseVisualStyleBackColor = true;
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.lblVersion);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Controls.Add(this.lnkUpdate);
			this.groupBox1.Controls.Add(this.lnk);
			this.groupBox1.Controls.Add(this.lnkWebSite);
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.lblLicense);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.lnkEmail);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(146, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(285, 176);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			// 
			// lblVersion
			// 
			this.lblVersion.AutoSize = true;
			this.lblVersion.Location = new System.Drawing.Point(75, 37);
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(48, 13);
			this.lblVersion.TabIndex = 10;
			this.lblVersion.Text = "(Version)";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(75, 57);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(88, 13);
			this.label8.TabIndex = 10;
			this.label8.Text = "Salar Khalilzadeh";
			// 
			// lnkUpdate
			// 
			this.lnkUpdate.AutoEllipsis = true;
			this.lnkUpdate.Location = new System.Drawing.Point(75, 103);
			this.lnkUpdate.Name = "lnkUpdate";
			this.lnkUpdate.Size = new System.Drawing.Size(205, 13);
			this.lnkUpdate.TabIndex = 1;
			this.lnkUpdate.TabStop = true;
			this.lnkUpdate.Text = "https://github.com/salarcode/AutoResxTranslator/";
			this.lnkUpdate.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkUpdate_LinkClicked);
			// 
			// lnk
			// 
			this.lnk.AutoSize = true;
			this.lnk.Location = new System.Drawing.Point(6, 103);
			this.lnk.Name = "lnk";
			this.lnk.Size = new System.Drawing.Size(49, 13);
			this.lnk.TabIndex = 8;
			this.lnk.Text = "Website:";
			// 
			// lnkWebSite
			// 
			this.lnkWebSite.AutoSize = true;
			this.lnkWebSite.Location = new System.Drawing.Point(75, 124);
			this.lnkWebSite.Name = "lnkWebSite";
			this.lnkWebSite.Size = new System.Drawing.Size(130, 13);
			this.lnkWebSite.TabIndex = 2;
			this.lnkWebSite.TabStop = true;
			this.lnkWebSite.Text = "http://blog.salarcode.com";
			this.lnkWebSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lnkWebSite_LinkClicked);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(6, 124);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(68, 13);
			this.label7.TabIndex = 8;
			this.label7.Text = "My blog ( fa):";
			// 
			// lblLicense
			// 
			this.lblLicense.AutoSize = true;
			this.lblLicense.Location = new System.Drawing.Point(75, 148);
			this.lblLicense.Name = "lblLicense";
			this.lblLicense.Size = new System.Drawing.Size(158, 13);
			this.lblLicense.TabIndex = 6;
			this.lblLicense.Text = "Free and Open-Source (MPLv2)";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(6, 148);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(47, 13);
			this.label5.TabIndex = 5;
			this.label5.Text = "License:";
			// 
			// lnkEmail
			// 
			this.lnkEmail.AutoSize = true;
			this.lnkEmail.Location = new System.Drawing.Point(75, 80);
			this.lnkEmail.Name = "lnkEmail";
			this.lnkEmail.Size = new System.Drawing.Size(99, 13);
			this.lnkEmail.TabIndex = 0;
			this.lnkEmail.TabStop = true;
			this.lnkEmail.Text = "salar2k@gmail.com";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(47, 13);
			this.label4.TabIndex = 3;
			this.label4.Text = "Contact:";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(6, 37);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(45, 13);
			this.label9.TabIndex = 1;
			this.label9.Text = "Version:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 57);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 13);
			this.label2.TabIndex = 1;
			this.label2.Text = "Author:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(152, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Auto Resource Translator";
			// 
			// frmAbout
			// 
			this.AcceptButton = this.btnClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnClose;
			this.ClientSize = new System.Drawing.Size(443, 223);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmAbout";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "About";
			this.Load += new System.EventHandler(this.frmAbout_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button btnClose;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lblVersion;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.LinkLabel lnkUpdate;
		private System.Windows.Forms.Label lnk;
		private System.Windows.Forms.LinkLabel lnkWebSite;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label lblLicense;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.LinkLabel lnkEmail;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
	}
}
