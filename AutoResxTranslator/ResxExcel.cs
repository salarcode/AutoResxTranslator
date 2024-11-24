using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace AutoResxTranslator
{
	public class ResxExcel
	{
		const string excelConnection =
			"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 12.0 Xml;HDR=YES;IMEX=1';";
		public class ExcelFileInfo
		{
			public string[] SheetNames { get; set; }
			public string[] SheetColumnsKey { get; set; }
			public string[] SheetColumnsTranslation { get; set; }
		}

		public static ExcelFileInfo ReadExcel(string excelFile)
		{
			var result = new ExcelFileInfo();
			try
			{
				var connString = string.Format(excelConnection, excelFile);
				using (var objConn = new OleDbConnection(connString))
				{
					objConn.Open();
					using (var schema = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null))
					{
						if (schema == null)
						{
							return null;
						}

						result.SheetNames = new string[schema.Rows.Count];

						// Add the sheet name to the string array.
						for (int j = 0; j < schema.Rows.Count; j++)
						{
							var key = schema.Rows[j]["TABLE_NAME"].ToString();
							if (key.EndsWith("'"))
								key = key.Remove(key.Length - 1, 1);
							if (key.StartsWith("'"))
								key = key.Remove(0, 1);
							if (key.EndsWith("$"))
								key = key.Remove(key.Length - 1, 1);
							result.SheetNames[j] = key;
						}
					}
					if (result.SheetNames.Length == 0)
						return result;


					using (var cmd = objConn.CreateCommand())
					using (var adapter = new OleDbDataAdapter(cmd))
					using (var ds = new DataSet())
					{
						cmd.CommandText = "SELECT Top 1 * FROM [" + result.SheetNames[0] + "$]";

						adapter.Fill(ds);
						var columns = new List<string>();
						foreach (DataTable table in ds.Tables)
						{
							foreach (DataColumn column in table.Columns)
							{
								columns.Add(column.ColumnName);
							}
						}
						result.SheetColumnsKey = columns.ToArray();
						result.SheetColumnsTranslation = columns.ToArray();
					}
				}

				return result;
			}
			catch (OleDbException)
			{
				throw;
			}
			catch (IOException)
			{
				throw;
			}
			catch (Exception)
			{
				return null;
			}
		}

		public static string[] GetExcelSheetNames(string excelFile)
		{
			OleDbConnection objConn = null;
			DataTable dt = null;

			try
			{
				var connString = string.Format(excelConnection, excelFile);

				objConn = new OleDbConnection(connString);
				objConn.Open();
				dt = objConn.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

				if (dt == null)
				{
					return null;
				}

				var excelSheets = new string[dt.Rows.Count];
				int i = 0;

				// Add the sheet name to the string array.
				foreach (DataRow row in dt.Rows)
				{
					var key = row["TABLE_NAME"].ToString();
					if (key.EndsWith("'"))
						key = key.Remove(key.Length - 1, 1);
					if (key.StartsWith("'"))
						key = key.Remove(0, 1);
					if (key.EndsWith("$"))
						key = key.Remove(key.Length - 1, 1);

					excelSheets[i] = key;
					i++;
				}
				return excelSheets;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				// Clean up.
				if (objConn != null)
				{
					objConn.Close();
					objConn.Dispose();
				}

				dt?.Dispose();
			}
		}

		public static string[] GetExcelSheetColumns(string excelFile, string sheetName)
		{
			var connString = string.Format(excelConnection, excelFile);

			var oconn = new OleDbConnection(connString);
			// if you don't want to show the header row (first row) in the grid
			// use 'HDR=NO' in the string

			using (var cmd = oconn.CreateCommand())
			using (var adapter = new OleDbDataAdapter(cmd))
			using (var ds = new DataSet())
			{
				cmd.CommandText = "SELECT Top 1 * FROM [" + sheetName + "$]";

				adapter.Fill(ds);
				var columns = new List<string>();
				foreach (DataTable table in ds.Tables)
				{
					foreach (DataColumn column in table.Columns)
					{
						columns.Add(column.ColumnName);
					}
				}
				return columns.ToArray();
			}
		}

		public static List<KeyValuePair<string, string>> ReadExcelLanguage(
			string excelFile,
			string sheetName,
			string keyColumn,
			string translationColumn)
		{
			var connString = string.Format(excelConnection, excelFile);
			var oconn = new OleDbConnection(connString);

			using (var cmd = oconn.CreateCommand())
			using (var adapter = new OleDbDataAdapter(cmd))
			using (var dt = new DataTable())
			{
				cmd.CommandText = "SELECT * FROM [" + sheetName + "$]";

				adapter.Fill(dt);
				var result = new List<KeyValuePair<string, string>>();
				foreach (DataRow row in dt.Rows)
				{
					var key = row[keyColumn].ToString();
					if (string.IsNullOrWhiteSpace(key))
						continue;

					var value = row[translationColumn].ToString();
					if (string.IsNullOrEmpty(value))
						continue;

					result.Add(new KeyValuePair<string, string>(key, value));
				}
				return result;
			}
		}
	}
}
