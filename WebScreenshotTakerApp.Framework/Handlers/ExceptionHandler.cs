using System;
using WebScreenshotTakerApp.Logging;

namespace WebScreenshotTakerApp.Framework.Handlers
{
	public static class ExceptionHandler
	{
		public static void Initialize()
		{
			AppDomain.CurrentDomain.UnhandledException += AppDomainException;
		}

		static void AppDomainException(object sender, UnhandledExceptionEventArgs e)
		{
			try
			{
				var exception = (Exception)e.ExceptionObject;
				Log4netHelper.All(x => x.Error($"AppDomain Unhandled exception: {exception.ToString()}."));
			}
			catch { }
		}
	}
}
