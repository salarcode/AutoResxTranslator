using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using AutoResxTranslator.Definitions;

/* 
 * AutoResxTranslator
 * by Salar Khalilzadeh
 * 
 * https://github.com/salarcode/AutoResxTranslator/
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
				{"pt-PT", "Portuguese - Portugal"},
				{"pt-BR", "Portuguese - Brazil"},
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
		private bool _translateSettingsChanged;

		ServiceTypeEnum ServiceType
		{
			get
			{
				if (rbtnGoogleTranslateService.Checked)
					return ServiceTypeEnum.Google;
				if (rbtnDeepLTranslateService.Checked)
					return ServiceTypeEnum.DeepL;
				return ServiceTypeEnum.Microsoft;
			}
		}

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
				MessageBox.Show("The source and the destination languages can not be the same.", "", MessageBoxButtons.OK,
					MessageBoxIcon.Error);
				return;
			}
			bool translateFromKey = chkTranslateFromKey.Checked;

			var translationOptions = new TranslationOptions
			{
				ServiceType = ServiceType,
				MsSubscriptionKey = txtMsTranslationKey.Text,
				MsSubscriptionRegion = txtMsTranslationRegion.Text,
				DeepLSubscriptionKey = txtDeepLTranslationKey.Text,
				DeepLSubscriptionRegion = cmbDeeplApiType.SelectedIndex.ToString()
			};

			IsBusy(true);
			new Action<string, string, TranslationOptions, List<string>, string, ResxProgressCallback, bool, bool, bool, string>(TranslateResxFilesAsync).BeginInvoke(
				txtSourceResx.Text,
				srcLng,
				translationOptions,
				destLanguages,
				txtOutputDir.Text,
				ResxWorkingProgress,
				translateFromKey,
				checkBoxTranslateOnlyNew.Checked,
				chkCSVOutput.Checked,
				txtCSVOutputDir.Text,
				(x) => IsBusy(false),
				null);
		}

		private delegate void ResxProgressCallback(int max, int pos, string status);

		async void TranslateResxFilesAsync(
			string sourceResx,
			string sourceLng,
			TranslationOptions translationOptions,
			List<string> desLanguages, string destDir,
			ResxProgressCallback progress,
			bool translateFromKey,
			bool translateOnlyNewKeys,
			bool generateCsv,
			string generateCsvDir)
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

				string[] csvOutputDataBuffer = null;
				if (generateCsv)
					csvOutputDataBuffer = new string[max];

				List<XmlNode> destinationDataList = null;
				var destTranslateOnlyNewKeys =
						translateOnlyNewKeys &&
						File.Exists(destFile);

				if (destTranslateOnlyNewKeys)
				{
					var destDoc = new XmlDocument();
					destDoc.Load(destFile);
					destinationDataList = ResxTranslator.ReadResxData(destDoc);
				}

				pos = 0;
				status = "Translating language: " + destLng;
				progress.BeginInvoke(max, pos, status, null, null);

				try
				{
					int destIndexCorrection = 0;
					foreach (var (node, index) in dataList.Select((n, i) => (n, i)))
					{
						status = "Translating language: " + destLng;
						pos += 1;
						progress.BeginInvoke(max, pos, status, null, null);
						var valueNode = ResxTranslator.GetDataValueNode(node);
						if (valueNode == null)
							continue;

						var keyNode = ResxTranslator.GetDataKeyName(node);
						var orgText = translateFromKey ? keyNode : valueNode.InnerText;

						if (destTranslateOnlyNewKeys)
						{
							int destIndex = index - destIndexCorrection;
							var destNode = ResxTranslator.GetDataKeyName(destinationDataList.ElementAt(destIndex));
							if (destNode == keyNode)
							{
								valueNode.InnerText = ResxTranslator.GetDataValueNode(destinationDataList.ElementAt(destIndex)).InnerText;
								if (generateCsv)
									csvOutputDataBuffer[index] = keyNode + "," + valueNode.InnerText;
								continue;
							}
							else
								destIndexCorrection++;
						}

						if (string.IsNullOrWhiteSpace(orgText))
							continue;

						if (translationOptions.ServiceType == ServiceTypeEnum.Google)
						{
							// There is no longer a key to validate
							// the key
							var textTranslatorUrlKey = "";

							string translated = string.Empty;
							bool success = false;
							trycount = 0;
							do
							{
								try
								{
									success = GTranslateService.Translate(orgText, sourceLng, destLng, textTranslatorUrlKey, out translated);
								}
								catch (Exception)
								{
									success = false;
								}
								trycount++;

								if (!success)
								{
									status = "Translating language: " + destLng + " , key '" + keyNode + "' failed to translate in try " + trycount;
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
								try
								{
									string message = "\r\nKey '" + keyNode + "' translation to language '" + destLng + "' failed.";
									File.AppendAllText(errorLogFile, message);
								}
								catch
								{
								}
							}
						}
						else if (translationOptions.ServiceType == ServiceTypeEnum.Microsoft)
						{
							var translationResult = await MsTranslateService.TranslateAsync(orgText, sourceLng, destLng,
								translationOptions.MsSubscriptionKey, translationOptions.MsSubscriptionRegion);

							if (translationResult.Success)
							{
								valueNode.InnerText = translationResult.Result;
							}
							else
							{
								hasErrors = true;
								var key = ResxTranslator.GetDataKeyName(node);
								try
								{
									string message = "\r\nKey '" + key + "' translation to language '" + destLng + "' failed. ";
									if (!string.IsNullOrEmpty(translationResult.Result))
										message += " Error message: " + translationResult.Result;

									File.AppendAllText(errorLogFile, message);
								}
								catch { }
							}
						}
						else if (translationOptions.ServiceType == ServiceTypeEnum.DeepL)
						{
							var translationResult = await DeepLTranslateService.TranslateAsync(orgText, sourceLng, destLng,
								translationOptions.DeepLSubscriptionKey, translationOptions.DeepLSubscriptionRegion);

							if (translationResult.Success)
							{
								valueNode.InnerText = translationResult.Result;
							}
							else
							{
								hasErrors = true;
								var key = ResxTranslator.GetDataKeyName(node);
								try
								{
									string message = "\r\nKey '" + key + "' translation to language '" + destLng + "' failed. ";
									if (!string.IsNullOrEmpty(translationResult.Result))
										message += " Error message: " + translationResult.Result;

									File.AppendAllText(errorLogFile, message);
								}
								catch { }
							}
						}
						if (generateCsv)
							csvOutputDataBuffer[index] = keyNode + "," + valueNode.InnerText;
					}
				}
				finally
				{
					// now save the data!
					doc.Save(destFile);

					if (generateCsv)
					{
						if (!Directory.Exists(generateCsvDir))
							Directory.CreateDirectory(generateCsvDir);
						var csvFile = Path.Combine(generateCsvDir, sourceResxFilename + "." + destLng + ".resx.csv");

						File.WriteAllLines(csvFile, new string[] { "KEY,Value" }, Encoding.UTF8);
						File.AppendAllLines(csvFile, csvOutputDataBuffer, Encoding.UTF8);
					}
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
				lblResxTranslateStatus.Text = $"Processing {max:00}/{pos:00}, " + status;
			}
		}


		void ImportExcel()
		{
			var sheetName = cmbExcelSheets.Text;
			var excelFile = txtExcelFile.Text;
			var resxFile = txtExcelResx.Text;
			var sheetKeyColumn = cmbExcelKey.Text;
			var sheetTranslation = cmbExcelTranslation.Text;
			var create = chkExcelCreateAbsent.Checked;

			IsBusy(true);
			new Action<string, string, string, string, string, bool>(ImportExcel).BeginInvoke(
				excelFile,
				resxFile,
				sheetName,
				sheetKeyColumn,
				sheetTranslation,
				create,
				(x) => IsBusy(false),
				null);
		}

		private void ImportExcel(string excelFile, string resxFile, string sheetName, string sheetKeyColumn,
			string sheetTranslation, bool create)
		{
			var doc = new XmlDocument();
			doc.Load(resxFile);
			var dataList = ResxTranslator.ReadResxData(doc);

			var excelLanguages = ResxExcel.ReadExcelLanguage(excelFile, sheetName, sheetKeyColumn, sheetTranslation);
			foreach (var lngPair in excelLanguages)
			{
				var node = dataList.FirstOrDefault(x => ResxTranslator.GetDataKeyName(x) == lngPair.Key);
				if (node != null)
				{
					ResxTranslator.SetDataValue(doc, node, lngPair.Value);
				}
				else
				{
					ResxTranslator.AddLanguageNode(doc, lngPair.Key, lngPair.Value);
				}
			}
			doc.PreserveWhitespace = false;
			var writer = new XmlTextWriter(resxFile, Encoding.UTF8)
			{
				Formatting = Formatting.Indented
			};
			doc.Save(writer);
			writer.Close();
		}

		bool IsGoogleTranslatorLoaded()
		{
			if (webBrowser.Document != null &&
				(webBrowser.ReadyState == WebBrowserReadyState.Loaded ||
				 webBrowser.ReadyState == WebBrowserReadyState.Complete))
			{
				return true;
			}
			return false;
		}

		private void frmMain_Load(object sender, EventArgs e)
		{
			FillComboBoxes();
			txtMsTranslationKey.Text = Properties.Settings.Default.MicrosoftTranslatorKey;
			txtMsTranslationRegion.Text = Properties.Settings.Default.MicrosoftTranslatorRegion;
			txtDeepLTranslationKey.Text = Properties.Settings.Default.DeepLTranslatorKey;
			cmbDeeplApiType.SelectedIndex = Properties.Settings.Default.DeepLTranslatorType;
			tabMain.TabPages.Remove(tabBrowser);
		}

		private async void btnTranslate_ClickAsync(object sender, EventArgs e)
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

			var translationOptions = new TranslationOptions
			{
				ServiceType = ServiceType,
				MsSubscriptionKey = txtMsTranslationKey.Text,
				MsSubscriptionRegion = txtMsTranslationRegion.Text,
				DeepLSubscriptionKey = txtDeepLTranslationKey.Text,
				DeepLSubscriptionRegion = cmbDeeplApiType.SelectedIndex.ToString()
			};


			IsBusy(true);

			if (ServiceType == ServiceTypeEnum.Google)
			{
				// There is no longer a key to validate
				var textTranslatorUrlKey = "";

				GTranslateService.TranslateAsync(
				text, lngSrc, lngDest, textTranslatorUrlKey,
				(success, result) =>
				{
					SetResult(result);
					IsBusy(false);
				});
			}
			else if (ServiceType == ServiceTypeEnum.Microsoft)
			{
				var translationResult = await MsTranslateService.TranslateAsync(text, lngSrc, lngDest, txtMsTranslationKey.Text, txtMsTranslationRegion.Text);

				if (translationResult.Success)
				{
					SetResult(translationResult.Result);
				}
				else
				{
					if (!string.IsNullOrEmpty(translationResult.Result))
						SetResult(translationResult.Result);
					else
						SetResult("Translation Failed!");
				}

				IsBusy(false);
			}
			else
			{
				var translationResult = await DeepLTranslateService.TranslateAsync(text, lngSrc, lngDest, txtDeepLTranslationKey.Text, cmbDeeplApiType.SelectedIndex.ToString());

				if (translationResult.Success)
				{
					SetResult(translationResult.Result);
				}
				else
				{
					if (!string.IsNullOrEmpty(translationResult.Result))
						SetResult(translationResult.Result);
					else
						SetResult("Translation Failed!");
				}

				IsBusy(false);
			}
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
					txtCSVOutputDir.Text = Path.GetDirectoryName(txtSourceResx.Text);
				}

				// reset selection
				foreach (int i in lstResxLanguages.CheckedIndices)
					lstResxLanguages.Items[i].Checked = false;

				// select based on what is in destination
				string[] languageFilesInDir = Directory.GetFiles(Path.GetDirectoryName(txtSourceResx.Text), "*.resx");

				foreach (var lngFile in languageFilesInDir)
				{
					var languageTag = ReadLanguageName(lngFile);
					if (languageTag != "")
					{
						var haskey = _languages.FirstOrDefault(x => x.Key.Equals(languageTag, StringComparison.InvariantCultureIgnoreCase));

						lstResxLanguages.Items[lstResxLanguages.Items.IndexOfKey(haskey.Key)].Checked = true;
					}
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
			// Remove dialog open directly without selected directory --> dlg.RootFolder = Environment.SpecialFolder.MyComputer;
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
			if (ServiceType == ServiceTypeEnum.Google && !IsGoogleTranslatorLoaded())
			{
				MessageBox.Show("Google Translator is not loaded.", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			TranslateResxFiles();
		}

		private void lnkAbout_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			using (var frm = new frmAbout())
			{
				frm.ShowDialog();
			}
		}

		private void btnOpenExcel_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtExcelFile.Text))
			{
				MessageBox.Show("Please select an excel file.", "Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var excel = ResxExcel.ReadExcel(txtExcelFile.Text);
			if (excel == null)
			{
				MessageBox.Show("Failed to read excel file", "Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			cmbExcelSheets.DataSource = excel.SheetNames;
			cmbExcelKey.DataSource = excel.SheetColumnsKey;
			cmbExcelTranslation.DataSource = excel.SheetColumnsTranslation;
			if (Array.IndexOf(excel.SheetColumnsKey, "Name") != -1)
			{
				cmbExcelKey.SelectedIndex = Array.IndexOf(excel.SheetColumnsKey, "Name");
			}
			btnImportExcel.Enabled = true;
		}

		private void btnSelectExcel_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog
			{
				Filter = "Excel File|*.xls;xlsx"
			};
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtExcelFile.Text = dlg.FileName;
			}
		}

		private void btnExcelResx_Click(object sender, EventArgs e)
		{
			var dlg = new OpenFileDialog
			{
				Filter = "ResourceX File|*.resx"
			};
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtExcelResx.Text = dlg.FileName;
			}
		}

		private void cmbExcelSheets_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtExcelFile.Text))
				return;
			if (!File.Exists(txtExcelFile.Text))
			{
				MessageBox.Show("Please select an excel file.", "Select Resx", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (cmbExcelSheets.SelectedIndex != -1)
			{
				var cols = ResxExcel.GetExcelSheetColumns(txtExcelFile.Text, cmbExcelSheets.Text);
				cmbExcelKey.DataSource = cols;
				var cols2 = ResxExcel.GetExcelSheetColumns(txtExcelFile.Text, cmbExcelSheets.Text);
				cmbExcelTranslation.DataSource = cols2;
				if (Array.IndexOf(cols, "Name") != -1)
				{
					cmbExcelKey.Text = "Name";
				}
			}
		}

		private void btnImportExcel_Click(object sender, EventArgs e)
		{
			if (!File.Exists(txtExcelResx.Text))
			{
				MessageBox.Show("Please select ResX file.", "Select Resx", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (!File.Exists(txtExcelFile.Text))
			{
				MessageBox.Show("Please select an excel file.", "Select Resx", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (cmbExcelSheets.Items.Count == 0)
			{
				MessageBox.Show("Please open excel file and select key and translation columns.", "Open Excel", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			if (cmbExcelKey.SelectedIndex == -1 || cmbExcelSheets.SelectedIndex == -1 || cmbExcelTranslation.SelectedIndex == -1)
			{
				MessageBox.Show("Please select excel columns.", "Select Columns", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			ImportExcel();
		}

		private void webBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
		{
			var doc = webBrowser.Document;
			if (doc == null)
				return;
			foreach (HtmlElement imgElemt in doc.Images)
			{
				imgElemt.SetAttribute("src", "");
			}
		}

		private void RbtnMsTranslateService_CheckedChanged(object sender, EventArgs e)
		{
			txtMsTranslationKey.Enabled = rbtnMsTranslateService.Checked;
			txtMsTranslationRegion.Enabled = rbtnMsTranslateService.Checked;
		}
		private void rbtnDeepLTranslateService_CheckedChanged(object sender, EventArgs e)
		{
			txtDeepLTranslationKey.Enabled = rbtnDeepLTranslateService.Checked;
			cmbDeeplApiType.Enabled = rbtnDeepLTranslateService.Checked;
		}

		private void chkCSVOutput_CheckedChanged(object sender, EventArgs e)
		{
			txtCSVOutputDir.Enabled = btnSelectCSVOutputDir.Enabled = chkCSVOutput.Checked;
		}

		private void btnSelectCSVOutputDir_Click(object sender, EventArgs e)
		{
			var dlg = new FolderBrowserDialog
			{
				ShowNewFolderButton = true,
				Description = "Please select output directory for CSV files:"
			};
			if (txtCSVOutputDir.Text.Length > 0)
			{
				dlg.SelectedPath = txtCSVOutputDir.Text;
			}
			if (dlg.ShowDialog() == DialogResult.OK)
			{
				txtCSVOutputDir.Text = dlg.SelectedPath;
			}
		}

		private void tabMain_Selected(object sender, TabControlEventArgs e)
		{
			var s = (TabControl)sender;
			if (_translateSettingsChanged)
			{
				Properties.Settings.Default.MicrosoftTranslatorKey = txtMsTranslationKey.Text;
				Properties.Settings.Default.MicrosoftTranslatorRegion = txtMsTranslationRegion.Text;
				Properties.Settings.Default.DeepLTranslatorKey = txtDeepLTranslationKey.Text;
				Properties.Settings.Default.DeepLTranslatorType = (short)cmbDeeplApiType.SelectedIndex;
				Properties.Settings.Default.Save();
				_translateSettingsChanged = false;
			}
			_translateSettingsChanged = s.SelectedTab.Name == "tabTranslateServices";
		}
	}
}
