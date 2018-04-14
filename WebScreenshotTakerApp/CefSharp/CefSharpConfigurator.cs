using CefSharp;
using System;
using System.IO;

namespace WebScreenshotTakerApp.CefSharp
{
	public static class CefSharpConfigurator
	{
		private static readonly object _syncLock = new object();

		private static bool _wasCefInitialized;
		private static bool _wasCefShutdown;

		public static void InitializeCef(bool ignoreCertificateErrors = true, bool multiThreadedMessageLoop = true, bool windowlessRenderingEnabled = true)
		{
			lock (_syncLock)
			{
				if (_wasCefInitialized)
				{
					return;
				}

				var baseDirPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

				var resourcesDirPath = Path.Combine(baseDirPath, Environment.Is64BitProcess ? "x64" : "x86");
				var resourcesDirectory = new DirectoryInfo(resourcesDirPath);

				if (!resourcesDirectory.Exists)
				{
					throw new Exception($"{nameof(resourcesDirectory)} doesn't exist.");
				}

				var browserSubprocessPath = Path.Combine(resourcesDirPath, "CefSharp.BrowserSubprocess.exe");

				var localesDirPath = Path.Combine(resourcesDirPath, "locales");
				var localesDirectory = new DirectoryInfo(localesDirPath);

				if (!localesDirectory.Exists)
				{
					throw new Exception($"{nameof(localesDirectory)} doesn't exist.");
				}

				var cefSettings = new CefSettings
				{
					IgnoreCertificateErrors = ignoreCertificateErrors,
					MultiThreadedMessageLoop = multiThreadedMessageLoop,
					WindowlessRenderingEnabled = windowlessRenderingEnabled,

					BrowserSubprocessPath = browserSubprocessPath,
					ResourcesDirPath = resourcesDirPath,
					LocalesDirPath = localesDirPath
				};

				if (_wasCefInitialized)
				{
					return;
				}

				Cef.Initialize(cefSettings, performDependencyCheck: false, browserProcessHandler: null);

				_wasCefInitialized = true;
			}
		}

		public static void ShutdownCef()
		{
			lock (_syncLock)
			{
				if (_wasCefShutdown)
				{
					return;
				}

				Cef.Shutdown();

				_wasCefShutdown = true;
			}
		}
	}
}
