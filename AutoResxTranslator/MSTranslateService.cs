using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using AutoResxTranslator.Definitions;

namespace AutoResxTranslator
{
	/// <summary>
	/// Translation service using Microsoft Cogntive service.
	/// 
	/// ref: https://azure.microsoft.com/en-in/services/cognitive-services/translator-text-api/
	/// </summary>
	public class MsTranslateService
	{
		private const string MsCognitiveServicesApiUrl = "https://api-apc.cognitive.microsofttranslator.com";
		//private static string subscriptionKey = "<YOUR-SUBSCRIPTION-KEY-HERE>";
		//private static string region = "centralindia";

		public static async Task<ResultHolder<string>> TranslateAsync(string text,
			string fromLanguage,
			string toLanguage,
			string subscriptionKey,
			string region)
		{
			if (fromLanguage.Equals("auto") || fromLanguage.Equals(""))
			{
				fromLanguage = null;
			}

			var route = "/translate?api-version=3.0&to=" + toLanguage + "&from=" + fromLanguage;

			try
			{
				var body = new object[] { new { Text = text } };
				var requestBody = JsonConvert.SerializeObject(body);

				using (var client = new HttpClient())
				using (var request = new HttpRequestMessage())
				{
					// Build the request.
					request.Method = HttpMethod.Post;
					request.RequestUri = new Uri(MsCognitiveServicesApiUrl + route);
					request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
					request.Headers.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
					request.Headers.Add("Ocp-Apim-Subscription-Region", region);

					// Send the request and get response.
					var response = await client.SendAsync(request).ConfigureAwait(false);

					if (response.StatusCode == System.Net.HttpStatusCode.OK)
					{
						// Read response as a string.
						var resultFromMs = await response.Content.ReadAsStringAsync();

						var deserializedOutput = JsonConvert.DeserializeObject<MsTranslationApi.TranslationResult[]>(resultFromMs);

						// Iterate over the deserialized results.
						foreach (var output in deserializedOutput)
						{
							// Iterate over the results, return the first result
							foreach (var t in output.Translations)
							{
								return new ResultHolder<string>(true, t.Text);
							}
						}
					}
					else
					{
						return new ResultHolder<string>(false, "Translation failed! Exception: " + response.ReasonPhrase);
					}
				}
				return new ResultHolder<string>(false);
			}
			catch (Exception e)
			{
				return new ResultHolder<string>(false, "Translation failed! Exception: " + e.Message);
			}
		}
	}
}