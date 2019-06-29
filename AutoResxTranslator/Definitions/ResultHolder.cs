using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoResxTranslator.Definitions
{
	public class ResultHolder<T>
	{
		public ResultHolder()
		{ }

		public ResultHolder(bool success)
		{
			Success = success;
		}
		public ResultHolder(bool success, T result)
		{
			Success = success;
			Result = result;
		}

		public bool Success { get; set; }

		public T Result { get; set; }
	}
}
