using log4net.Config;
using System;
using System.IO;
using System.Runtime.InteropServices;
using WebScreenshotTakerApp.CefSharp;
using WebScreenshotTakerApp.Framework.Culture;
using WebScreenshotTakerApp.Framework.Handlers;
using WebScreenshotTakerApp.Logging;

namespace WebScreenshotTakerApp.Tests
{
	class Program
	{
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();

		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

		const int SW_HIDE = 0;
		const int SW_SHOW = 5;

		static void Main(string[] args)
		{
			var handle = GetConsoleWindow();

			ShowWindow(handle, SW_HIDE);

			Initialize();

			using (var webScreenshotServiceController = new WebScreenshotServiceController())
			{
				var url = "https://www.facebook.com/";

				var localPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, "Files", "fb.jpeg");

				webScreenshotServiceController.TakeScreenshotAndGenerateThumbnail(url, localPath);
			}

			Shutdown();
		}

		private static void Initialize()
		{
			// Culture
			CultureConfigurator.Configure();

			// Log4Net
			XmlConfigurator.Configure();

			// Log4Net
			Log4netHelper.LoggerPrefix = "WebScreenshotTakerApp";

			// Exception Handler
			ExceptionHandler.Initialize();

			// Assembly Resolver
			CefSharpAssemblyResolver.Initialize();

			// CefSharp
			CefSharpConfigurator.InitializeCef();
		}

		private static void Shutdown()
		{
			// CefSharp
			CefSharpConfigurator.ShutdownCef();
		}
	}
}
