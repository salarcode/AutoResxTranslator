using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://autoresxtranslator.codeplex.com/
 * Mozilla Public License v2
 */
namespace AutoResxTranslator
{
	public partial class frmMain : Form
	{
		public frmMain()
		{
			InitializeComponent();
		}

		private readonly Dictionary<string, string> _languages =
			new Dictionary<string, string>
				{
					{"auto", "(Detect)"},
					{"af", "Afrikaans"},
					{"sq", "Albanian"},
					{"ar", "Arabic"},
					{"hy", "Armenian"},
					{"az", "Azerbaijani"},
					{"eu", "Basque"},
					{"be", "Belarusian"},
					{"bn", "Bengali"},
					{"bg", "Bulgarian"},
					{"ca", "Catalan"},
					{"zh-CN", "Chinese (Simplified)"},
					{"zh-TW", "Chinese (Traditional)"},
					{"hr", "Croatian"},
					{"cs", "Czech"},
					{"da", "Danish"},
					{"nl", "Dutch"},
					{"en", "English"},
					{"eo", "Esperanto"},
					{"et", "Estonian"},
					{"tl", "Filipino"},
					{"fi", "Finnish"},
					{"fr", "French"},
					{"gl", "Galician"},
					{"ka", "Georgian"},
					{"de", "German"},
					{"el", "Greek"},
					{"gu", "Gujarati"},
					{"ht", "Haitian Creole"},
					{"iw", "Hebrew"},
					{"hi", "Hindi"},
					{"hu", "Hungarian"},
					{"is", "Icelandic"},
					{"id", "Indonesian"},
					{"ga", "Irish"},
					{"it", "Italian"},
					{"ja", "Japanese"},
					{"kn", "Kannada"},
					{"km", "Khmer"},
					{"ko", "Korean"},
					{"lo", "Lao"},
					{"la", "Latin"},
					{"lv", "Latvian"},
					{"lt", "Lithuanian"},
					{"mk", "Macedonian"},
					{"ms", "Malay"},
					{"mt", "Maltese"},
					{"no", "Norwegian"},
					{"fa", "Persian"},
					{"pl", "Polish"},
					{"pt", "Portuguese"},
					{"ro", "Romanian"},
					{"ru", "Russian"},
					{"sr", "Serbian"},
					{"sk", "Slovak"},
					{"sl", "Slovenian"},
					{"es", "Spanish"},
					{"sw", "Swahili"},
					{"sv", "Swedish"},
					{"ta", "Tamil"},
					{"te", "Telugu"},
					{"th", "Thai"},
					{"tr", "Turkish"},
					{"uk", "Ukrainian"},
					{"ur", "Urdu"},
					{"vi", "Vietnamese"},
					{"cy", "Welsh"},
					{"yi", "Yiddish"}
				};


		void FillComboBoxes()
		{
			cmbSrc.DisplayMember = "Value";
			cmbSrc.ValueMember = "Key";

			cmbDesc.DisplayMember = "Value";
			cmbDesc.ValueMember = "Key";
			lstResxLanguages.Items.Clear();

			foreach (var k in _languages)
			{
				cmbSrc.Items.Add(k);
				if (k.Key == "auto")
					continue;
				cmbDesc.Items.Add(k);
				cmbSourceResxLng.Items.Add(k);
				lstResxLanguages.Items.Add(k.Key, k.Value, -1);
			}
			cmbSrc.SelectedIndex = 0;
			cmbDesc.Text = "English";
		}

