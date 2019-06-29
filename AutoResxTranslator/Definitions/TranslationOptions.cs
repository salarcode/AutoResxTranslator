using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoResxTranslator.Definitions
{
	public class TranslationOptions
	{
		public ServiceTypeEnum ServiceType { get; set; }

		public string MsSubscriptionKey { get; set; }

		public string MsSubscriptionRegion { get; set; }
	}
}
