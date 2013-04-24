using System;
using System.Windows.Forms;

/* 
 * ResxAutoTranslator
 * by Salar Khalilzadeh
 * 
 * https://resxautotranslator.codeplex.com/
 * Mozilla Public License v2
 */
namespace ResxAutoTranslator
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new frmMain());
		}
	}
}