		void SetResult(string result)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action<string>(SetResult), result);
				return;
			}
			else
			{
				txtDesc.Text = result;
			}
		}

		void IsBusy(bool isbusy)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new Action<bool>(IsBusy), isbusy);
				return;
			}
			else
			{
				tabMain.Enabled = !isbusy;
			}
		}


		string ReadLanguageName(string fileName)
		{
			try
			{
				fileName = Path.GetFileName(fileName);
				var match = Regex.Match(fileName, @".+\.(?<lng>.+)\.resx");
				var language = match.Groups["lng"].Value;
				var culture = new CultureInfo(language);
				return language;
			}
			catch (Exception)
			{
				return "en";
			}
		}

		string ReadLanguageFilename(string fileName)
		{
			try
			{
				fileName = Path.GetFileName(fileName);
				var match = Regex.Match(fileName, @"(?<file>.+)\.(?<lng>.+)\.resx");
				var file = match.Groups["file"].Value;
				if (string.IsNullOrWhiteSpace(file))
				{
					file = Path.GetFileNameWithoutExtension(fileName);
				}
				return file;
			}
			catch (Exception)
			{
				return "res";
			}
		}



		bool ValidateResxTranslate()
		{
			string errors = "";

			if (!File.Exists(txtSourceResx.Text))
				errors += "Please select source ResX file.\n";
			if (cmbSourceResxLng.SelectedIndex == -1)
				errors += "Please select source ResX file's language.\n";
			if (!Directory.Exists(txtOutputDir.Text))
				errors += "Please select output directory.\n";

			bool anychecked = false;
			foreach (ListViewItem item in lstResxLanguages.Items)
			{
				if (item.Checked)
				{
					anychecked = true;
					break;
				}
			}
			if (!anychecked)
			{
				errors += "At least one output language should be selected.\n";
			}

			if (errors.Length > 0)
			{
				MessageBox.Show(errors, "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return false;
			}
			return true;
		}


		void TranslateResxFiles()
		{
			var srcLng = ((KeyValuePair<string, string>)cmbSourceResxLng.SelectedItem).Key;
			var destLanguages = new List<string>();
			foreach (ListViewItem item in lstResxLanguages.Items)
			{
				if (!item.Checked)
					continue;
				if (item.Name == srcLng)
					continue;
				destLanguages.Add(item.Name);
			}
			if (destLanguages.Count == 0)
			{
				MessageBox.Show("The source and the destination languages can not be the same.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			IsBusy(true);
			new Action<string, string, List<string>, string, ResxProgressCallback>(TranslateResxFilesAsync).BeginInvoke(
				txtSourceResx.Text,
				srcLng,
				destLanguages,
				txtOutputDir.Text,
				ResxWorkingProgress,
				(x) => IsBusy(false),
				null);

		}

		private delegate void ResxProgressCallback(int max, int pos, string status);
		void TranslateResxFilesAsync(string sourceResx, string sourceLng, List<string> desLanguages, string destDir, ResxProgressCallback progress)
		{
			int max = 0;
			int pos = 0;
			int trycount = 0;
			string status = "";
			bool hasErrors = false;

			var sourceResxFilename = ReadLanguageFilename(sourceResx);
			var errorLogFilename = sourceResxFilename + ".errors.log";
			var errorLogFile = Path.Combine(destDir, errorLogFilename);

			foreach (var destLng in desLanguages)
			{
				var destFile = Path.Combine(destDir, sourceResxFilename + "." + destLng + ".resx");
				var doc = new XmlDocument();
				doc.Load(sourceResx);
				var dataList = ResxTranslator.ReadResxData(doc);
				max = dataList.Count;

				pos = 0;
				status = "Translating language: " + destLng;
				progress.BeginInvoke(max, pos, status, null, null);

				try
				{

					foreach (var node in dataList)
					{
						status = "Translating language: " + destLng;
						pos += 1;
						progress.BeginInvoke(max, pos, status, null, null);


						var valueNode = ResxTranslator.GetDataValueNode(node);
						if (valueNode == null) continue;

						var orgText = valueNode.InnerText;
						if (string.IsNullOrWhiteSpace(orgText)) continue;

						string translated = string.Empty;
						bool success = false;
						trycount = 0;
						do
						{
							try
							{
								success = GTranslateService.Translate(orgText, sourceLng, destLng, out translated);
							}
							catch (Exception)
							{
								success = false;
							}
							trycount++;

							if (!success)
							{
								var key = ResxTranslator.GetDataKeyName(node);
								status = "Translating language: " + destLng + " , key '" + key + "' failed to translate in try " + trycount;
								progress.BeginInvoke(max, pos, status, null, null);
							}

						} while (success == false && trycount <= 2);

						if (success)
						{
							valueNode.InnerText = translated;
						}
						else
						{
							hasErrors = true;
							var key = ResxTranslator.GetDataKeyName(node);
							try
							{
								string message = "\r\nKey '" + key + "' translation to language '" + destLng + "' failed.";
								File.AppendAllText(errorLogFile, message);
							}
							catch { }
						}
					}
				}
				finally
				{
					// now save that shit!
					doc.Save(destFile);
				}
			}

			if (hasErrors)
			{
				status = "Translation finished. Errors are logged in to '" + errorLogFilename + "'.";
			}
			else
			{
				status = "Translation finished.";

			}


			progress.BeginInvoke(max, pos, status, null, null);

		}

		void ResxWorkingProgress(int max, int pos, string status)
		{
			if (this.InvokeRequired)
			{
				this.BeginInvoke(new ResxProgressCallback(ResxWorkingProgress), max, pos, status);
				return;
			}
			else
			{
				barResxProgress.Minimum = 0;
				barResxProgress.Maximum = max;
				barResxProgress.Value = pos;
				lblResxTranslateStatus.Text = string.Format("Processing {0:00}/{1:00}, ", max, pos) + status;
			}
		}


		private void frmMain_Load(object sender, EventArgs e)
		{
			FillComboBoxes();
		}




		private void btnTranslate_Click(object sender, EventArgs e)
		{
			if (cmbDesc.SelectedIndex == -1 || cmbSrc.SelectedIndex == -1)
			{
				MessageBox.Show("Please select source and destination languages correctly.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (txtSrc.Text.Length == 0)
			{
				MessageBox.Show("The text body can not be empty.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var lngSrc = ((KeyValuePair<string, string>)cmbSrc.SelectedItem).Key;
			var lngDest = ((KeyValuePair<string, string>)cmbDesc.SelectedItem).Key;
			var text = txtSrc.Text;

			IsBusy(true);
			GTranslateService.TranslateAsync(
				text, lngSrc, lngDest,
				(success, result) =>
				{
					SetResult(result);
					IsBusy(false);
				});
		}

		private void btnSelectResxSource_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog();
			dlg.Filter = "ResourceX File|*.resx";
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtSourceResx.Text = dlg.FileName;
				if (txtOutputDir.Text.Length == 0)
				{
					txtOutputDir.Text = Path.GetDirectoryName(txtSourceResx.Text);
				}

				var lng = ReadLanguageName(txtSourceResx.Text);
				var key = _languages.FirstOrDefault(x => string.Compare(x.Key, lng, StringComparison.InvariantCultureIgnoreCase) == 0);
				if (key.Key != null)
					cmbSourceResxLng.SelectedItem = key;
				else
				{
					lng = "en";
					key = _languages.FirstOrDefault(x => string.Compare(x.Key, lng, StringComparison.InvariantCultureIgnoreCase) == 0);
					if (key.Key != null)
						cmbSourceResxLng.SelectedItem = key;
				}
			}
		}

		private void btnSelectOutputDir_Click(object sender, EventArgs e)
		{
			var dlg = new FolderBrowserDialog();
			dlg.ShowNewFolderButton = true;
			dlg.RootFolder = Environment.SpecialFolder.MyComputer;
			if (txtOutputDir.Text.Length > 0)
			{
				dlg.SelectedPath = txtOutputDir.Text;
			}
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtOutputDir.Text = dlg.SelectedPath;
			}
		}


		private void btnStartResxTranslate_Click(object sender, EventArgs e)
		{
			if (!ValidateResxTranslate())
				return;
			TranslateResxFiles();
		}

		private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using (var frm = new frmAbout())
			{
				frm.ShowDialog();
			}
		}

	}
}
