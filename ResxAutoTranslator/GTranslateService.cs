using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

/* 
 * ResxAutoTranslator
 * by Salar Khalilzadeh
 * 
 * https://resxautotranslator.codeplex.com/
 * Mozilla Public License v2
 */
namespace ResxAutoTranslator
{
	public class GTranslateService
	{
		public delegate void TranslateCallBack(bool succeed, string result);
		public static void TranslateAsync(string text, string sourceLng, string destLng, TranslateCallBack callBack)
		{
			var request = CreateWebRequest(text, sourceLng, destLng);
			request.BeginGetResponse(TranslateRequestCallBack, new KeyValuePair<WebRequest, TranslateCallBack>(request, callBack));
		}

		public static bool Translate(string text, string sourceLng, string destLng, out string result)
		{
			var request = CreateWebRequest(text, sourceLng, destLng);
			try
			{
				var response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode != HttpStatusCode.OK)
				{
					result = "Response is failed with code: " + response.StatusCode;
					return false;
				}

				using (var stream = response.GetResponseStream())
				{
					string output;
					var succeed = ReadGoogleTranslatedResult(stream, out output);
					result = output;
					return succeed;
				}
			}
			catch (Exception ex)
			{
				result = ex.Message;
				return false;
			}
		}

		static WebRequest CreateWebRequest(string text, string lngSourceCode, string lngDestinationCode)
		{
			var url = @"https://translate.google.com/translate_a/t?client=t&ie=UTF-8&oe=UTF-8&tsel=0&uptl={1}&alttl={0}&sc=1&text={2}";

			text = HttpUtility.UrlEncode(text);

			url = string.Format(url, lngSourceCode, lngDestinationCode, text);
			var create = (HttpWebRequest)WebRequest.Create(url);
			create.UserAgent = "Mozilla/5.0 (Windows NT 6.2; WOW64; rv:20.0) Gecko/20100101 Firefox/20.0";
			create.Timeout = 50 * 1000;
			return create;
		}

		private static void TranslateRequestCallBack(IAsyncResult ar)
		{
			var pair = (KeyValuePair<WebRequest, TranslateCallBack>)ar.AsyncState;
			var request = pair.Key;
			var callback = pair.Value;
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.EndGetResponse(ar);
				if (response.StatusCode != HttpStatusCode.OK)
				{
					callback(false, "Response is failed with code: " + response.StatusCode);
					return;
				}

				using (var stream = response.GetResponseStream())
				{
					string output;
					var succeed = ReadGoogleTranslatedResult(stream, out output);

					callback(succeed, output);
				}
			}
			catch (Exception ex)
			{
				callback(false, "Request failed.\r\n" + ex.Message);
			}
			finally
			{
				if (response != null)
				{
					response.Close();
				}
			}
		}

		static bool ReadGoogleTranslatedResult(Stream rawdata, out string result)
		{
			string text;
			using (var reader = new StreamReader(rawdata, Encoding.UTF8))
			{
				text = reader.ReadToEnd();
			}

			try
			{
				dynamic obj = SimpleJson.DeserializeObject(text);

				// the main trick :)
				var final = "";

				// the number of lines
				int lines = obj[0].Count;
				for (int i = 0; i < lines; i++)
				{
					// the translated text.
					final += (obj[0][i][0]).ToString();
				}
				result = final;
				return true;
			}
			catch (Exception ex)
			{
				result = ex.Message;
				return false;
			}
		}

	}
}
