using System.Globalization;
using System.Threading;

namespace WebScreenshotTakerApp.Framework.Culture
{
	public static class CultureConfigurator
	{
		public static void Configure()
		{
			Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
		}
	}
}
