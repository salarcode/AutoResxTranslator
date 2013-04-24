using System;
using System.Diagnostics;
using System.Reflection;
using System.Windows.Forms;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://AutoResxTranslator.codeplex.com/
 * Mozilla Public License v2
 */
namespace AutoResxTranslator
{
	public partial class frmAbout : Form
	{
		public frmAbout()
		{
			InitializeComponent();
		}

		private void btnClose_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void frmAbout_Load(object sender, EventArgs e)
		{
			lblVersion.Text = Assembly.GetExecutingAssembly().GetName().Version.ToString();
			this.Icon = Application.OpenForms[0].Icon;
		}

		private void lnkUpdate_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var start = new ProcessStartInfo(lnkUpdate.Text);
			try
			{
				start.UseShellExecute = true;
				Process.Start(start);
			}
			catch { }
		}

		private void lnkWebSite_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			var start = new ProcessStartInfo(lnkWebSite.Text);
			try
			{
				start.UseShellExecute = true;
				Process.Start(start);
			}
			catch { }
		}
	}
}
