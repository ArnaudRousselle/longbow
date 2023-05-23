using System.Diagnostics;
using Microsoft.Practices.Prism.Logging;

namespace LongBow.Common.Logger
{
	public class LongBowLogger : ILoggerFacade
	{
		public void Log(string message, Category category, Priority priority)
		{
			Debug.WriteLine(string.Format("{1} ({2}): {0}", message, category, priority));
		}
	}
}
